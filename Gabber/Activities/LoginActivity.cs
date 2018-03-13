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

            FindViewById<TextInputEditText>(Resource.Id.password).EditorAction += (_, e) => {
                e.Handled = false;
                if (e.ActionId == Android.Views.InputMethods.ImeAction.Done)
                {
                    FindViewById<AppCompatButton>(Resource.Id.submit).PerformClick();
                    e.Handled = true;
                }
            };

			FindViewById<TextView>(Resource.Id.register).Click += delegate
			{
				StartActivity(typeof(RegisterActivity));
			};

			FindViewById<AppCompatButton>(Resource.Id.submit).Click += async delegate
			{
                var imm = (Android.Views.InputMethods.InputMethodManager)GetSystemService(InputMethodService);
                imm.HideSoftInputFromWindow(FindViewById<TextInputEditText>(Resource.Id.password).WindowToken, 0);

				var email = FindViewById<AppCompatEditText>(Resource.Id.email);
				var passw = FindViewById<AppCompatEditText>(Resource.Id.password);

				if (string.IsNullOrWhiteSpace(email.Text))
				{
                    email.RequestFocus();
					Snackbar.Make(email, "An email address is required", Snackbar.LengthLong).Show();
				}
                else if (string.IsNullOrWhiteSpace(passw.Text))
                {
                    passw.RequestFocus();
                    Snackbar.Make(passw, "A password is required", Snackbar.LengthLong).Show();
                }
				else if (!Android.Util.Patterns.EmailAddress.Matcher(email.Text).Matches())
				{
                    email.RequestFocus();
					Snackbar.Make(email, "The email provided is invalid", Snackbar.LengthLong).Show();
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
                            email.RequestFocus();
						});
					}
				}
			};
		}
	}
}