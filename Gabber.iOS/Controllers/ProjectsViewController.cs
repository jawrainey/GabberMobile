using System;
using UIKit;
using System.Collections.Generic;
using Gabber.iOS.ViewSources;
using Foundation;
using GabberPCL;
using Newtonsoft.Json;
using Gabber.iOS.Helpers;
using System.Linq;

namespace Gabber.iOS
{
    public partial class ProjectsViewController : UIViewController
    {
        public ProjectsViewController (IntPtr handle) : base (handle) {}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Register the implementation to the global interface within the PCL.
            Session.PrivatePath = new PrivatePath();

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