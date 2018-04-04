using Foundation;
using System;
using UIKit;
using GabberPCL;
using Gabber.iOS.ViewSources;
using System.Collections.Generic;
using Gabber.iOS.Helpers;
using System.Threading.Tasks;
using GabberPCL.Models;
using AVFoundation;
using GabberPCL.Resources;

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

        static string GetTextStatus(bool granted)
        {
            // Text is required as there's three states, since there's no permissions < ios 8.
            return string.Format("Access {0}", granted ? "allowed" : "denied");
        }

        public string CheckAccess()
        {
            string micAccessText = "Not determined";

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var permission = AVAudioSession.SharedInstance().RecordPermission;
                if (permission == AVAudioSessionRecordPermission.Undetermined)
                    return micAccessText;

                micAccessText = GetTextStatus(permission == AVAudioSessionRecordPermission.Granted);
            }
            return micAccessText;
        }

        public void RequestAudioRecordPermission()
        {
            AVAudioSession.SharedInstance().RequestRecordPermission(granted => {
                GetTextStatus(granted);
                new TaskCompletionSource<object>().SetResult(null);
            });
        }

        public override void ViewDidLoad()
        {
            Title = StringResources.recording_ui_title;
            TopicsInstructions.Text = StringResources.recording_ui_instructions_header;
            RecordInstructions.Text = StringResources.recording_ui_instructions_footer;

            var es = new CoreGraphics.CGSize(UIScreen.MainScreen.Bounds.Width - 36, 70);
            (TopicsCollectionView.CollectionViewLayout as UICollectionViewFlowLayout).EstimatedItemSize = es;

            RequestAudioRecordPermission();                  
            if (CheckAccess().Contains("denied"))
            {
                ConfigureMicrophoneAccessDialog();
                return;
            }

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("BackButton"), UIBarButtonItemStyle.Plain, (sender, args) => 
            {
                if (AudioRecorder.IsRecording())
                {
                    var doDeleteRecording = UIAlertController.Create(
                        StringResources.recording_ui_dialog_back_title,
                        StringResources.recording_ui_dialog_back_body, 
                        UIAlertControllerStyle.Alert);

                    doDeleteRecording.AddAction(
                        UIAlertAction.Create(
                            StringResources.recording_ui_dialog_back_negative, 
                            UIAlertActionStyle.Cancel, 
                            (_) => { }
                        )
                    );
                    doDeleteRecording.AddAction(
                        UIAlertAction.Create(
                            StringResources.recording_ui_dialog_back_positive,
                            UIAlertActionStyle.Default, (_) => {
                        NavigationController.PopViewController(false);
                    }));
                    PresentViewController(doDeleteRecording, true, null);   
                }
                NavigationController.PopViewController(false);
            });

            // As we can record, enable it all.
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
            if (CheckAccess().Contains("denied"))
            {
                ConfigureMicrophoneAccessDialog();
                return;
            }

            // Has the first topic been selected, i.e. one of the states has changed
            if (Topics.FindAll((p) => p.SelectionState != Prompt.SelectedState.never).Count == 1)
            {
                RecordInstructions.Hidden = true;
                RecordButton.Hidden = false;
                InterviewTimer.Hidden = false;
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
                    Title = StringResources.recording_ui_title_active;
                    InterviewTimer.Text = TimeSpan.FromSeconds(AudioRecorder.CurrentTime()).ToString((@"mm\:ss"));
                });
                await Task.Delay(15);
            }
        }

        protected void ConfigureMicrophoneAccessDialog()
        {
            var finishRecordingAlertController = UIAlertController.Create(
                StringResources.recording_ui_permission_title, 
                StringResources.recording_ui_permission_body, UIAlertControllerStyle.Alert);

            finishRecordingAlertController.AddAction(
                UIAlertAction.Create(
                    StringResources.recording_ui_permission_button_negative, 
                    UIAlertActionStyle.Cancel, 
                    (_) => { }
                )
            );
            finishRecordingAlertController.AddAction(
                UIAlertAction.Create(
                    StringResources.recording_ui_permission_button_positive, 
                    UIAlertActionStyle.Default, 
                    (_) => { UIApplication.SharedApplication.OpenUrl(new NSUrl("app-settings:")); }
                )
            );
            PresentViewController(finishRecordingAlertController, true, null);
        }

        partial void RecordingCompleteDialog(UIButton sender)
        {
            var finishRecordingAlertController = UIAlertController.Create(
                StringResources.recording_ui_dialog_finish_title, "",
                UIAlertControllerStyle.Alert
            );

            finishRecordingAlertController.AddAction(
                UIAlertAction.Create(
                    StringResources.recording_ui_dialog_finish_positive, 
                    UIAlertActionStyle.Default, 
                    FinishRecording)
            );
            finishRecordingAlertController.AddAction(
                UIAlertAction.Create(
                    StringResources.recording_ui_dialog_finish_negative, 
                    UIAlertActionStyle.Cancel, 
                    (_) => {})
            );

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
                CreatedAt = DateTime.Now,
                // This uniquely identifies who created the interview; the account created locally 
                // will have a different ID than the one on the server as they are not in sync
                CreatorEmail = Session.ActiveUser.Email,
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