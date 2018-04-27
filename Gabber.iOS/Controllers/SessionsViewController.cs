using System;
using System.Collections.Generic;
using System.Linq;
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

		public SessionsViewController (IntPtr handle) : base (handle) {}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            UpdateSessionsSource();
            SessionsInstructions.Text = StringResources.sessions_ui_header_instructions;
            SessionsInstructionsBody.Text = StringResources.sessions_ui_body_instructions;
            Title = StringResources.common_menu_gabbers;

            SessionsUpload.Layer.BorderWidth = 1.0f;
            SessionsUpload.Layer.BorderColor = UIColor.FromRGB(.43f, .80f, .79f).CGColor;
            SessionsUpload.SetTitle(StringResources.sessions_ui_submit_button, UIControlState.Normal);
        }

		public override void ViewDidAppear(bool animated)
		{
            UpdateSessionsSource();
            base.ViewDidAppear(animated);
            TabBarController.Title = StringResources.sessions_ui_title;
		}

        // Index is optional such that the method could be used onSelected(item)
        public async void UploadSessions(int index, bool recursive)
        {
            var sessions = SessionsViewSource.Sessions;
            // Out of bounds validation
            if (sessions.ElementAtOrDefault(index) == null) return;

            // Update attribute so when item is reloaded the indicator will animate.
            sessions[index].IsUploading = true;
            var item = Foundation.NSIndexPath.FromIndex((uint)Sessions.IndexOf(sessions[index]));
            SessionsCollectionView.ReloadItems(new Foundation.NSIndexPath[] { item });
            // TODO: creating a new instance of API for each view, urgh.
            var didUpload = await new RestClient().Upload(sessions[index]);

            if (didUpload)
            {
                sessions[index].IsUploaded = true;
                // Update state so the session isnt shown on reload etc.
                Session.Connection.Update(sessions[index]);
                sessions.Remove(sessions[index]);
                SessionsCollectionView.ReloadData();
                // Try to upload the next session
                if (recursive) UploadSessions(0, true);
            }
            else
            {
                // Stop spining
                sessions[index].IsUploading = false;
                SessionsCollectionView.ReloadItems(new Foundation.NSIndexPath[] { item });
                PresentViewController(
                    new Helpers.MessageDialog().BuildErrorMessageDialog(
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