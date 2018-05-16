using System;
using UIKit;
using Gabber.iOS.ViewSources;
using Foundation;
using GabberPCL;
using Newtonsoft.Json;
using GabberPCL.Models;
using GabberPCL.Resources;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Gabber.iOS
{
    public partial class ProjectsViewController : UIViewController
    {
		// Prevents multiple calls being made to the API when one is in progress.
        Task LoadingProjects;
        // Make availiable to update
		List<Project> _projects;

        public ProjectsViewController (IntPtr handle) : base (handle) {}

        public override void ViewDidLoad()
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

            var refreshControl = new UIRefreshControl
            {
                AttributedTitle = new NSAttributedString("Fetching projects ..."),
                TintColor = UIColor.FromRGB(.43f, .80f, .79f)
            };

			refreshControl.AddTarget(async delegate {
				await LoadData();
                refreshControl.EndRefreshing();
            }, UIControlEvent.AllEvents);
            
			if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
			{
                ProjectsCollectionView.RefreshControl = refreshControl;	
			}
			else
			{
				ProjectsCollectionView.AddSubview(refreshControl);
			}

            if (Session.ActiveUser == null)
            {
                var email = NSUserDefaults.StandardUserDefaults.StringForKey("username");
                var user = Queries.UserByEmail(email);
                var _tokens = NSUserDefaults.StandardUserDefaults.StringForKey("tokens");
                var tokens = JsonConvert.DeserializeObject<JWToken>(_tokens);
                Queries.SetActiveUser(new DataUserTokens { User = user, Tokens = tokens });
            }
			_projects = Queries.AllProjects();
			ProjectsCollectionView.Source = new ProjectsCollectionViewSource(_projects);
            if (_projects.Count <= 0) LoadDataIfNotLoading(true);
        }

		public async Task LoadData(bool withLoadingBar=false)
        {
			if (withLoadingBar) ProjectsActivityIndicator.StartAnimating();
            var response = await new RestClient().GetProjects(ErrorMessageDialog);
			if (withLoadingBar) ProjectsActivityIndicator.StopAnimating();
         
            if (response.Count > 0)
            {
                Queries.AddProjects(response);
				_projects = response;
				ProjectsCollectionView.Source = new ProjectsCollectionViewSource(_projects);
            }
        }

        public void LoadDataIfNotLoading(bool withLoadingBar = false)
        {
            if (LoadingProjects == null || LoadingProjects.IsCompleted)
            {
                LoadingProjects = LoadData(withLoadingBar);
            }
        }

        public override void ViewWillAppear(bool animated)
        {
			LoadDataIfNotLoading();
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