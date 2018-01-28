using Foundation;
using System;
using UIKit;
using GabberPCL;
using Gabber.iOS.ViewSources;
using System.Collections.Generic;
using Gabber.iOS.Helpers;
using System.Threading.Tasks;

namespace Gabber.iOS
{
    public partial class RecordViewController : UIViewController
    {
        AudioRecorder AudioRecorder;
        // The topics associated with the selected project 
        List<Prompt> Topics;
        // Each interview recorded has a unique SID for uniqueness between interviews,
        // and is required to associate annotations with a particular session.
        string InterviewSessionID;

        public RecordViewController(IntPtr handle) : base(handle) {}

        public override void ViewDidLoad()
        {
            AudioRecorder = new AudioRecorder();
            InterviewSessionID = Guid.NewGuid().ToString();
            // TODO: this should use the new singleton and Queries class.
            var model = new DatabaseManager(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            var selectedProject = model.ProjectByName(NSUserDefaults.StandardUserDefaults.StringForKey("selectedProject"));
            // Shared as we can use this to determine when a row is first clicked
            Topics = selectedProject.prompts;
            TopicsCollectionView.Source = new TopicsCollectionViewSource { 
                Rows = Topics,
                AddAnnotation = AddAnnotation
            };
        }

        // TODO: move the SelectionCount and CreateAnnotation logic to the query Class 
        // once Topics are stored in the database correctly (currently they're JSONed) there.
        void AddAnnotation()
        {
            RecordInstructions.Hidden = true;
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
            Queries.CreateAnnotation(InterviewSessionID, AudioRecorder.CurrentTime(), current.prompt);
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

            finishRecordingAlertController.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, delegate {
                GabberPCL.Models.Annotation.ComputeEndForAllAnnotationsInSession(AudioRecorder.CurrentTime());
                var recordingFilePath = AudioRecorder.FinishRecording();
                // TODO: prepare recording for uploading via PCL Rest API
                PerformSegue("UnWindToProjectsVC", this);
            }));
            finishRecordingAlertController.AddAction(UIAlertAction.Create("No", UIAlertActionStyle.Cancel, delegate {}));

            PresentViewController(finishRecordingAlertController, true, null);
        }
    }
}