using System;
using UIKit;
using System.Collections.Generic;
using Gabber.iOS.ViewSources;
using Foundation;
using GabberPCL;
using Newtonsoft.Json;

namespace Gabber.iOS
{
    public partial class ProjectsViewController : UIViewController
    {
        public ProjectsViewController (IntPtr handle) : base (handle) {}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // TODO: IF the user IS NOT authenticated THEN show the login workflow

            var model = new DatabaseManager(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            var projects = new RestClient().GetProjects();

			// TODO: There is no Internet access as projects is empty iff an error occurs;
            if (projects.Count == 0)
            {
                // TODO: update instructions with error message
            }
            else 
            {
                model.SaveRequest(JsonConvert.SerializeObject(projects));
                ProjectsCollectionView.Source = new ProjectsCollectionViewSource(projects);
            }
        }

        [Action("UnwindToProjectsViewController:")]
        public void UnwindToProjectsViewController(UIStoryboardSegue segue) {}
    }
}