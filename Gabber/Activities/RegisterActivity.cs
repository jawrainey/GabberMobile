using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GabberPCL;

namespace Gabber
{
	[Activity(Label = "Register")]
	public class RegisterActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.register);


			FindViewById<AppCompatButton>(Resource.Id.submit).Click += async delegate
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

					if (await new RestClient().Authenticate(email.Text, passw.Text))
					{
						PreferenceManager.GetDefaultSharedPreferences(
							ApplicationContext).Edit().PutString("username", email.Text).Commit();
						
						var intent = new Intent(this, typeof(MainActivity));
						intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
						StartActivity(intent);
						Finish();
					}
					else
					{
						RunOnUiThread(() =>
						{
							FindViewById<AppCompatButton>(Resource.Id.submit).Enabled = true;
							FindViewById<ProgressBar>(Resource.Id.progressBar).Visibility = ViewStates.Gone;
						});

						Snackbar.Make(email, "An error... oh my", 0).Show();
					}
				}
			};
		}
	}
}