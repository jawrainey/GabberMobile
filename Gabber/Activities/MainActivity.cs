using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Firebase;
using Firebase.Analytics;
using Gabber.Fragments;
using Gabber.Helpers;
using GabberPCL;
using GabberPCL.Models;
using GabberPCL.Resources;
using Newtonsoft.Json;

namespace Gabber
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MyTheme")]
    public class MainActivity : AppCompatActivity
    {
        public static FirebaseAnalytics FireBaseAnalytics;
        private BottomNavigationView nav;

        private PrefsFragment prefsFragment;
        private ProjectsFragment projectsFragment;
        private UploadsFragment sessionsFragment;
        private Android.Support.V4.App.Fragment activeFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main);

            var preferences = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            var UserEmail = preferences.GetString("username", "");

            // Create the user once as they can come here after Register/Login or anytime they reopen app
            if (Session.ActiveUser == null)
            {
                var user = Queries.UserByEmail(UserEmail);
                var tokens = JsonConvert.DeserializeObject<JWToken>(preferences.GetString("tokens", ""));
                Queries.SetActiveUser(new DataUserTokens { User = user, Tokens = tokens });
                FireBaseAnalytics.SetUserId(Session.ActiveUser.Id.ToString());
            }

            nav = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            nav.NavigationItemSelected += NavigationItemSelected;
            LoadStrings();


            prefsFragment = new PrefsFragment();
            projectsFragment = new ProjectsFragment();
            sessionsFragment = new UploadsFragment();
            activeFragment = projectsFragment;

            SupportFragmentManager.BeginTransaction().Add(Resource.Id.content_frame, prefsFragment, "settings").Hide(prefsFragment).Commit();
            SupportFragmentManager.BeginTransaction().Add(Resource.Id.content_frame, sessionsFragment, "sessions").Hide(sessionsFragment).Commit();
            SupportFragmentManager.BeginTransaction().Add(Resource.Id.content_frame, projectsFragment, "projects").Commit();

            SupportActionBar.Title = StringResources.projects_ui_title;

            LanguagesManager.RefreshIfNeeded();
        }

        public void LoadStrings()
        {
            nav.Menu.FindItem(Resource.Id.menu_projects).SetTitle(StringResources.common_menu_projects);
            nav.Menu.FindItem(Resource.Id.menu_gabbers).SetTitle(StringResources.common_menu_gabbers);
            nav.Menu.FindItem(Resource.Id.menu_settings).SetTitle(StringResources.common_menu_settings);

            int selectedTabId = nav.SelectedItemId;

            switch (nav.SelectedItemId)
            {
                case Resource.Id.menu_projects:
                    SupportActionBar.Title = StringResources.projects_ui_title;
                    break;
                case Resource.Id.menu_gabbers:
                    SupportActionBar.Title = StringResources.sessions_ui_title;
                    break;
                case Resource.Id.menu_settings:
                    SupportActionBar.Title = StringResources.common_menu_settings;
                    break;
            }
        }

        private void NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            int id = e.Item.ItemId;
            Android.Support.V4.App.Fragment toShow = null;

            switch (id)
            {
                case Resource.Id.menu_projects:
                    SupportActionBar.Title = StringResources.projects_ui_title;
                    toShow = projectsFragment;
                    LOG_FRAGMENT_SELECTED("projects");
                    break;
                case Resource.Id.menu_gabbers:
                    SupportActionBar.Title = StringResources.sessions_ui_title;
                    toShow = sessionsFragment;
                    LOG_FRAGMENT_SELECTED("uploads");
                    break;
                case Resource.Id.menu_settings:
                    SupportActionBar.Title = StringResources.common_menu_settings;
                    toShow = prefsFragment;
                    LOG_FRAGMENT_SELECTED("settings");
                    break;
                default:
                    toShow = projectsFragment;
                    break;
            }

            SupportFragmentManager.BeginTransaction().Hide(activeFragment).Show(toShow).Commit();
            activeFragment = toShow;
        }

        private void LOG_FRAGMENT_SELECTED(string name)
        {
            var bundle = new Bundle();
            bundle.PutString("FRAGMENT", name);
            FireBaseAnalytics.LogEvent("FRAGMENT_SHOWN", bundle);
        }

        public void LogOut()
        {
            // reset all data stored in prefs
            var prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            prefs.Edit().Clear().Commit();

            // Make sure the projects fragment pulls from the server when we log back in 
            ProjectsFragment.HasRefreshedProjects = false;

            // reset to system language
            StringResources.Culture = Localise.GetCurrentCultureInfo();

            // nuke the database
            Session.NukeItFromOrbit();

            //return to login
            GoToOnboarding();
        }

        private void GoToOnboarding()
        {
            // We must clear the navigation stack here otherwise this activity is behind onboarding.
            var intent = new Intent(this, typeof(Activities.Onboarding));
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask);
            StartActivity(intent);
            Finish();
        }
    }
}