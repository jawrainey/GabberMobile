using Android.App;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
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
					FindViewById<ProgressBar>(Resource.Id.progressBar).Visibility = ViewStates.Visible;
					FindViewById<AppCompatButton>(Resource.Id.submit).Enabled = false;
					// If the user details are correct: take user to their dashboard, otherwise snackbar error.
					new RestAPI().Authenticate(email.Text, passw.Text, AuthCallback);
				}
			};

			FindViewById<TextView>(Resource.Id.signup).Click += delegate
			{
				StartActivity(typeof(SignUpActivity));
			};
		}

		void AuthCallback(System.Tuple<bool, string> response)
		{
			var username = FindViewById<AppCompatEditText>(Resource.Id.email);
			// The user details have been validatd and are correct!
			if (response.Item1)
			{
				// Use preferences to only show recordings for each specific user.
				PreferenceManager.GetDefaultSharedPreferences(
					ApplicationContext).Edit().PutString("username", username.Text).Commit();
				StartActivity(typeof(MainActivity));
				// Prevent returning to login once authenticated.
				Finish();
			}
			else
			{
				RunOnUiThread(() =>
				{
					FindViewById<AppCompatButton>(Resource.Id.submit).Enabled = true;
					FindViewById<ProgressBar>(Resource.Id.progressBar).Visibility = ViewStates.Gone;
				});

				Snackbar.Make(username, response.Item2, 0).Show();
			}
		}

	}
}