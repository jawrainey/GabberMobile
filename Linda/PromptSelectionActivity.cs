using Android.App;
using Android.OS;
using Android.Support.V7.App;

namespace Linda
{
	[Activity(Label = "S3: what do they want to discuss?")]
	public class PromptSelectionActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.promptselection);
		}
	}
}