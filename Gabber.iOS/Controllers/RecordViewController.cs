using Foundation;
using System;
using UIKit;
using GabberPCL;
using Gabber.iOS.ViewSources;
using System.Collections.Generic;
using Gabber.iOS.Helpers;
using System.Threading.Tasks;
using GabberPCL.Models;

namespace Gabber.iOS
{
    public partial class RecordViewController : UIViewController
    {
        AudioRecorder AudioRecorder;
        // The topics associated with the selected project 
        List<Prompt> Topics;
        // Each interview recorded has a unique SID (GUID) to associate annotations with a session.
        string InterviewSessionID;
        // Which project are we recording an interview for?
        int SelectedProjectID;

        public RecordViewController(IntPtr handle) : base(handle) {}

        public override void ViewDidLoad()
        {
            AudioRecorder = new AudioRecorder();
            InterviewSessionID = Guid.NewGuid().ToString();

            SelectedProjectID = Convert.ToInt32(NSUserDefaults.StandardUserDefaults.IntForKey("SelectedProjectID"));
            var SelectedProject = Queries.ProjectById(SelectedProjectID);

            // Shared as we can use this to determine when a row is first clicked
            Topics = SelectedProject.Prompts;
            TopicsCollectionView.Source = new TopicsCollectionViewSource { 
                Rows = Topics,
                AddAnnotation = AddAnnotation
            };
        }

        void AddAnnotation()
        {
            RecordInstructions.Hidden = true;
            RecordNote.Hidden = true;
            RecordButton.Hidden = false;
            AudioRecorder.Record();

            // Has the first topic been selected, i.e. one of the states has changed
            if (Topics.FindAll((p) => p.SelectionState != Prompt.SelectedState.never).Count == 1)
            {
                RecordInstructions.Hidden = true;
                RecordButton.Hidden = false;
                AudioRecorder.Record();
                UpdateTimeLabelAsync();
            }
            var current = Topics.Find((p) => p.SelectionState == Prompt.SelectedState.current);
            Queries.CreateAnnotation(AudioRecorder.CurrentTime(), InterviewSessionID, current.ID);
        }

        async void UpdateTimeLabelAsync()
        {
            while (AudioRecorder.IsRecording())
            {
                InvokeOnMainThread(() =>
                {
                    InterviewTimer.Text = TimeSpan.FromSeconds(AudioRecorder.CurrentTime()).ToString((@"mm\:ss"));
                });
                await Task.Delay(15);
            }
        }

        partial void RecordingCompleteDialog(UIButton sender)
        {
            var finishRecordingAlertController = UIAlertController.Create(
                "End recording", 
                "Are you sure you want to end this recording?", 
                UIAlertControllerStyle.Alert
            );

            finishRecordingAlertController.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, FinishRecording));
            finishRecordingAlertController.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Cancel, (_) => {}));

            PresentViewController(finishRecordingAlertController, true, null);
        }

        void FinishRecording(UIAlertAction _)
        {
            // Only once a recording is complete can End for each annotation be computed
            InterviewPrompt.ComputeEndForAllAnnotationsInSession(AudioRecorder.CurrentTime());

            // Added before to simplify accessing the participants involved next.
            Queries.AddSelectedParticipantsToInterviewSession(InterviewSessionID);

            var InterviewSession = new InterviewSession
            {
                SessionID = InterviewSessionID,
                RecordingURL = AudioRecorder.FinishRecording(),

                CreatorID = Session.ActiveUser.Id,
                ProjectID = SelectedProjectID,

                Prompts = Queries.AnnotationsForLastSession(),
                Participants = Queries.ParticipantsForSession(InterviewSessionID),

                IsUploaded = false
            };

            Queries.AddInterviewSession(InterviewSession);

            // The ProjectsController manages uploading sessions
            PerformSegue("UnWindToProjectsVC", this);
        }
    }
}