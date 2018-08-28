using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Firebase.Analytics;
using GabberPCL;
using GabberPCL.Resources;
using Newtonsoft.Json;

namespace Gabber
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class LoginActivity : AppCompatActivity
    {
        FirebaseAnalytics firebaseAnalytics;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            firebaseAnalytics = FirebaseAnalytics.GetInstance(this);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.login);
            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar));
            SupportActionBar.Title = StringResources.login_ui_title;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var submit = FindViewById<AppCompatButton>(Resource.Id.submit);
            submit.Text = StringResources.login_ui_submit_button;

            // Have to manually change hints on input layout.
            var _email = FindViewById<TextInputLayout>(Resource.Id.emailTextInput);
            _email.Hint = StringResources.common_ui_forms_email_label;
            var _password = FindViewById<TextInputLayout>(Resource.Id.passwordTextInput);
            _password.Hint = StringResources.common_ui_forms_password_label;

            FindViewById<TextInputEditText>(Resource.Id.password).EditorAction += (_, e) =>
            {
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
                    email.Error = StringResources.common_ui_forms_email_validate_empty;
                    email.RequestFocus();
                }
                else if (string.IsNullOrWhiteSpace(passw.Text))
                {
                    passw.Error = StringResources.common_ui_forms_password_validate_empty;
                    passw.RequestFocus();
                }
                else if (!Android.Util.Patterns.EmailAddress.Matcher(email.Text).Matches())
                {
                    email.Error = StringResources.common_ui_forms_email_validate_invalid;
                    email.RequestFocus();
                }
                else
                {
                    FindViewById<ProgressBar>(Resource.Id.progressBar).Visibility = ViewStates.Visible;
                    FindViewById<AppCompatButton>(Resource.Id.submit).Enabled = false;

                    LOG_EVENT_WITH_ACTION("LOGIN", "ATTEMPT");
                    CustomAuthResponse response = await RestClient.Login(email.Text.ToLower(), passw.Text);

                    if (response.Meta.Messages.Count > 0)
                    {
                        LOG_EVENT_WITH_ACTION("LOGIN", "ERROR");
                        RunOnUiThread(() =>
                        {
                            response.Meta.Messages.ForEach(MakeError);
                            FindViewById<AppCompatButton>(Resource.Id.submit).Enabled = true;
                            FindViewById<ProgressBar>(Resource.Id.progressBar).Visibility = ViewStates.Gone;
                        });
                    }
                    // If there are no errors, then tokens exist as the request was a great success.
                    else if (!string.IsNullOrEmpty(response.Data?.Tokens.Access))
                    {
                        LOG_EVENT_WITH_ACTION("LOGIN", "SUCCESS");
                        // When the application is closed, the ActiveUser is reset. The username and tokens
                        // are used to build a new active user.
                        var prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
                        // Used in MainActivity to determine if a user has logged in.
                        prefs.Edit().PutString("username", email.Text).Commit();
                        prefs.Edit().PutString("tokens", JsonConvert.SerializeObject(response.Data.Tokens)).Commit();

                        // Set active user on login/register as the user object is in the response.
                        // This prevents us from storing a user object in local storage.
                        Queries.SetActiveUser(response.Data);
                        FirebaseAnalytics.GetInstance(this).SetUserId(Session.ActiveUser.Id.ToString());

                        // We do not want the user to return to ANY gabber recording pages once captured.
                        var intent = new Intent(this, typeof(MainActivity));
                        intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                        StartActivity(intent);
                        // Prevent returning to login once authenticated.
                        Finish();
                    }
                }
            };

            _email.RequestFocus();
            Window.SetSoftInputMode(SoftInput.StateAlwaysVisible);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            LOG_EVENT_WITH_ACTION("BACK_BUTTON", "PRESSED");
            OnBackPressed();
            return true;
        }

        void MakeError(string errorMessage)
        {
            var email = FindViewById<AppCompatEditText>(Resource.Id.email);
            var message = StringResources.ResourceManager.GetString($"login.api.error.{errorMessage}");
            Snackbar.Make(email, message, Snackbar.LengthLong).Show();
        }

        void LOG_EVENT_WITH_ACTION(string eventName, string action)
        {
            var bundle = new Bundle();
            bundle.PutString("ACTION", action);
            bundle.PutString("TIMESTAMP", System.DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
            firebaseAnalytics.LogEvent(eventName, bundle);
        }
    }
}