using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using Gabber.iOS.Helpers;
using Gabber.iOS.ViewSources;
using GabberPCL;
using GabberPCL.Models;
using GabberPCL.Resources;
using Newtonsoft.Json;
using UIKit;

namespace Gabber.iOS
{
    public partial class ProjectTableViewController : UITableViewController
    {
        private List<Project> projects;
        private UIRefreshControl refreshControl;

        public ProjectTableViewController(IntPtr handle) : base(handle) {}

        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var SupportedLanguages = (await LanguagesManager.GetLanguageChoices()).OrderBy((lang) => lang.Code).ToList();

            TableView.RegisterNibForCellReuse(ProjectTableViewHeader.Nib, ProjectTableViewHeader.CellID);
            TableView.RowHeight = UITableView.AutomaticDimension;
            TableView.EstimatedRowHeight = 250;

            if (Session.ActiveUser == null)
            {
                var email = NSUserDefaults.StandardUserDefaults.StringForKey("username");
                var user = Queries.UserByEmail(email);
                var _tokens = NSUserDefaults.StandardUserDefaults.StringForKey("tokens");
                var tokens = JsonConvert.DeserializeObject<JWToken>(_tokens);
                Queries.SetActiveUser(new DataUserTokens { User = user, Tokens = tokens });
                Firebase.Analytics.Analytics.SetUserID(Session.ActiveUser.Id.ToString());
                // If the user has set the preference or is was determined below, we want to apply it
                Session.ActiveUser.AppLang = user.AppLang;
            }

            var currentMobileLang = Localize.GetCurrentCultureInfo().TwoLetterISOLanguageName;
            var isSupportedLang = SupportedLanguages.FindIndex((lang) => lang.Code == currentMobileLang);

            // If the user has logged in for the first time, then
            // and their mobile language is one we support, then we must choose that.
            if (isSupportedLang != -1 && Session.ActiveUser.AppLang == 0)
            {
                Session.ActiveUser.AppLang = SupportedLanguages.First((lang) => lang.Code == currentMobileLang).Id;
            }
            // Otherwise, the user who logged in, may not have their phone in a lang we do not support
            else if (isSupportedLang == -1 && Session.ActiveUser.AppLang == 0)
            {
                Session.ActiveUser.AppLang = 1;
            }
            Queries.SaveActiveUser();

            var languages = SupportedLanguages.FirstOrDefault((lang) => lang.Id == Session.ActiveUser.AppLang);


            StringResources.Culture = new CultureInfo(languages.Code);

            SetStringResources();

            projects = Queries.AllProjects();

            refreshControl = new UIRefreshControl
            {
                AttributedTitle = new NSAttributedString(StringResources.projects_ui_fetching),
                TintColor = UIColor.FromRGB(.43f, .80f, .79f)
            };

            refreshControl.AddTarget(delegate
            {
                Logger.LOG_EVENT_WITH_ACTION("SWIPE_REFRESH", projects.Count.ToString(), "PROJECT_COUNT");
                var suppress = RefreshData();
            }, UIControlEvent.AllEvents);

            ProjectsTableViewSource source = new ProjectsTableViewSource(projects, LaunchProject, HandleExpansion);

            TableView.Source = source;
            TableView.RefreshControl = refreshControl;
            TableView.ReloadData();

            var suppressAsync = RefreshData();
        }

        void SetStringResources()
        {
            // This is required as when we navigate to here, there blurb is not updated.
            // Have to set this here for now as the default page for TabBar is this and ViewLoad
            // is where the titles are set within each controler of the tabbar.
            TabBarController.TabBar.Items[0].Title = StringResources.common_menu_projects;
            TabBarController.TabBar.Items[1].Title = StringResources.common_menu_gabbers;
            TabBarController.TabBar.Items[2].Title = StringResources.common_menu_settings;

            TabBarController.Title = StringResources.common_menu_projects;
            blurbLabel.Text = StringResources.projects_ui_instructions;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            SetStringResources();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            // By removing the title here we also remove it from the navbar item
            TabBarController.Title = "";
        }

        private async Task RefreshData()
        {
            refreshControl.BeginRefreshing();

            var response = await RestClient.GetProjects(ErrorMessageDialog);

            if (response.Count > 0)
            {
                Queries.AddProjects(response);
                List<Project> loadedProjects = response;

                TableView.Source = new ProjectsTableViewSource(loadedProjects, LaunchProject, HandleExpansion);
                TableView.ReloadData();
            }

            refreshControl.EndRefreshing();
        }

        void ErrorMessageDialog(string message)
        {
            var dialog = new MessageDialog().BuildErrorMessageDialog("ERROR", message);
            PresentViewController(dialog, true, null);
        }

        private void HandleExpansion(int section)
        {
            TableView.ReloadData();
        }

        private void LaunchProject(Project chosenProj)
        {
            Logger.LOG_EVENT_WITH_ACTION("PROJECT_SELECTED", chosenProj.Title, "PROJECT");
            NSUserDefaults.StandardUserDefaults.SetInt(chosenProj.ID, "SelectedProjectID");
            PerformSegue("OpenProjectSegue", this);
        }
    }
}
