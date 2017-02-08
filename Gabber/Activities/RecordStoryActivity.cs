using Android.App;
using Android.Media;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using System.IO;
using System.Threading.Tasks;
using System;
using Android.Support.Design.Widget;
using Android.Content;
using Android.Locations;
using FFImageLoading.Views;
using FFImageLoading;
using GabberPCL;
using Android.Preferences;

namespace Gabber
{
	[Activity(Label = "Capture and share the Gabber")]
	public class RecordStoryActivity : AppCompatActivity
	{
		// TODO: move all recording logic to a seperate class, which is useful when creating a PCL
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
			var imageView = selectedPrompt.FindViewById<ImageViewAsync>(Resource.Id.imagePrompt);
			// Download or retrieve from cache, the image the user has _previously_ selected.
			ImageService.Instance.LoadUrl(Intent.GetStringExtra("promptImage")).Into(imageView);

			selectedPrompt.FindViewById<TextView>(
				Resource.Id.caption).Text = Intent.GetStringExtra("promptText");
			
			var record = FindViewById<FloatingActionButton>(Resource.Id.start);
			var cancel = FindViewById<FloatingActionButton>(Resource.Id.cancel);
			var timer = FindViewById<TextView>(Resource.Id.timer);

			Snackbar.Make(record, "Why not introduce your friend?", Snackbar.LengthLong).Show();

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
					// Override path for re-use as user may record many audios. Store only once.
					if (string.IsNullOrWhiteSpace(_path))
					{
						var personal = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
						_path = Path.Combine(personal, DateTimeOffset.Now.ToUnixTimeSeconds() + ".mp3");	
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
					StopRecording();
					// Saves the recording and sends to next screen for play-back
					SaveRecording();
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
		}

		async void SaveRecording()
		{
			// Link this interview to interviewer (the logged in user).
			var prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);

			// TODO: have a guess what needs re-written...
			string currentlocation = "N/A";

			try
			{
				var locMan = (LocationManager)GetSystemService(LocationService);
				Location location = locMan.GetLastKnownLocation(locMan.GetBestProvider(new Criteria(), true));
				currentlocation = location.Latitude + " / " + location.Longitude;
			}
			catch { }

			var story = new Story
			{
				AudioPath = _path,
				PhotoPath = Intent.GetStringExtra("photo"),
				InterviewerEmail = prefs.GetString("username", ""),
				IntervieweeEmail = Intent.GetStringExtra("email"),
				IntervieweeName = Intent.GetStringExtra("name"),
				Location = currentlocation,
				promptText = Intent.GetStringExtra("promptText"),
				Uploaded = false
			};

			// Store locally so we know what users recorded what experiences.
			var model = new DatabaseManager(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal));
			model.InsertStory(story);

			var alert = new Android.Support.V7.App.AlertDialog.Builder(this);
			alert.SetTitle("Interview saved, but what next?");
			alert.SetMessage("You can record another interview with the same project and participant, " + 
			                 "or select another project or participant to interview.");

			alert. SetPositiveButton("Record again", (senderAlert, args) =>
			{
				Finish();
			});

			alert.SetNegativeButton("Finish", (senderAlert, args) =>
			{
				var intent = new Intent(this, typeof(MainActivity));
				intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
				StartActivity(intent);
				Finish();
			});

			alert.Create().Show();

			if (await new RestClient().Upload(story))
			{
				story.Uploaded = true;
				model.UpdateStory(story);
			}
		}

		void StartRecording()
		{
			_recorder = new MediaRecorder();
			_isrecording = true;
			// Set how we want the audio formatting to be.
			_recorder.SetAudioSource(AudioSource.Mic);
			_recorder.SetOutputFormat(OutputFormat.Mpeg4);
			_recorder.SetAudioEncoder(AudioEncoder.Aac);
			_recorder.SetAudioSamplingRate(44100);
			_recorder.SetAudioEncodingBitRate(96000);

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