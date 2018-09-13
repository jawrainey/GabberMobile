using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Gabber.iOS.Helpers;
using Gabber.iOS.ViewSources;
using GabberPCL;
using GabberPCL.Models;
using GabberPCL.Resources;
using UIKit;

namespace Gabber.iOS
{
    public partial class SessionsViewController : UIViewController
    {
        List<InterviewSession> Sessions;

        SessionsCollectionViewSource SessionsViewSource;

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
            UpdateSessionsSource();
            base.ViewDidAppear(animated);
            TabBarController.Title = StringResources.sessions_ui_title;
            SessionsInstructions.Text = StringResources.sessions_ui_header_instructions;
            SessionsInstructionsBody.Text = StringResources.sessions_ui_body_instructions;
        }

        // Index is optional such that the method could be used onSelected(item)
        public async void UploadSessions(int index, bool recursive)
        {
            var sessions = SessionsViewSource.Sessions;
            // Out of bounds validation
            if (sessions.ElementAtOrDefault(index) == null) return;

            // Update attribute so when item is reloaded the indicator will animate.
            sessions[index].IsUploading = true;
            var item = NSIndexPath.FromIndex((uint)Sessions.IndexOf(sessions[index]));
            SessionsCollectionView.ReloadItems(new NSIndexPath[] { item });
            Logger.LOG_EVENT_WITH_ACTION("UPLOAD_SESSION", "ATTEMPT");

            var didUpload = await RestClient.Upload(sessions[index]);

            if (didUpload)
            {
                Logger.LOG_EVENT_WITH_ACTION("UPLOAD_SESSION", "SUCCESS");
                sessions[index].IsUploaded = true;
                // Update state so the session isnt shown on reload etc.
                Session.Connection.Update(sessions[index]);
                sessions.Remove(sessions[index]);
                SessionsCollectionView.ReloadData();

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
                // Stop spining
                sessions[index].IsUploading = false;
                SessionsCollectionView.ReloadItems(new NSIndexPath[] { item });
                PresentViewController(
                    new MessageDialog().BuildErrorMessageDialog(
                        StringResources.sessions_ui_message_upload_fail, ""), true, null);
            }
            if (Sessions.Count <= 0) ShowHideInstructions();
        }

        partial void UploadAll(UIButton sender)
        {
            SessionsUpload.Enabled = false;
            UploadSessions(0, true);
            SessionsUpload.Enabled = true;
        }

        void UpdateSessionsSource()
        {
            Sessions = Queries.AllNotUploadedInterviewSessionsForActiveUser();
            // Must set this locally to access RemoveSession ...
            SessionsViewSource = new SessionsCollectionViewSource(Sessions);
            SessionsViewSource.SelectSession += (int s) => UploadSessions(s, false);
            SessionsCollectionView.Source = SessionsViewSource;
            ShowHideInstructions();
        }

        void ShowHideInstructions()
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
    }
}