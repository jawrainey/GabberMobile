using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace Gabber
{
	[Activity]
	public class LoginActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.login);

			FindViewById<TextView>(Resource.Id.register).Click += delegate
			{
				StartActivity(typeof(RegisterActivity));
			};

			FindViewById<AppCompatButton>(Resource.Id.submit).Click += async delegate
			{
				var email = FindViewById<AppCompatEditText>(Resource.Id.email);
				var passw = FindViewById<AppCompatEditText>(Resource.Id.password);

				// TODO: snackbars are used for simplicity. Ideally, specific error messages
				// would be output for each unique error instead of a generic (informative) message.
				if (string.IsNullOrWhiteSpace(email.Text) || string.IsNullOrWhiteSpace(passw.Text))
				{
					Snackbar.Make(email, Resources.GetText(Resource.String.error_usern_pass_req), Snackbar.LengthLong).Show();
				}
				else if (!Android.Util.Patterns.EmailAddress.Matcher(email.Text).Matches())
				{
					Snackbar.Make(email, Resources.GetText(Resource.String.error_invalid_email), Snackbar.LengthLong).Show();
				}
				else
				{
					FindViewById<ProgressBar>(Resource.Id.progressBar).Visibility = ViewStates.Visible;
					FindViewById<AppCompatButton>(Resource.Id.submit).Enabled = false;

                    var api = new GabberPCL.RestClient();
                    var tokens = await api.Login(
                        email.Text, 
                        passw.Text, 
                        (errorMessage) => Snackbar.Make(email, errorMessage, 0).Show()
                    );

					// If the user details are correct: then a token was generated
                    if (!string.IsNullOrEmpty(tokens.Access))
					{
                        var prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
                        prefs.Edit().PutString("username", email.Text).Commit();
                        prefs.Edit().PutString("tokens", JsonConvert.SerializeObject(tokens)).Commit();
						
                        // We do not want the user to return to ANY gabber recording pages once captured.
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
					}
				}
			};
		}
	}
}