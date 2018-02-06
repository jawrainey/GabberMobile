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

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // TODO: Of course not the best solution as the refresh token is stored here too
            var tokens = NSUserDefaults.StandardUserDefaults.StringForKey("ActiveUserTokens");
            Session.Token = JsonConvert.DeserializeObject<JWToken>(tokens);

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