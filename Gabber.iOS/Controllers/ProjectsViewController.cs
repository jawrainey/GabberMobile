using System;
using UIKit;
using Gabber.iOS.ViewSources;
using Foundation;
using GabberPCL;
using Newtonsoft.Json;
using GabberPCL.Models;

namespace Gabber.iOS
{
    public partial class ProjectsViewController : UIViewController
    {
        public ProjectsViewController (IntPtr handle) : base (handle) {}

        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var es = new CoreGraphics.CGSize(UIScreen.MainScreen.Bounds.Width - 36, 70);
            (ProjectsCollectionView.CollectionViewLayout as UICollectionViewFlowLayout).EstimatedItemSize = es;

            if (Session.ActiveUser == null)
            {
                var email = NSUserDefaults.StandardUserDefaults.StringForKey("username");
                var user = Queries.UserByEmail(email);
                var _tokens = NSUserDefaults.StandardUserDefaults.StringForKey("tokens");
                var tokens = JsonConvert.DeserializeObject<JWToken>(_tokens);
                Queries.SetActiveUser(new DataUserTokens { User = user, Tokens = tokens });
            }

            ProjectsActivityIndicator.StartAnimating();
            var projects = await (new RestClient()).GetProjects(ErrorMessageDialog);
            ProjectsActivityIndicator.StopAnimating();

            if (projects.Count > 0)
            {
                Queries.AddProjects(projects);
            }
            else 
            {
                projects = Queries.AllProjects();
            }
            ProjectsCollectionView.Source = new ProjectsCollectionViewSource(projects);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewDidLoad();
            // TODO: should upload on swipe up...
            // TODO: there's currently no feedback to the user that items are being uploaded
            Queries.UploadInterviewSessionsAsync();
        }

        [Action("UnwindToProjectsViewController:")]
        public void UnwindToProjectsViewController(UIStoryboardSegue segue) {}

        // TODO: given this is in all controllers, should make a super class to reduce duplication
        void ErrorMessageDialog(string message)
        {
            var dialog = new Helpers.MessageDialog();
            var errorDialog = dialog.BuildErrorMessageDialog("ERROR", message);
            PresentViewController(errorDialog, true, null);
        }
    }
}