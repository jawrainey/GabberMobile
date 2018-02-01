using System;
using UIKit;
using Gabber.iOS.ViewSources;
using Foundation;
using GabberPCL;
using Gabber.iOS.Helpers;

namespace Gabber.iOS
{
    public partial class ProjectsViewController : UIViewController
    {
        public ProjectsViewController (IntPtr handle) : base (handle) {}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Used by the PCL for database interactions so must be defined early.
            Session.PrivatePath = new PrivatePath();
            // Register the implementation to the global interface within the PCL.
            RestClient.GlobalIO = new DiskIO();

            // TODO: redirect to Login workflow
            if (!Session.ActiveUser.IsActive) 
            {
                // TODO: REDIRECT!
                // TODO: this should be handled on the login page!!
                if (Queries.AllParticipants().Count <= 0) 
                {
                    Queries.AddUser(Session.ActiveUser);   
                }
            }

            var projects = new RestClient().GetProjects();

            if (projects.Count > 0)
            {
                Queries.AddProjects(projects);
                ProjectsCollectionView.Source = new ProjectsCollectionViewSource(projects);
            }
            else 
            {
                // Use projects from database, and if there are none there, then error msg
                // TODO: There is no Internet access as projects is empty iff an error occurs;
                // TODO: update instructions with error message
                Console.WriteLine("There is no Internet connection");
            }
        }

        [Action("UnwindToProjectsViewController:")]
        public void UnwindToProjectsViewController(UIStoryboardSegue segue) {}
    }
}