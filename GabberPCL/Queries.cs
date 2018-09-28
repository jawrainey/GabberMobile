using System.Collections.Generic;
using System.Linq;
using GabberPCL.Models;
using GabberPCL.Resources;
using Newtonsoft.Json;
using SQLiteNetExtensions.Extensions;
using System.Globalization;

namespace GabberPCL
{
    public static class Queries
    {
        public static string FormatFromSeconds(int seconds)
        {
            var timeSpan = System.TimeSpan.FromSeconds(seconds);
            return string.Format("{0:D2}:{1:D2}", (int)timeSpan.TotalMinutes, timeSpan.Seconds);
        }

        public static void AddProjects(List<Project> _projects)
        {
            // Resync database when data pulled from the server.
            Session.Connection.DeleteAll<Project>();

            foreach (Project project in _projects)
            {
                project.SerializeJson();
                Session.Connection.Insert(project);
            }
        }

        public static List<Project> AllProjects()
        {
            List<Project> res = Session.Connection.GetAllWithChildren<Project>()
                .OrderByDescending(p => p.ID).ToList();

            foreach (Project proj in res)
            {
                proj.LoadJson();
            }

            return res;
        }

        public static void AddLanguages(List<LanguageChoice> languages)
        {
            // Resync database when data pulled from the server.
            Session.Connection.DeleteAll<LanguageChoice>();

            foreach (LanguageChoice lang in languages)
            {
                Session.Connection.Insert(lang);
            }
        }

        public static List<LanguageChoice> AllLanguages()
        {
            return Session.Connection.GetAllWithChildren<LanguageChoice>()
                          .OrderBy(lang => lang.Code).ToList();
        }

        public static List<InterviewSession> AllNotUploadedInterviewSessionsForActiveUser()
        {
            var sessions = Session.Connection.GetAllWithChildren<InterviewSession>().Where((i) => !i.IsUploaded);
            List<InterviewSession> res = sessions.OrderByDescending(i => i.CreatedAt).ToList();

            foreach (InterviewSession session in res)
            {
                session.LoadJson();
            }

            return res;
        }

        public static Project ProjectById(int projectID)
        {
            var project = Session.Connection.Get<Project>(projectID);
            project.LoadJson();
            return project;
        }

        public static void SaveActiveUser()
        {
            Session.Connection.InsertOrReplace(Session.ActiveUser);
        }

        public static User UserById(int userID) => Session.Connection.Get<User>(userID);

        public static User UserByEmail(string email)
        {
            return Session.Connection.Table<User>().Where(u => u.Email == email).FirstOrDefault();
        }

        public static User FindOrInsertUser(User user, string email)
        {
            var usr = UserByEmail(email);

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
            if (!Session.ActiveUser.Name.Contains("You")) Session.ActiveUser.Name += (" (You)");
            Session.Token = response.Tokens;
        }

        public static async void UploadInterviewSessionsAsync()
        {
            foreach (var s in AllInterviewSessionsNotUploaded())
            {
                var didUpload = await RestClient.Upload(s);
                if (didUpload)
                {
                    s.IsUploaded = didUpload;
                    Session.Connection.Update(s);
                }
            }
        }

        public static List<InterviewSession> AllInterviewSessionsNotUploaded()
        {
            List<InterviewSession> res = Session.Connection.GetAllWithChildren<InterviewSession>((i) => !i.IsUploaded);

            foreach (InterviewSession session in res)
            {
                session.LoadJson();
            }

            return res;
        }

        public static void AddInterviewSession(InterviewSession interviewSession)
        {
            interviewSession.SerializeJson();
            Session.Connection.Insert(interviewSession);
        }

        public static List<User> AllParticipantsUnSelected()
        {
            // TODO: This is inefficient as the database is hit each time this page is visited.
            // Instead, the Selected property should be removed, and SelectedParticipants stored
            // internally between selecting parts<->Recording, and reset from projects<->parts

            var participants = AllParticipants();

            foreach (var p in participants)
            {
                p.Selected = false;

                if (p.Email == Session.ActiveUser.Email)
                {
                    p.Selected = true;
                    Session.Connection.Update(p);
                    p.Name = Session.ActiveUser.Name;
                }
                else
                {
                    Session.Connection.Update(p);
                }
            }

            return participants;
        }
        public static List<User> AllParticipants() => Session.Connection.Table<User>().OrderBy(u => u.Id).ToList();
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

        public static InterviewSession AddSelectedParticipantsToInterviewSession(InterviewSession session)
        {
            if (session.Participants == null) session.Participants = new List<InterviewParticipant>();

            foreach (User participant in SelectedParticipants())
            {
                // TODO: we store the Email/UserID here to simplify logic on the server, which is bad
                // as this is duplicated information between InterviewParticipant, etc
                // We represent a participant as a User, but in doing so do not send the Email/Name
                // when creating a session object (as they are abstracted away), which imho is not ideal.
                session.Participants.Add(new InterviewParticipant
                {
                    InterviewID = session.SessionID,
                    // True if participant was the intervieweer
                    Role = Session.ActiveUser.Id == participant.Id,
                    Name = participant.Name,
                    Email = participant.Email,
                    UserID = participant.Id,
                    Gender = participant.GenderId,
                    Gender_Term = participant.GenderTerm,
                    Age = participant.AgeBracket,
                    Society = participant.Society,
                    IFRC_Role = participant.Role
                });
            }

            return session;
        }

        public static List<InterviewParticipant> ParticipantsForSession(string InterviewSessionID)
        {
            return Session.Connection.Table<InterviewParticipant>().Where((ip) => ip.InterviewID == InterviewSessionID).ToList();
        }

        public static InterviewSession LastInterviewSession()
        {
            InterviewSession last = Session.Connection.Table<InterviewSession>().Last();
            last.LoadJson();
            return last;
        }

        public static List<InterviewPrompt> AnnotationsForLastSession()
        {
            var AnnotationTable = Session.Connection.Table<InterviewPrompt>();
            string lastAnnotatedInterviewId = AnnotationTable.Last().InterviewID;
            return AnnotationTable.Where((a) => a.InterviewID == lastAnnotatedInterviewId).ToList();
        }

        public static void UpdateAnnotation(InterviewPrompt annotation) => Session.Connection.Update(annotation);
    }
}
