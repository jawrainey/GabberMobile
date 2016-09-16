using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using System.Collections.Generic;
using Android.Widget;
using Android.Support.Design.Widget;
using System.Threading.Tasks;
using FFImageLoading.Views;
using System.Linq;

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

			// TODO: move this to MainActivity and make async (not pmact GUI).
			// Only make a new request if something has changed?
			var projectResponse = new RestAPI().GetProjects().Result;

			// TODO: we should never be on this page without having projects. However, this may happen if a user
			// downloads the application and wants to contribute. We could show all the public projects, then,
			// if they want to contribute, then they can "join the campaign".
			if (projectResponse.projects.Count <= 0) return;

			var recyclerView = FindViewById<RecyclerView>(Resource.Id.prompts);

			// TODO: this should be configured based on what _theme_ the user selects on the main/home page.
			// The projectResponse logic (to find all associated information) will be moved there and _must_ be stored
			// within a local database. Everytime a user enters the app, a request is made to check if
			// TODO: remove hard-coded element (and LINQ use) as we should lookup by theme.
			// TODO: cache and images. When the user opens the app, all images/text are downloaded/cached.
			// That way, they can use it offline. However, this is not currently possible (see above return).
			recyclerView.SetAdapter(new RVPromptAdapter(projectResponse.projects.ElementAt(0).prompts));

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

	// These classes are used JSON deserialization.

	public class RootObject
	{
		public List<Project> projects { get; set; }
	}

	public class Project
	{
		public string theme { get; set; }
		public List<Prompt> prompts { get; set; }
	}

	public class Prompt
	{
		public string prompt { get; set; }
		public string imageName { get; set; }
	}
}