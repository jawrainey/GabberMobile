using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Widget;

namespace Linda
{
	[Activity]
	public class LoginActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.login);

			FindViewById<AppCompatButton>(Resource.Id.login).Click += delegate
			{
				var email = FindViewById<AppCompatEditText>(Resource.Id.email);

				// Use preferences to only show recordings for each specific user.
				// This simplifies database modelling; its unnecessary to store uname/pass.
				var prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
				prefs.Edit().PutString("username", email.Text).Commit(); 

				// TODO: authentication & form validation.
				StartActivity(typeof(MainActivity));
				// Prevent returning to login once authenticated.
				Finish();
			};

			FindViewById<TextView>(Resource.Id.signup).Click += delegate
			{
				StartActivity(typeof(SignUpActivity));
			};

			FindViewById<TextView>(Resource.Id.forgot_password).Click += delegate
			{
				StartActivity(typeof(ForgotPasswordActivity));
			};
		}
	}
}