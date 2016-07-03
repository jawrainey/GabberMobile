using Android.App;
using Android.OS;
using Android.Support.V7.App;

namespace Linda
{
	[Activity(Label = "S5: thanks for participating!")]
	public class CompletionActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.completion);
		}
	}
}