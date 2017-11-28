using Android.App;
using Android.Media;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System.IO;
using System.Threading.Tasks;
using System;
using Android.Support.Design.Widget;
using Android.Content;
using Android.Locations;
using GabberPCL;
using Newtonsoft.Json;
using System.Collections.Generic;
using Android.Support.V7.Widget;

namespace Gabber
{
	[Activity]
	public class RecordStoryActivity : AppCompatActivity
	{
		// TODO: move all recording logic to a seperate class, which is useful when creating a PCL
		MediaRecorder _recorder;
		// Gosh: https://code.google.com/p/android/issues/detail?id=800
		bool _isrecording;
		// The path to the experience recorded.
		string _path;
        // Holds the prompts for this project
        List<Prompt> themes;
        // This will be reset on cancel and updated in several methods
        string SelectedAnnotationsAsJSON;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.record);
            // There are no annotations by default
            SelectedAnnotationsAsJSON = "";

            var model = new DatabaseManager(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal));
            var selectedProject = model.GetProjects().Find((Project pj) => pj.theme == Intent.GetStringExtra("theme"));

            var promptRecyclerView = FindViewById<RecyclerView>(Resource.Id.prompts);
            promptRecyclerView.SetLayoutManager(new GridLayoutManager(this, 2));

            themes = selectedProject.prompts;
            var adapter = new RVPromptAdapter(themes);
            adapter.ProjectClicked += ProjectSelected;
            promptRecyclerView.SetAdapter(adapter);

			var record = FindViewById<FloatingActionButton>(Resource.Id.start);
			var cancel = FindViewById<FloatingActionButton>(Resource.Id.cancel);
			var timer = FindViewById<TextView>(Resource.Id.timer);

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
                // TODO: for now, we are only interested in the annotations made for the recording that's POSTED
                SelectedAnnotationsAsJSON = "";
			};
		}

        void ProjectSelected(object sender, int position)
        {
            if (_isrecording) 
            {
                var annotation = JsonConvert.SerializeObject(new {
                    theme = themes[position].prompt, 
                    time = FindViewById<TextView>(Resource.Id.timer).Text
                });

                SelectedAnnotationsAsJSON += annotation;
                // TODO: change the background color of the selected item 
                // or increase a counter at the bottom so we know when in the
                // audio that the item was selected
            }
            
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
				Location = currentlocation,
				SessionID = Intent.GetStringExtra("session"),
				InterviewerEmail = prefs.GetString("username", ""),
				ParticipantsAsJSON = Intent.GetStringExtra("participants"),
				Uploaded = false,
                AnnotationsAsJSON = JsonConvert.SerializeObject(SelectedAnnotationsAsJSON),
                Type = "interview"
			};

			// Store locally so we know what users recorded what experiences.
			var model = new DatabaseManager(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal));
			model.InsertStory(story);

			var alert = new Android.Support.V7.App.AlertDialog.Builder(this);
			alert.SetTitle(Resources.GetText(Resource.String.popup_record_again));
			alert.SetMessage(Resources.GetText(Resource.String.popup_record_question));

			alert.SetPositiveButton(Resources.GetText(Resource.String.popup_record_button), (s,a) => 
			{
                var intent = new Intent(this, typeof(PreparationActivity));
                SetResult(Result.Ok, intent);
				Finish(); 
			});

			alert.SetNegativeButton(Resources.GetText(Resource.String.popup_finish), (senderAlert, args) =>
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
			_recorder.SetAudioEncodingBitRate(512000);

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
