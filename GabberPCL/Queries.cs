﻿using System.Collections.Generic;
using System.Linq;
using GabberPCL.Models;
using SQLiteNetExtensions.Extensions;

namespace GabberPCL
{
    public static class Queries
    {
        public static void AddProjects(List<Project> _projects)
        {
            // Resync database when data pulled from the server.
            // Projects that the user may have had access to could change, and if they
            // are not removed, then sessions could be created for them.
            Session.Connection.DeleteAll<Project>();
            foreach (var p in _projects)
            {
                Session.Connection.InsertOrReplace(p);
                Session.Connection.InsertOrReplaceAllWithChildren(p.Prompts);
            }
        }

        public static List<Project> AllProjects()
        {
            return Session.Connection.GetAllWithChildren<Project>().OrderByDescending(p => p.ID).ToList();
        }

        public static List<InterviewSession> AllNotUploadedInterviewSessionsForActiveUser()
        {
            var sessions = Session.Connection.GetAllWithChildren<InterviewSession>().Where((i) => !i.IsUploaded);
            return sessions.OrderByDescending(i => i.CreatedAt).ToList();
        }

        public static Project ProjectById(int projectID)
        {
            var project = Session.Connection.GetWithChildren<Project>(projectID);
            // Only show the active topics to the user
            project.Prompts = project.Prompts.Where((p) => p.IsActive).ToList();
            return project;
        }

        public static User UserById(int userID) => Session.Connection.Get<User>(userID);

        public static User UserByEmail(string email)
        {
            return Session.Connection.Table<User>().Where(u => u.Email == email).FirstOrDefault();
        }

        public static User FindOrInsertUser(User user, string email)
        {
            var usr = Session.Connection.Table<User>().Where(u => u.Email == email).FirstOrDefault();

            if (usr == null)
            {
                AddUser(user);
                return user;
            }
            return usr;
        }

        public static void AddUser(User user) => Session.Connection.Insert(user);

        public static void SetActiveUser(DataUserTokens response)
        {
            var user = FindOrInsertUser(response.User, response.User.Email);
            user.Selected = true;
            user.IsActive = true;
            Session.ActiveUser = user;
            Session.ActiveUser.Name += " (You)";
            Session.Token = response.Tokens;
        }

        public static async void UploadInterviewSessionsAsync()
        {
            var api = new RestClient();

            foreach (var s in AllInterviewSessionsNotUploaded())
            {
                var didUpload = await api.Upload(s);
                if (didUpload)
                {
                    s.IsUploaded = didUpload;
                    Session.Connection.Update(s);
                }
            }
        }

        public static List<InterviewSession> AllInterviewSessionsNotUploaded()
        {
            return Session.Connection.GetAllWithChildren<InterviewSession>((i) => !i.IsUploaded);
        }

        public static void AddInterviewSession(InterviewSession InterviewSession) => Session.Connection.Insert(InterviewSession);

        public static List<User> AllParticipantsUnSelected() {
            var participants = Session.Connection.Table<User>().ToList();
            foreach (var p in participants) p.Selected = false;
            // participants.FirstOrDefault(i => i.Email == Session.ActiveUser.Email && !i.Name.Contains("You")).Name += " (You)";
            return participants;
        }
        public static List<User> AllParticipants() => Session.Connection.Table<User>().ToList();
        public static List<User> SelectedParticipants() => AllParticipants().FindAll((p) => p.Selected);

        public static void CreateAnnotation(int start, string interviewID, int promptID)
        {
            Session.Connection.Insert(new InterviewPrompt
            {
                Start = start,
                End = 0,
                InterviewID = interviewID,
                PromptID = promptID
            });
        }

        public static void AddSelectedParticipantsToInterviewSession(string InterviewSessionID)
        {
            foreach (var participant in SelectedParticipants())
            {
                // TODO: we store the Email/UserID here to simplify logic on the server, which is bad
                // as this is duplicated information between InterviewParticipant, etc
                // We represent a participant as a User, but in doing so do not send the Email/Name
                // when creating a session object (as they are abstracted away), which imho is not ideal.
                Session.Connection.Insert(new InterviewParticipant
                {
                    InterviewID = InterviewSessionID,
                    // True if participant was the intervieweer
                    Role = Session.ActiveUser.Id == participant.Id,
                    Name = participant.Name,
                    Email = participant.Email,
                    UserID = participant.Id
                });
            }
        }

        public static List<InterviewParticipant> ParticipantsForSession(string InterviewSessionID)
        {
            return Session.Connection.Table<InterviewParticipant>().Where((ip) => ip.InterviewID == InterviewSessionID).ToList();
        }

        public static InterviewSession LastInterviewSession => Session.Connection.Table<InterviewSession>().Last();

        public static List<InterviewPrompt> AnnotationsForLastSession()
        {
            var AnnotationTable = Session.Connection.Table<InterviewPrompt>();
            var LastAnnotationInserted = AnnotationTable.Last().InterviewID;
            return AnnotationTable.Where((a) => a.InterviewID == LastAnnotationInserted).ToList();
        }

        public static void UpdateAnnotation(InterviewPrompt annotation) => Session.Connection.Update(annotation);
    }
}
