using Android.App;
using Android.OS;
using Android.Support.V7.App;

namespace Linda
{
	[Activity(Label = "Change password")]
	public class ForgotPasswordActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.forgotpassword);
		}
	}
}