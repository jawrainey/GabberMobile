using Android.App;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
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

			FindViewById<AppCompatButton>(Resource.Id.submit).Click += delegate
			{
				var email = FindViewById<AppCompatEditText>(Resource.Id.email);
				var passw = FindViewById<AppCompatEditText>(Resource.Id.password);

				// TODO: snackbars are used for simplicity. Ideally, specific error messages
				// would be output for each unique error instead of a generic (informative) message.
				if (string.IsNullOrWhiteSpace(email.Text) || string.IsNullOrWhiteSpace(passw.Text))
				{
					Snackbar.Make(email, "A username and password are required.", Snackbar.LengthLong).Show();
				}
				else if (!Android.Util.Patterns.EmailAddress.Matcher(email.Text).Matches())
				{
					Snackbar.Make(email, "That email address is invalid.", Snackbar.LengthLong).Show();
				}
				else
				{
					// TODO: check username and password exist on the server:
					// If invalid, output snackbar, else, invoke code below.

					// Use preferences to only show recordings for each specific user.
					// This simplifies database modelling; its unnecessary to store uname/pass.
					var prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
					prefs.Edit().PutString("username", email.Text).Commit();

					// TODO: authentication & form validation.
					StartActivity(typeof(MainActivity));
					// Prevent returning to login once authenticated.
					Finish();	
				}
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