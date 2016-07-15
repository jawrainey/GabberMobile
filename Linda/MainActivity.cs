using Android.App;
using Android.Media;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Linda
{
	[Activity(Label = "Your recorded chats")]
	public class MainActivity : AppCompatActivity
	{
		// Used to obtain items from the RecyclerView
		RecyclerView mView;
		// Each story the user recorded has an associated image and audio.
		List<Tuple<string, string>> _stories;
		// To playback the audio message on click.
		MediaPlayer mplayer;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			// Used to display interviews that the logged in user has recorded
			// This ensures if many log into the device, then they will see theirs.
			var prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);

			// One MediaPlayer to rule the view.
			mplayer = new MediaPlayer();

			// The stories (experiences) the user has gathered from other people.
			_stories = new List<Tuple<string, string>>();

			// Only show stories for the current logged in user
			// NOTE: returns all data for a story, as meta-data may be used later.
			foreach (var story in new Model().GetStories(prefs.GetString("username", "")))
			{
				_stories.Add(new Tuple<string, string>(story.PhotoPath, story.AudioPath));	
			}

			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.main);

			// Toolbar will now take on default actionbar characteristics
			SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar));

			mView = FindViewById<RecyclerView>(Resource.Id.stories);
			mView.HasFixedSize = true;
			mView.SetLayoutManager(new LinearLayoutManager(this));
			mView.AddItemDecoration(new DividerItemDecoration(this));

			var mAdapter = new RecyclerAdapter(_stories);
			mAdapter.AudioClicked += OnAudioClick; 

			mView.SetAdapter(mAdapter);

			FindViewById<FloatingActionButton>(Resource.Id.fab).Click += delegate
			{
				StartActivity(typeof(PreparationActivity));
			};


			if (_stories.Count == 0) Snackbar.Make(mView, "Whose perspective do you want to share?", 0).Show();
		}

		void OnAudioClick(object sender, int position)
		{
			// The current RecyclerView row and the relevant seekbar/story
			var row = (RecyclerAdapter.StoryViewHolder)mView.FindViewHolderForAdapterPosition(position);
			var cbar = row.mposition;
			var cstory = row.mstory;

			// Why must we reset?
			mplayer.Reset();
			mplayer.SetDataSource(_stories[position].Item2);
			mplayer.Prepare();

			// We can only know this once we assign the audio source above.
			cbar.Max = (int)Math.Round(TimeSpan.FromMilliseconds(mplayer.Duration).TotalSeconds);

			// Binds this event to the selected item in the RecyclerView row above.
			cbar.ProgressChanged += (object send, SeekBar.ProgressChangedEventArgs e) =>
			{
				if (e.FromUser)
				{
					cstory.Selected = false;
					mplayer.Stop();
				}
			};

			// Enables state of the play/pause button to change upon press.
			cstory.Selected = !cstory.Selected;

			// This is when it's in the "pause" state...
			if (cstory.Selected)
			{
				// Start where we left off, including the ProgressChanged event. 
				mplayer.SeekTo(cbar.Progress * 1000);
				mplayer.Start();
			}
			else
			{
				mplayer.Stop();
			}

			// Increase the seekbars progress to reflect audio position, including ending.
			RunOnUiThread(async () =>
			{
				// TODO: this is for any (or all) audios playing, rather than this audio.
				while (mplayer.IsPlaying)
				{
					cbar.Progress += 1;
					await Task.Delay(1000);
				}

				// The maximum for each audio may be different.
				if (cbar.Progress == cbar.Max)
				{
					cbar.Progress = 0;
					cstory.Selected = false;
				}
			});
		}
	}
}