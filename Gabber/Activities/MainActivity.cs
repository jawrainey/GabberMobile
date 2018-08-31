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
    [Activity(MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MyTheme")]
    public class MainActivity : AppCompatActivity
    {
        private FirebaseAnalytics firebaseAnalytics;
        private BottomNavigationView nav;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            FirebaseApp.InitializeApp(ApplicationContext);
            firebaseAnalytics = FirebaseAnalytics.GetInstance(this);

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main);

            // Used by the PCL for database interactions so must be defined early.
            Session.PrivatePath = new PrivatePath();
            // Register the implementation to the global interface within the PCL.
            RestClient.GlobalIO = new DiskIO();

            var preferences = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            var UserEmail = preferences.GetString("username", "");

            if (string.IsNullOrWhiteSpace(UserEmail))
            {
                GoToOnboarding();
            }
            else
            {
                // Create the user once as they can come here after Register/Login or anytime they reopen app
                if (Session.ActiveUser == null)
                {
                    var user = Queries.UserByEmail(UserEmail);
                    var tokens = JsonConvert.DeserializeObject<JWToken>(preferences.GetString("tokens", ""));
                    Queries.SetActiveUser(new DataUserTokens { User = user, Tokens = tokens });
                    firebaseAnalytics.SetUserId(Session.ActiveUser.Id.ToString());
                }

                nav = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
                nav.NavigationItemSelected += (sender, e) => LoadFragment(e.Item.ItemId);
                LoadStrings();

                // Load projects by default and sessions/about if they came from other activity.
                LoadDefaultFragment();
            }

            LanguagesManager.RefreshIfNeeded();
        }

        public void LoadStrings()
        {
            nav.Menu.FindItem(Resource.Id.menu_projects).SetTitle(StringResources.common_menu_projects);
            nav.Menu.FindItem(Resource.Id.menu_gabbers).SetTitle(StringResources.common_menu_gabbers);
            nav.Menu.FindItem(Resource.Id.menu_about).SetTitle(StringResources.common_menu_settings);

            int selectedTabId = nav.SelectedItemId;

            if (selectedTabId == Resource.Id.menu_about)
            {
                SupportActionBar.Title = StringResources.common_menu_settings;
            }
        }

        private void LoadDefaultFragment()
        {
            var fragmentToShow = Intent.GetStringExtra("FRAGMENT_TO_SHOW");
            fragmentToShow = !string.IsNullOrWhiteSpace(fragmentToShow) ? fragmentToShow : "";

            switch (fragmentToShow)
            {
                case "gabbers":
                    LoadFragment(Resource.Id.menu_gabbers);
                    nav.Menu.FindItem(Resource.Id.menu_gabbers).SetChecked(true);
                    break;
                default:
                    LoadFragment(Resource.Id.menu_projects);
                    nav.Menu.FindItem(Resource.Id.menu_projects).SetChecked(true);
                    break;
            }
        }

        private void LoadFragment(int id)
        {
            Android.Support.V4.App.Fragment fragment = null;

            switch (id)
            {
                case Resource.Id.menu_projects:
                    fragment = Fragments.ProjectsFragment.NewInstance();
                    LOG_FRAGMENT_SELECTED("projects");
                    break;
                case Resource.Id.menu_gabbers:
                    fragment = Fragments.SessionsFragment.NewInstance();
                    LOG_FRAGMENT_SELECTED("recordings");
                    break;
                case Resource.Id.menu_about:
                    SupportActionBar.Title = StringResources.common_menu_settings;
                    fragment = Fragments.PrefsFragment.NewInstance();
                    LOG_FRAGMENT_SELECTED("about");
                    break;
                default:
                    fragment = Fragments.ProjectsFragment.NewInstance();
                    break;
            }

            if (fragment == null) return;

            SupportFragmentManager.BeginTransaction()
               .Replace(Resource.Id.content_frame, fragment)
               .Commit();
        }

        private void LOG_FRAGMENT_SELECTED(string name)
        {
            var bundle = new Bundle();
            bundle.PutString("FRAGMENT", name);
            firebaseAnalytics.LogEvent("FRAGMENT_SHOWN", bundle);
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