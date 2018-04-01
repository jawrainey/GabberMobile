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
using GabberPCL.Resources;
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

            var submit = FindViewById<AppCompatButton>(Resource.Id.submit);
            submit.Text = StringResources.login_ui_submit_button;

            // Have to manually change hints on input layout.
            var _password = FindViewById<TextInputLayout>(Resource.Id.passwordTextInput);
            _password.Hint = StringResources.login_ui_password_label;
            var _email = FindViewById<TextInputLayout>(Resource.Id.emailTextInput);
            _email.Hint = StringResources.login_ui_email_label;
                            
            FindViewById<TextInputEditText>(Resource.Id.password).EditorAction += (_, e) => {
                e.Handled = false;
                if (e.ActionId == Android.Views.InputMethods.ImeAction.Done)
                {
                    FindViewById<AppCompatButton>(Resource.Id.submit).PerformClick();
                    e.Handled = true;
                }
            };

			submit.Click += async delegate
			{
                var imm = (Android.Views.InputMethods.InputMethodManager)GetSystemService(InputMethodService);
                imm.HideSoftInputFromWindow(FindViewById<TextInputEditText>(Resource.Id.password).WindowToken, 0);

				var email = FindViewById<AppCompatEditText>(Resource.Id.email);
				var passw = FindViewById<AppCompatEditText>(Resource.Id.password);

				if (string.IsNullOrWhiteSpace(email.Text))
				{
                    email.RequestFocus();
                    Snackbar.Make(email, StringResources.login_ui_error_email_empty, Snackbar.LengthLong).Show();
				}
                else if (string.IsNullOrWhiteSpace(passw.Text))
                {
                    passw.RequestFocus();
                    Snackbar.Make(passw, StringResources.login_ui_error_email_password, Snackbar.LengthLong).Show();
                }
				else if (!Android.Util.Patterns.EmailAddress.Matcher(email.Text).Matches())
				{
                    email.RequestFocus();
                    Snackbar.Make(email, StringResources.login_ui_error_email_invalid, Snackbar.LengthLong).Show();
				}
				else
				{
					FindViewById<ProgressBar>(Resource.Id.progressBar).Visibility = ViewStates.Visible;
					FindViewById<AppCompatButton>(Resource.Id.submit).Enabled = false;

                    var response = await new RestClient().Login(email.Text.ToLower(), passw.Text);

                    if (response.Meta.Messages.Count > 0)
                    {
                        RunOnUiThread(() =>
                        {
                            response.Meta.Messages.ForEach(
                                (err) => 
                                Snackbar.Make(
                                    email, 
                                    StringResources.ResourceManager.GetString($"login.api.error.{err}"), 
                                    Snackbar.LengthLong).Show()
                            );
                            FindViewById<AppCompatButton>(Resource.Id.submit).Enabled = true;
                            FindViewById<ProgressBar>(Resource.Id.progressBar).Visibility = ViewStates.Gone;
                        });
                    }
					// If there are no errors, then tokens exist as the request was a great success.
                    else if (!string.IsNullOrEmpty(response.Data?.Tokens.Access))
					{
                        // When the application is closed, the ActiveUser is reset. The username and tokens
                        // are used to build a new active user.
                        var prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
                        // Used in MainActivity to determine if a user has logged in.
                        prefs.Edit().PutString("username", email.Text).Commit();
                        prefs.Edit().PutString("tokens", JsonConvert.SerializeObject(response.Data.Tokens)).Commit();

                        // Set active user on login/register as the user object is in the response.
                        // This prevents us from storing a user object in local storage.
                        Queries.SetActiveUser(response.Data);

                        // We do not want the user to return to ANY gabber recording pages once captured.
						var intent = new Intent(this, typeof(MainActivity));
						intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
						StartActivity(intent);
						// Prevent returning to login once authenticated.
						Finish();
					}
				}
			};
		}
	}
}