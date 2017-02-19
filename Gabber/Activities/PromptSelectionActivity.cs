using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Widget;
using FFImageLoading.Views;
using GabberPCL;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Gabber
{
	[Activity(Label = "Topics to discuss")]
	public class PromptSelectionActivity : AppCompatActivity
	{
		RecyclerView promptRecyclerView;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.promptselection);
			SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));

			var model = new DatabaseManager(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal));
			var selectedProject = model.GetProjects().Find((Project pj) => pj.theme == Intent.GetStringExtra("theme"));

			promptRecyclerView = FindViewById<RecyclerView>(Resource.Id.prompts);
			promptRecyclerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false));

			var adapter = new RVPromptAdapter(selectedProject.prompts);
			adapter.ProjectClicked += ProjectSelected;
			promptRecyclerView.SetAdapter(adapter);
			// This allows us to use recyclerview similar to a ViewPager
			new PagerSnapHelper().AttachToRecyclerView(promptRecyclerView);
		}

		void ProjectSelected(object sender, int position)
		{
			var prompt = promptRecyclerView.GetLayoutManager().FindViewByPosition(position).FindViewById(Resource.Id.promptCard);
			// All the previous form data and selected prompt.
			var intent = new Intent(this, typeof(RecordStoryActivity));
			intent.PutExtra("promptImage", prompt.FindViewById<ImageViewAsync>(Resource.Id.imagePrompt).Tag.ToString());
			intent.PutExtra("promptText", prompt.FindViewById<TextView>(Resource.Id.caption).Text);
			// Pass the previous form data
			intent.PutExtras(Intent.Extras);
			StartActivity(intent);
		}
	}
}