using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Firebase.Analytics;
using GabberPCL;
using GabberPCL.Resources;

namespace Gabber
{
	[Activity(ScreenOrientation = ScreenOrientation.Portrait)]
	public class RegisterActivity : AppCompatActivity
	{
		FirebaseAnalytics firebaseAnalytics;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			firebaseAnalytics = FirebaseAnalytics.GetInstance(this);
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.register);
            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar));
            SupportActionBar.Title = StringResources.register_ui_title;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var submit = FindViewById<AppCompatButton>(Resource.Id.submit);
            submit.Text = StringResources.register_ui_submit_button;

            var _name = FindViewById<TextInputLayout>(Resource.Id.nameLayout);
            _name.Hint = StringResources.register_ui_fullname_label;
            var _email = FindViewById<TextInputLayout>(Resource.Id.emailLayout);
            _email.Hint = StringResources.common_ui_forms_email_label;
            var _password = FindViewById<TextInputLayout>(Resource.Id.passwordLayout);
            _password.Hint = StringResources.common_ui_forms_password_label;

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

				var fname = FindViewById<AppCompatEditText>(Resource.Id.name);
				var email = FindViewById<AppCompatEditText>(Resource.Id.email);
				var passw = FindViewById<AppCompatEditText>(Resource.Id.password);

                if (string.IsNullOrWhiteSpace(fname.Text))
				{
                    fname.Error = StringResources.register_ui_fullname_validate_empty;
                    fname.RequestFocus();
				}
                else if (string.IsNullOrWhiteSpace(email.Text))
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

                    var api = new RestClient();
					LOG_EVENT_WITH_ACTION("REGISTER", "ATTEMPT");
                    var response = await api.Register(fname.Text, email.Text.ToLower(), passw.Text);

                    if (response.Meta.Success)
					{
                        LOG_EVENT_WITH_ACTION("REGISTER", "SUCCESS");
                        var intent = new Intent(this, typeof(Activities.RegisterVerification));
                        intent.PutExtra("EMAIL_USED_TO_REGISTER", email.Text);
						intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
						StartActivity(intent);
						Finish();
					}
					else
					{
						RunOnUiThread(() =>
						{
                            if (response.Meta.Messages.Count > 0)
                            {
								LOG_EVENT_WITH_ACTION("REGISTER", "ERROR");
                                response.Meta.Messages.ForEach(MakeError);
                            }
							FindViewById<AppCompatButton>(Resource.Id.submit).Enabled = true;
							FindViewById<ProgressBar>(Resource.Id.progressBar).Visibility = ViewStates.Gone;
                            fname.RequestFocus();
						});
					}
				}
			};

            _name.RequestFocus();
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
            // Using login string lookup as there are no different error messages between login/register, only general.
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