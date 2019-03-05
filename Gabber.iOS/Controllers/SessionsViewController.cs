using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using Gabber.iOS.Helpers;
using Gabber.iOS.ViewSources;
using GabberPCL;
using GabberPCL.Models;
using GabberPCL.Resources;
using Plugin.Connectivity;
using UIKit;
using Firebase.Analytics;
using Firebase.InstanceID;

namespace Gabber.iOS
{
    public partial class SessionsViewController : UIViewController
    {
        List<InterviewSession> Sessions;
        SessionsCollectionViewSource SessionsViewSource;
        // Used to prevent multiple sessions being simultaneously uploaded. 
        Task IsUploading;

        public SessionsViewController(IntPtr handle) : base(handle) { }

        [Action("UnwindToSessionsViewController:")]
        public void UnwindToSessionsViewController(UIStoryboardSegue segue) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            UpdateSessionsSource();
            SessionsInstructions.Text = StringResources.sessions_ui_header_instructions;
            SessionsInstructionsBody.Text = StringResources.sessions_ui_body_instructions;
            Title = StringResources.common_menu_gabbers;

            var es = new CoreGraphics.CGSize(UIScreen.MainScreen.Bounds.Width - 36, 70);
            (SessionsCollectionView.CollectionViewLayout as UICollectionViewFlowLayout).EstimatedItemSize = es;

            SessionsUpload.Layer.BorderWidth = 1.0f;
            SessionsUpload.Layer.BorderColor = Application.MainColour;
            SessionsUpload.SetTitle(StringResources.sessions_ui_submit_button, UIControlState.Normal);

        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            UpdateSessionsSource();
            TabBarController.Title = StringResources.sessions_ui_title;
            SessionsInstructions.Text = StringResources.sessions_ui_header_instructions;
            SessionsInstructionsBody.Text = StringResources.sessions_ui_body_instructions;

            NSUserDefaults prefs = NSUserDefaults.StandardUserDefaults;
            if (prefs.BoolForKey("SESSION_RECORDED"))
            {
                prefs.SetBool(false, "SESSION_RECORDED");

                // Get epoch time
                TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                int secondsSinceEpoch = (int)t.TotalSeconds;

                Analytics.SetUserProperty(
                        "lastUploadAdded",
                    secondsSinceEpoch.ToString());

                var currentConnections = CrossConnectivity.Current.ConnectionTypes;
                if (!currentConnections.Contains(Plugin.Connectivity.Abstractions.ConnectionType.WiFi))
                {
                    var alertController = UIAlertController.Create(StringResources.sessions_ui_wifiwarning_title,
                                                                   StringResources.sessions_ui_wifiwarning_message,
                                                                   UIAlertControllerStyle.Alert);
                    alertController.AddAction(UIAlertAction.Create(StringResources.sessions_ui_wifiwarning_cancel,
                                                                   UIAlertActionStyle.Cancel, (obj) => { }));
                    alertController.AddAction(UIAlertAction.Create(StringResources.sessions_ui_wifiwarning_confirm,
                                                                   UIAlertActionStyle.Default, (obj) =>
                                                                   {
                                                                       UploadIfNot(0, true);
                                                                   }));

                    PresentViewController(alertController, true, null);
                }
                else
                {
                    UploadIfNot(0, true);
                }
            }
        }

        void UploadIfNot(int position, bool recursive)
        {
            if (IsUploading == null || IsUploading.IsCompleted) {
                SessionsUpload.Enabled = false;
                SessionsUpload.BackgroundColor = UIColor.LightGray;
                IsUploading = UploadSessions(position, recursive);
            }
        }

        // Index is optional such that the method could be used onSelected(item)
        public async Task UploadSessions(int index, bool recursive)
        {
            var sessions = SessionsViewSource.Sessions;
            // Out of bounds validation
            if (sessions.ElementAtOrDefault(index) == null) return;

            SessionsViewSource.SessionIsUploading(index);
            SessionsCollectionView.ReloadData();
            Logger.LOG_EVENT_WITH_ACTION("UPLOAD_SESSION", "ATTEMPT");

            var didUpload = await RestClient.Upload(sessions[index]);
            if (didUpload)
            {
                Logger.LOG_EVENT_WITH_ACTION("UPLOAD_SESSION", "SUCCESS");
                SessionsViewSource.SessionIsUploaded(index);
                SessionsCollectionView.ReloadData();

                TrackWaitingUploads();

                Session.ActiveUser.NumUploaded++;
                Queries.SaveActiveUser();

                Analytics.SetUserProperty("numUploaded", Session.ActiveUser.NumUploaded.ToString());

                var prefs = NSUserDefaults.StandardUserDefaults;
                if (!prefs.BoolForKey("SHOWN_FIRSTUPLOAD"))
                {
                    // Show pop-up explaining viewing uploaded data and changing consent
                    PerformSegue("FirstUploadModal", this);
                }
                else
                {
                    PresentViewController(new MessageDialog().BuildErrorMessageDialog(
                    StringResources.sessions_ui_message_upload_success, ""), true, null);
                    // Try to upload the next session
                    if (recursive) UploadSessions(0, true);
                }
            }
            else
            {
                Logger.LOG_EVENT_WITH_ACTION("UPLOAD_SESSION", "ERROR");
                SessionsViewSource.SessionUploadFail(index);
                SessionsCollectionView.ReloadData();

                PresentViewController(
                    new MessageDialog().BuildErrorMessageDialog(
                        StringResources.sessions_ui_message_upload_fail, ""), true, null);
            }
            // TODO: this should be done on UI; modify enabled color, obviously.
            SessionsUpload.Enabled = true;
            SessionsUpload.BackgroundColor = UIColor.White;
            if (Sessions.Count <= 0) ShowHideInstructions();
        }

        partial void UploadAll(UIButton sender) => UploadIfNot(0, true);

        void UpdateSessionsSource()
        {
            Sessions = Queries.AllNotUploadedInterviewSessionsForActiveUser();
            // Must set this locally to access RemoveSession ...
            SessionsViewSource = new SessionsCollectionViewSource(Sessions);
            SessionsViewSource.SelectSession += (int s) => UploadIfNot(s, false);
            SessionsCollectionView.Source = SessionsViewSource;
            ShowHideInstructions();
            TrackWaitingUploads();
        }

        private void ShowHideInstructions()
        {
            if (Sessions.Count > 0)
            {
                SessionsCollectionView.Hidden = false;
                SessionsUpload.Hidden = false;
                SessionsInstructionsBody.Hidden = true;
            }
            else
            {
                SessionsCollectionView.Hidden = true;
                SessionsUpload.Hidden = true;
                SessionsInstructionsBody.Hidden = false;
            }
        }

        private void TrackWaitingUploads()
        {
            Analytics.SetUserProperty("uploadQueueCount",
                SessionsViewSource.Sessions?.Count().ToString());
        }
    }
}