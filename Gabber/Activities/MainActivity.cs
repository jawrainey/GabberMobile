using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Firebase;
using Firebase.Analytics;
using Gabber.Helpers;
using GabberPCL;
using GabberPCL.Models;
using Newtonsoft.Json;

namespace Gabber
{
	[Activity(MainLauncher=true)]
	public class MainActivity : AppCompatActivity
	{
		FirebaseAnalytics firebaseAnalytics;

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
				// We must clear the navigation stack here otherwise this activity is behind onboarding.
				var intent = new Intent(this, typeof(Activities.Onboarding));
                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask);
                StartActivity(intent);
                Finish();
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

                var nav = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
                nav.NavigationItemSelected += (sender, e) => LoadFragment(e.Item.ItemId);

                // Load projects by default and sessions/about if they came from other activity.
                LoadDefaultFragment(nav);
            }
        }

        void LoadDefaultFragment(BottomNavigationView nav)
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

        void LoadFragment(int id)
        {
            Android.Support.V4.App.Fragment fragment = null;

            switch (id)
            {
                case Resource.Id.menu_projects:
                    fragment = Fragments.Projects.NewInstance();
					LOG_FRAGMENT_SELECTED("projects");
                    break;
                case Resource.Id.menu_gabbers:
                    fragment = Fragments.Sessions.NewInstance();
					LOG_FRAGMENT_SELECTED("recordings");
                    break;
                case Resource.Id.menu_about:
                    fragment = Fragments.About.NewInstance();
					LOG_FRAGMENT_SELECTED("about");
                    break;
                default:
                    fragment = Fragments.Projects.NewInstance();
                    break;
            }

            if (fragment == null) return;

            SupportFragmentManager.BeginTransaction()
               .Replace(Resource.Id.content_frame, fragment)
               .Commit();
        }

		void LOG_FRAGMENT_SELECTED(string name)
        {
            var bundle = new Bundle();
			bundle.PutString("FRAGMENT", name);
			firebaseAnalytics.LogEvent("FRAGMENT_SHOWN", bundle);
        }
	}
}