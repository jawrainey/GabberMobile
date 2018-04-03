using System;
using System.Collections.Generic;
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
            SessionsUpload.SetTitle(StringResources.sessions_ui_submit_button, UIControlState.Normal);
        }

		public override void ViewDidAppear(bool animated)
		{
            UpdateSessionsSource();
            base.ViewDidAppear(animated);
            TabBarController.Title = StringResources.sessions_ui_title;
		}

        async partial void UploadAll(UIButton sender)
        {
            SessionsUpload.Enabled = false;
            var api = new RestClient();

            // Iterating backwards to remove session while iterating
            for (int i = Sessions.Count - 1; i >= 0; i--)
            {
                var session = Sessions[i];
                var item = Foundation.NSIndexPath.FromIndex((uint)Sessions.IndexOf(session));

                // Start spinning indicator
                session.IsUploading = true;
                SessionsCollectionView.ReloadItems(new Foundation.NSIndexPath[]{item});

                var didUpload = await api.Upload(session);

                if (didUpload)
                {
                    session.IsUploaded = true;
                    // Update database state so the session isnt shown on reload etc.
                    Session.Connection.Update(session);
                    Sessions.Remove(session);
                    SessionsCollectionView.ReloadData();
                }
                else 
                {
                    // Stop spining
                    session.IsUploading = false;
                    SessionsCollectionView.ReloadItems(new Foundation.NSIndexPath[] { item });  
                    // Doesnt appear to be a better UI way, darn.
                    PresentViewController(
                        new Helpers.MessageDialog().BuildErrorMessageDialog(
                            StringResources.sessions_ui_message_upload_fail, ""), true, null);
                }
            }

            ShowHideInstructions();
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