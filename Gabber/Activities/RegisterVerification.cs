using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Widget;
using Firebase.Analytics;
using GabberPCL.Resources;

namespace Gabber.Activities
{
	[Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class RegisterVerification : AppCompatActivity
    {
		FirebaseAnalytics firebaseAnalytics;

        protected override void OnCreate(Bundle savedInstanceState)
        {
			firebaseAnalytics = FirebaseAnalytics.GetInstance(this);

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.register_verification);
            FindViewById<TextView>(Resource.Id.verifyTitle).Text = StringResources.register_verify_ui_page_title;
            var email = Intent.GetStringExtra("EMAIL_USED_TO_REGISTER");
            FindViewById<TextView>(Resource.Id.verifyContent).Text = string.Format(StringResources.register_verify_ui_page_content, email);

            var intent = new Intent(Intent.ActionMain);
            intent.SetFlags(ActivityFlags.NewTask);
            intent.AddCategory(Intent.CategoryAppEmail);

            if (intent.ResolveActivity(PackageManager) != null)
            {
                var openEmail = FindViewById<AppCompatButton>(Resource.Id.openEmail);
                openEmail.Text = StringResources.register_verify_ui_button_openemail;
				openEmail.Click += (s, e) =>
				{
					LOG_EVENT_WITH_ACTION("EMAIL_CLIENT", "CLICK_OPENED");
					StartActivity(intent);
				};
                openEmail.Visibility = Android.Views.ViewStates.Visible;
            }
			else
			{
				LOG_EVENT_WITH_ACTION("EMAIL_CLIENT", "NOT_SHOWN");
			}

			FindViewById<TextView>(Resource.Id.loginContent).Text = StringResources.register_verify_ui_page_subcontent;
			var login = FindViewById<AppCompatButton>(Resource.Id.loginButton);
			login.Text = StringResources.login_ui_submit_button;

			login.Click += (s, e) =>
			{
				LOG_EVENT_WITH_ACTION("LOGIN_BUTTON", "CLICKED");
				StartActivity(typeof(LoginActivity));
			};
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