using Android.App;
using Android.Media;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using System.IO;
using System.Threading.Tasks;
using System;

namespace Linda
{
	[Activity(Label = "S4: Record their story")]
	public class RecordStoryActivity : AppCompatActivity
	{
		MediaRecorder _recorder;
		// Gosh: https://code.google.com/p/android/issues/detail?id=800
		bool _isrecording;
		// The path to the experience recorded.
		string _path;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.record);

			SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));

			// Enable a user to modify the interviewees details if they wish
			// TODO: needs implemented; parent for this activity must be set.
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			SupportActionBar.SetHomeButtonEnabled(true);

			var record = FindViewById<ImageButton>(Resource.Id.start);
			var cancel = FindViewById<ImageButton>(Resource.Id.cancel);

			// Required to increase timer per second.
			var timer = FindViewById<TextView>(Resource.Id.timer);

			// TODO: ideally, a submit should exist after the audio has been recorded.
			// e.g. if stop pressed, change icon to save, 
			var submit = FindViewById<AppCompatButton>(Resource.Id.submit);

			// Note: record has two states: start and stop record.
			record.Click += delegate
			{
				// Change icon between record to stop.
				record.Selected = !record.Selected;
				// Show/Hide the cancel/timer on click!
				cancel.Visibility = cancel.Visibility == ViewStates.Invisible ? ViewStates.Visible : ViewStates.Invisible;
				timer.Visibility = timer.Visibility == ViewStates.Invisible ? ViewStates.Visible : ViewStates.Invisible;

				if (record.Selected)
				{
					_recorder = new MediaRecorder();
					_isrecording = true;

					// Set how we want the audio formatting to be.
					_recorder.SetAudioSource(AudioSource.Mic);
					_recorder.SetOutputFormat(OutputFormat.ThreeGpp);
					_recorder.SetAudioEncoder(AudioEncoder.AmrNb);

					// Override path for re-use as user may record many audios. Store only once.
					if (string.IsNullOrWhiteSpace(_path))
					{
						_path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
						                     System.Diagnostics.Stopwatch.GetTimestamp() + ".3gpp");	
					}

					_recorder.SetOutputFile(_path);
					_recorder.Prepare();
					_recorder.Start();

					// TODO: do we want users to record for as long as they desire?
					RunOnUiThread(async () =>
					{
						int seconds = 0;

						while (_isrecording)
						{
							timer.Text = TimeSpan.FromSeconds(seconds++).ToString((@"mm\:ss"));
							await Task.Delay(1000);
						}
					});
				}
				else
				{
					submit.Visibility = ViewStates.Visible;
					// TODO: update preview of the audio recorded.
					StopRecording();
				}
			};

			cancel.Click += delegate
			{
				cancel.Visibility = ViewStates.Invisible;
				timer.Visibility = ViewStates.Invisible;
				// Revert back to the "record" icon.
				record.Selected = false;
				// Stops the current audio from playing.
				StopRecording();
			};

			// TODO: temporary button to faciliate implementation.
			submit.Click += delegate
			{
				// Link this interview to interviewer (the logged in user).
				var prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);

				// TODO: assumes ALL fields are validated!
				var story = new Story {
					AudioPath = _path,
					PhotoPath = Intent.GetStringExtra("photo"),
					InterviewerEmail = prefs.GetString("username", ""),
					IntervieweeEmail = Intent.GetStringExtra("email"),
					IntervieweeName = Intent.GetStringExtra("name"),
					Ethics = Intent.GetStringExtra("consent"),
					Location = Intent.GetStringExtra("location"),
					Uploaded = false
				};

				new Model().InsertStory(story);

				// TODO: an audio must have been created! Enable once recording made?
				StopRecording();

				// We do not want the user to return to this page once experience captured.
				Finish();
				StartActivity(typeof(MainActivity));
			};

			// TODO: click functionality for preview of audio playback
		}

		void StopRecording()
		{
			if (_isrecording)
			{
				_isrecording = false;
				_recorder.Stop();
				_recorder.Reset();
			}
		}
	}
}