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
using Android.Support.Design.Widget;

namespace Linda
{
	[Activity(Label = "Record your gabberings")]
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

			var selectedPrompt = FindViewById(Resource.Id.promptCard);
			selectedPrompt.FindViewById<ImageView>(
				Resource.Id.imagePrompt).SetBackgroundResource(Intent.GetIntExtra("promptImage", -1));
			selectedPrompt.FindViewById<TextView>(
				Resource.Id.caption).Text = Intent.GetStringExtra("promptText");
			
			var record = FindViewById<FloatingActionButton>(Resource.Id.start);
			var cancel = FindViewById<FloatingActionButton>(Resource.Id.cancel);
			var timer = FindViewById<TextView>(Resource.Id.timer);
			var submit = FindViewById<AppCompatButton>(Resource.Id.submit);

			Snackbar.Make(record, "Note: only one conversation is saved.", Snackbar.LengthLong).Show();

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
					submit.Visibility = ViewStates.Invisible;

					// Override path for re-use as user may record many audios. Store only once.
					if (string.IsNullOrWhiteSpace(_path))
					{
						var personal = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
						_path = Path.Combine(personal, System.Diagnostics.Stopwatch.GetTimestamp() + ".mp3");	
					}

					StartRecording();

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
					submit.Visibility = submit.Visibility == ViewStates.Invisible ? ViewStates.Visible : ViewStates.Invisible;
					StopRecording();
				}
			};

			cancel.Click += delegate
			{
				cancel.Visibility = ViewStates.Invisible;
				timer.Visibility = ViewStates.Invisible;
				submit.Visibility = ViewStates.Invisible;
				// Revert back to the "record" icon.
				record.Selected = false;
				// Stops the current audio from playing.
				StopRecording();
			};

			submit.Click += delegate
			{
				// Link this interview to interviewer (the logged in user).
				var prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);

				// TODO: get the current location!
				var story = new Story {
					AudioPath = _path,
					PhotoPath = Intent.GetStringExtra("photo"),
					InterviewerEmail = prefs.GetString("username", ""),
					IntervieweeEmail = Intent.GetStringExtra("email"),
					IntervieweeName = Intent.GetStringExtra("name"),
					Location = Intent.GetStringExtra("location"),
					promptText = Intent.GetStringExtra("promptText"),
					Uploaded = false
				};
				// Store locally so we know what users recorded what experiences.
				new Model().InsertStory(story);
				// For now, we will not notify the user that the data is uploading or has been uploaded.
				// TODO: this information should be represented visually on the dashboard.
				new RestAPI().Upload(story);
				// We do not want the user to return to this page once experience captured.
				Finish();
				StartActivity(typeof(CompletionActivity));
			};
		}

		void StartRecording()
		{
			_recorder = new MediaRecorder();
			_isrecording = true;
			// Set how we want the audio formatting to be.
			_recorder.SetAudioSource(AudioSource.Mic);
			_recorder.SetOutputFormat(OutputFormat.Mpeg4);
			_recorder.SetAudioEncoder(AudioEncoder.Aac);

			_recorder.SetOutputFile(_path);
			_recorder.Prepare();
			_recorder.Start();
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