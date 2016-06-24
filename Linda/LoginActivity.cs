using Android.App;
using Android.OS;
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

			FindViewById<AppCompatButton>(Resource.Id.login).Click += delegate
			{
				// Redirect as this is not the main activity.
				MainActivity.STATE = FindViewById<AppCompatEditText>(Resource.Id.email).Text;
				if (string.IsNullOrEmpty(MainActivity.STATE)) return;

				// TODO: authentication & form validation.
				StartActivity(typeof(MainActivity));
				// Prevent returning to login once authenticated.
				Finish();
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