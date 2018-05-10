using Android.App;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Firebase;
using Gabber.Helpers;
using GabberPCL;
using GabberPCL.Models;
using Newtonsoft.Json;

namespace Gabber
{
	[Activity(MainLauncher=true)]
	public class MainActivity : AppCompatActivity
	{
        protected override void OnCreate(Bundle savedInstanceState)
        {
            FirebaseApp.InitializeApp(ApplicationContext);
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
                StartActivity(typeof(Activities.Onboarding));
            }
            else 
            {
                // Create the user once as they can come here after Register/Login
                if (Session.ActiveUser == null)
                {
                    var user = Queries.UserByEmail(UserEmail);
                    var tokens = JsonConvert.DeserializeObject<JWToken>(preferences.GetString("tokens", ""));
                    Queries.SetActiveUser(new DataUserTokens { User = user, Tokens = tokens });
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
                    break;
                case Resource.Id.menu_gabbers:
                    fragment = Fragments.Sessions.NewInstance();
                    break;
                case Resource.Id.menu_about:
                    fragment = Fragments.About.NewInstance();
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
	}
}