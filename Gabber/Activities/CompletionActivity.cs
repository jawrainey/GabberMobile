using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;

namespace Gabber
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
				// TODO: there is absolutely a better way to do this...
				var intent = new Intent(this, typeof(MainActivity));
				intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
				StartActivity(intent);
				Finish();
			};

			FindViewById<AppCompatButton>(Resource.Id.capture).Click += delegate
			{
				StartActivity(typeof(PreparationActivity));
				Finish();
			};
		}
	}
}