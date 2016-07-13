using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;

namespace Linda
{
	[Activity(Label = "What happens next?")]
	public class CompletionActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.completion);

			FindViewById<AppCompatButton>(Resource.Id.dashboard).Click += delegate
			{
				StartActivity(typeof(MainActivity));
			};

			FindViewById<AppCompatButton>(Resource.Id.capture).Click += delegate
			{
				StartActivity(typeof(PreparationActivity));
			};
		}
	}
}