using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using GabberPCL;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Gabber
{
	[Activity]
	public class PromptSelectionActivity : AppCompatActivity
	{
		// Used to uniquely identify this activity across requests; particularly on result.
		static int uniqueRequestCode = 10125;
		// The view container for prompts in this project
		RecyclerView promptRecyclerView;
		// Holds the prompts for this project
		List<Prompt> prompts;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.promptselection);
			SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));
			SupportActionBar.Title = Resources.GetText(Resource.String.topics_to_discuss);

			var model = new DatabaseManager(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal));
			var selectedProject = model.GetProjects().Find((Project pj) => pj.theme == Intent.GetStringExtra("theme"));

			promptRecyclerView = FindViewById<RecyclerView>(Resource.Id.prompts);
			promptRecyclerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false));

			prompts = selectedProject.prompts;
			var adapter = new RVPromptAdapter(prompts);
			adapter.ProjectClicked += ProjectSelected;
			promptRecyclerView.SetAdapter(adapter);
			// This allows us to use recyclerview similar to a ViewPager
			new PagerSnapHelper().AttachToRecyclerView(promptRecyclerView);
		}

		void ProjectSelected(object sender, int position)
		{
			// All the previous form data and selected prompt.
			var intent = new Intent(this, typeof(RecordStoryActivity));
			intent.PutExtra("promptImage", prompts[position].imageName);
			intent.PutExtra("promptText", prompts[position].prompt);
			intent.PutExtra("PROMPT_SELECTION_POSITION", position);
			// Pass the newly added and previous form data to the next intent
			intent.PutExtras(Intent.Extras);
			// When the next intent finishes, we expect a result returned, e.g. the prompt position.
			StartActivityForResult(intent, uniqueRequestCode);
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			if (requestCode == uniqueRequestCode && resultCode == Result.Ok)
			{
				promptRecyclerView.SmoothScrollToPosition(data.GetIntExtra("PROMPT_SELECTION_POSITION", 0) + 1);
			}
		}

	}
}
