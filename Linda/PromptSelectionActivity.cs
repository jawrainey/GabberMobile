using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using System.Collections.Generic;
using Android.Widget;

namespace Linda
{
	[Activity(Label = "S3: what do they want to discuss?")]
	public class PromptSelectionActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.promptselection);

			var prompts = new List<string> { "CARD1", "CARD2", "CARD3", "CARD4" };
			var recyclerView = FindViewById<RecyclerView>(Resource.Id.prompts);
			recyclerView.SetAdapter(new RVPromptAdapter(prompts));
			// Custom layout required to disable vertical scrolling.
			recyclerView.SetLayoutManager(new CustomLinearLayoutManager(this));

			// Handles the "swipe to dismiss" ability that is incorporated as prompt-cards.
			var callback = new PromptSelectorCallback(0, ItemTouchHelper.Left | ItemTouchHelper.Right, recyclerView);
			var touchHelper = new ItemTouchHelper(callback);
			touchHelper.AttachToRecyclerView(recyclerView);

			FindViewById<ImageButton>(Resource.Id.select).Click += delegate
			{
				// TODO: validate selection and highlight the chosen one
				// TODO: change button icon to send, and only forward when pressed?
				var intent = new Intent(this, typeof(RecordStoryActivity));
				intent.PutExtra("prompt", "SelectedPrompt");
				// Pass the previous form data (photo/email/pass)
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