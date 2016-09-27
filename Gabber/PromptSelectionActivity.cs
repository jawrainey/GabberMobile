using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Widget;
using Android.Support.Design.Widget;
using FFImageLoading.Views;

namespace Gabber
{
	[Activity(Label = "Swipe to select a topic to gabber about")]
	public class PromptSelectionActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.promptselection);
			SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar));
			// As data is stored entirely in JSON, we retrieve all projects to filter manually.
			var allProjects = new Model().GetProjects();
			// Filter the list based on the previously selected theme
			var selectedProject = allProjects.Find((Project pj) => pj.theme == Intent.GetStringExtra("theme"));
			// Set appropriate prompts for the selected project
			var recyclerView = FindViewById<RecyclerView>(Resource.Id.prompts);
			recyclerView.SetAdapter(new RVPromptAdapter(selectedProject.prompts));

			// Custom layout required to disable vertical scrolling.
			recyclerView.SetLayoutManager(new CustomLinearLayoutManager(this));
			// Handles the "swipe to dismiss" ability that is incorporated as prompt-cards.
			var callback = new PromptSelectorCallback(0, ItemTouchHelper.Left | ItemTouchHelper.Right);
			var touchHelper = new ItemTouchHelper(callback);
			touchHelper.AttachToRecyclerView(recyclerView);

			// Make it obvious how to select a discussion prompt.
			Snackbar.Make(recyclerView, "Click button above when you are happy with your selection.", Snackbar.LengthLong).Show();

			FindViewById<ImageButton>(Resource.Id.selectFAB).Click += delegate
			{
				// Given the selected item is the first item in the view
				var selectedPrompt = recyclerView.FindViewById(Resource.Id.promptCard);
				var promptImage = selectedPrompt.FindViewById<ImageViewAsync>(Resource.Id.imagePrompt);
				var promptText = selectedPrompt.FindViewById<TextView>(Resource.Id.caption).Text;
				// All the previous form data and selected prompt.
				var intent = new Intent(this, typeof(RecordStoryActivity));
				// The tag is the drawable resource ID, which is a Java object, hence conversion and cast.
				intent.PutExtra("promptImage", promptImage.Tag.ToString());
				intent.PutExtra("promptText", promptText);
				// Pass the previous form data (photo/name/email)
				intent.PutExtras(Intent.Extras);
				StartActivity(intent);
			};
		}
	}

	public class CustomLinearLayoutManager : LinearLayoutManager
	{
		public CustomLinearLayoutManager(Context c) : base(c) { }
		// Disable scrolling within the layout container that shows a swipable item.
		public override bool CanScrollVertically() { return false; }
	}
}