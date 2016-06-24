using Android.Media;
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
		// To playback the audio message on click.
		MediaPlayer _player;

		public RecyclerAdapter(List<Tuple<string, string>> stories, MediaPlayer mediaplayer)
		{
			_stories = stories;
			_player = mediaplayer;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.row, parent, false);
			return new StoryViewHolder(row);
		}
		
		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var Myholder = holder as StoryViewHolder;

			// TODO: should be the associated audio-image
			// Myholder.mauthor.SetImageDrawable(Drawable.CreateFromPath("/"));

			// TODO: could set data here and access it (to playback)
			// Myholder.mstory.SetTag(118, _stories[position].Item2);

			// TODO: via a background thread
			// 1. while audio is playing, increment progress by bar 1s
			// 2. If audio has reached end, reset progress bar (via media_player delegate?)
			// 3. If user drags seekbar, then update the position of the audio for that row?

			// Binds click event to all rows (hence reset is required). 
			// Illustrates the logic required to playback audio.
			Myholder.mstory.Click += (sender, e) =>
			{
				_player.Reset();
				_player.SetDataSource(_stories[position].Item2);
				_player.Prepare();

				// Convert the seekbar to the length of the audio in seconds
				// We can only know this once we assign the source above.
				Myholder.mposition.Max = ((_player.Duration % (60 * 1000)) / 1000);

				// Enables state of the play/pause button to change upon press.
				Myholder.mstory.Selected = !Myholder.mstory.Selected;

				// This is when it's in the "pause" state...
				if (Myholder.mstory.Selected) _player.Start();
				else _player.Stop();
			};
		}

		public override int ItemCount
		{
			get { return _stories.Count; }
		}

		public class StoryViewHolder : RecyclerView.ViewHolder
		{
			public ImageView mauthor { get; set; }
			public ImageButton mstory { get; set; }
			public SeekBar mposition { get; set; }

			public StoryViewHolder(View view) : base(view)
			{
				mauthor = view.FindViewById<ImageView>(Resource.Id.author);
				mstory = view.FindViewById<ImageButton>(Resource.Id.story);
				mposition = view.FindViewById<SeekBar>(Resource.Id.position);
			}
		}
	}
}
