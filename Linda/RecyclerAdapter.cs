using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

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
			// var Myholder = holder as StoryViewHolder;
			// TODO: should be the associated audio-image
			// Myholder.mauthor.SetImageDrawable(Drawable.CreateFromPath("/"));
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
}
