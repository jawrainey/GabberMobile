using System;
using UIKit;
using System.Collections.Generic;
using Gabber.iOS.ViewSources;
using Foundation;

namespace Gabber.iOS
{
    public partial class ProjectsViewController : UIViewController
    {
        // TODO: should be Project from PCL
        List<string> projects;

        public ProjectsViewController (IntPtr handle) : base (handle) 
        {
            projects = new List<string> { "Project one", "Project two" };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // TODO: populate list with data from API
            ProjectsCollectionView.Source = new ProjectsCollectionViewSource(projects);
        }

        [Action("UnwindToProjectsViewController:")]
        public void UnwindToProjectsViewController(UIStoryboardSegue segue) {}
    }
}