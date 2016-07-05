using Android.App;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Support.V7.Widget;

namespace Linda
{
	[Activity(MainLauncher = true)]
	public class HomeActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			var username = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext).GetString("username", "");
			// Used to redirect unauthenticated users
			if (string.IsNullOrWhiteSpace(username))
			{
				StartActivity(typeof(LoginActivity));
				Finish();
			}

			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.home);

			// TODO: share button layout with HomeActivity
			FindViewById<AppCompatButton>(Resource.Id.dashboard).Click += delegate
			{
				StartActivity(typeof(MainActivity));
			};


			FindViewById<AppCompatButton>(Resource.Id.capture).Click += delegate
			{
				StartActivity(typeof(PreparationActivity));
			};
		}
	}
}