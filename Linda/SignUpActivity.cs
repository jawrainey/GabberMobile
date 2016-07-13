using Android.App;
using Android.OS;
using Android.Support.V7.App;

namespace Linda
{
	[Activity(Label = "Register")]
	public class SignUpActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.register);
		}
	}
}