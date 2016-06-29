using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using Android.Graphics.Drawables;
using Android.Content;

namespace Linda
{
	public class RecyclerAdapter : RecyclerView.Adapter
	{
		// Each story the user recorded has an associated image and audio.
		List<Tuple<string, string>> _stories;

		public RecyclerAdapter(List<Tuple<string, string>> stories)
		{
			_stories = stories;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.row, parent, false);
			return new StoryViewHolder(row, OnClick);
		}
		
		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var Myholder = holder as StoryViewHolder;

			var authors_face = _stories[position].Item1;
			// Note: a default silhouette is used if no image was taken.
			if (!string.IsNullOrWhiteSpace(authors_face))
				Myholder.mauthor.SetImageDrawable(Drawable.CreateFromPath(authors_face));
		}

		public override int ItemCount
		{
			get { return _stories.Count; }
		}

		public event EventHandler<int> AudioClicked;

		void OnClick(int position)
		{
			if (AudioClicked != null) AudioClicked(this, position);
		}

		public class StoryViewHolder : RecyclerView.ViewHolder
		{
			public ImageView mauthor { get; set; }
			public ImageButton mstory { get; set; }
			public SeekBar mposition { get; set; }

			public StoryViewHolder(View view, Action<int> listener) : base(view)
			{
				mauthor = view.FindViewById<ImageView>(Resource.Id.author);
				mstory = view.FindViewById<ImageButton>(Resource.Id.story);
				mposition = view.FindViewById<SeekBar>(Resource.Id.position);
				// Binds an event to play/pause button click
				mstory.Click += (sender, e) => listener(LayoutPosition);
			}
		}
	}

	class DividerItemDecoration : RecyclerView.ItemDecoration
	{
		readonly Drawable divider;

		public DividerItemDecoration(Context context)
		{
			// Obtains the default divider for this theme.
			var theme = context.ObtainStyledAttributes(new int[] { Android.Resource.Attribute.ListDivider });
			divider = theme.GetDrawable(0);
			theme.Recycle();
		}

		public override void OnDraw(Android.Graphics.Canvas cValue, RecyclerView parent, RecyclerView.State state)
		{
			base.OnDraw(cValue, parent, state);
			// Calculate bounds of divider and draw it onto RecyclerView.
			for (int i = 0; i < parent.ChildCount; i++)
			{
				View child = parent.GetChildAt(i);
				var parameters = (RecyclerView.LayoutParams)child.LayoutParameters;

				int top = child.Bottom + parameters.BottomMargin;
				int bottom = top + divider.IntrinsicHeight;
				// NOTE: right does not account for padding as we went to fill to right.
				divider.SetBounds(parent.PaddingLeft, top, parent.Width, bottom);
				divider.Draw(cValue);
			}
		}
	}
}