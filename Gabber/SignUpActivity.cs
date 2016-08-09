using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Gabber
{
	[Activity(Label = "Register")]
	public class SignUpActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.register);


			FindViewById<AppCompatButton>(Resource.Id.submit).Click += delegate
			{
				var fname = FindViewById<AppCompatEditText>(Resource.Id.name);
				var email = FindViewById<AppCompatEditText>(Resource.Id.email);
				var passw = FindViewById<AppCompatEditText>(Resource.Id.password);

				// TODO: snackbars are used for simplicity. Ideally, specific error messages
				// would be output for each unique error instead of a generic (informative) message.
				if (string.IsNullOrWhiteSpace(fname.Text) || 
				    string.IsNullOrWhiteSpace(email.Text) || 
				    string.IsNullOrWhiteSpace(passw.Text))
				{
					Snackbar.Make(email, "All details are required.", Snackbar.LengthLong).Show();
				}
				else if (!Android.Util.Patterns.EmailAddress.Matcher(email.Text).Matches())
				{
					Snackbar.Make(email, "That email address is invalid.", Snackbar.LengthLong).Show();
				}
				else
				{
					FindViewById<ProgressBar>(Resource.Id.progressBar).Visibility = ViewStates.Visible;
					FindViewById<AppCompatButton>(Resource.Id.submit).Enabled = false;

					new RestAPI().Create(fname.Text, email.Text, passw.Text, RegisterCallback);
				}
			};
		}

		void RegisterCallback(System.Tuple<bool, string> response)
		{
			var username = FindViewById<AppCompatEditText>(Resource.Id.email);
			// The user details have been validatd and are correct!
			if (response.Item1)
			{
				// Use preferences to only show recordings for each specific user.
				PreferenceManager.GetDefaultSharedPreferences(
					ApplicationContext).Edit().PutString("username", username.Text).Commit();
				// We do not want the user to return to ANY gabber recording pages once captured.
				// TODO this code (heck, the majority of the method) is the same as LoginActivity...
				var intent = new Intent(this, typeof(MainActivity));
				intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
				StartActivity(intent);
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