using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using System.Collections.Generic;
using Android.Widget;
using Android.Graphics;
using System;

namespace Linda
{
	[Activity(Label = "S3: what do they want to discuss?")]
	public class PromptSelectionActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.promptselection);
			// TODO: create prompt based on thematic analysis of volunteer workshop
			var prompts = new List<Tuple<string, int>>
			{
				new Tuple<string, int>("Prompt One", Resource.Drawable.prompt_one),
				new Tuple<string, int>("Prompt Two", Resource.Drawable.prompt_two),
				new Tuple<string, int>("Prompt Three", Resource.Drawable.prompt_three)
			};

			var recyclerView = FindViewById<RecyclerView>(Resource.Id.prompts);
			recyclerView.SetAdapter(new RVPromptAdapter(prompts));
			// Custom layout required to disable vertical scrolling.
			recyclerView.SetLayoutManager(new CustomLinearLayoutManager(this));
			// Handles the "swipe to dismiss" ability that is incorporated as prompt-cards.
			var callback = new PromptSelectorCallback(0, ItemTouchHelper.Left | ItemTouchHelper.Right);
			var touchHelper = new ItemTouchHelper(callback);
			touchHelper.AttachToRecyclerView(recyclerView);

			var selectPrompt = FindViewById<ImageButton>(Resource.Id.select);
			selectPrompt.Click += delegate
			{
				// When the "next" button is pressed (after it has been switched below)
				if (selectPrompt.Selected)
				{
					// Given the selected item is the first item in the view
					var selectedPrompt = recyclerView.FindViewById(Resource.Id.promptCard);
					var promptImage = selectedPrompt.FindViewById<ImageView>(Resource.Id.imagePrompt);
					var promptText = selectedPrompt.FindViewById<TextView>(Resource.Id.caption).Text;
					// All the previous form data and selected prompt.
					var intent = new Intent(this, typeof(RecordStoryActivity));
					// The tag is the drawable resource ID, which is a Java object, hence conversion and cast.
					intent.PutExtra("promptImage", int.Parse(promptImage.Tag.ToString()));
					intent.PutExtra("promptText", promptText);
					// Pass the previous form data (photo/name/email)
					intent.PutExtras(Intent.Extras);
					StartActivity(intent);
					// Prevents the re-draw of the icon below.
					return;
				}
				FindViewById<CardView>(Resource.Id.promptCard).SetBackgroundColor(Color.ParseColor("#FF4081"));
				// Switch icon only after action
				selectPrompt.Selected = !selectPrompt.Selected;
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