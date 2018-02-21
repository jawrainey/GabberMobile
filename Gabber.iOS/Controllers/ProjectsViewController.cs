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

            if (Session.ActiveUser == null)
            {
                // TODO: although we have Session.ActiveUser, it's overkill at the moment
                var email = NSUserDefaults.StandardUserDefaults.StringForKey("Username");
                Session.ActiveUser = Queries.FindOrInsertUser(email);
                Session.ActiveUser.IsActive = true;
                // TODO: should only store refresh token and keep access token in memory
                var tokens = NSUserDefaults.StandardUserDefaults.StringForKey("ActiveUserTokens");
                Session.Token = JsonConvert.DeserializeObject<JWToken>(tokens);
            }

            var projects = await (new RestClient()).GetProjects();

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

        public override void ViewWillAppear(bool animated)
        {
            base.ViewDidLoad();
            // TODO: should upload on swipe up...
            // TODO: there's currently no feedback to the user that items are being uploaded
            Queries.UploadInterviewSessionsAsync();
        }

        [Action("UnwindToProjectsViewController:")]
        public void UnwindToProjectsViewController(UIStoryboardSegue segue) {}
    }
}