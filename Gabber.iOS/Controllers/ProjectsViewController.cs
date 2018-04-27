using System;
using UIKit;
using Gabber.iOS.ViewSources;
using Foundation;
using GabberPCL;
using Newtonsoft.Json;
using GabberPCL.Models;
using GabberPCL.Resources;

namespace Gabber.iOS
{
    public partial class ProjectsViewController : UIViewController
    {
        public ProjectsViewController (IntPtr handle) : base (handle) {}

        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ProjectsInstructions.Text = StringResources.projects_ui_instructions;
            TabBarController.Title = StringResources.projects_ui_title;

            // Have to set this here for now as the default page for TabBar is this and ViewLoad
            // is where the titles are set within each controler of the tabbar.
            TabBarController.TabBar.Items[0].Title = StringResources.common_menu_projects;
            TabBarController.TabBar.Items[1].Title = StringResources.common_menu_gabbers;
            TabBarController.TabBar.Items[2].Title = StringResources.common_menu_about;

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
            base.ViewDidAppear(animated);
            TabBarController.Title = StringResources.common_menu_projects;
        }

		public override void ViewWillDisappear(bool animated)
		{
            base.ViewWillDisappear(animated);
            // By removing the title here we also remove it from the navbar item
            TabBarController.Title = "";
		}

        // TODO: given this is in all controllers, should make a super class to reduce duplication
        void ErrorMessageDialog(string message)
        {
            var dialog = new Helpers.MessageDialog();
            var errorDialog = dialog.BuildErrorMessageDialog("ERROR", message);
            PresentViewController(errorDialog, true, null);
        }
    }
}