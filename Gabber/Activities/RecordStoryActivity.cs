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
    // Created here & not PCL as it is not used elsewhere for now ...
    public class Annotation 
    {
        public string Prompt { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
    }

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
        List<Annotation> SelectedAnnotationsAsJSON;
        // Exposed as used to identify when a prompt was selected
        RVPromptAdapter adapter;
        // This must be held outside ProjectSelected or it would be overridden
        bool FirstPromptSelected;
        // Exposed as we want to get this once a prompt is selected
        int _seconds;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.record);
            // There are no annotations by default
            SelectedAnnotationsAsJSON = new List<Annotation>();

            var model = new DatabaseManager(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal));
            var selectedProject = model.GetProjects().Find((Project pj) => pj.theme == Intent.GetStringExtra("theme"));

            var promptRecyclerView = FindViewById<RecyclerView>(Resource.Id.prompts);
            promptRecyclerView.SetLayoutManager(new GridLayoutManager(this, 1));

            themes = selectedProject.prompts;
            adapter = new RVPromptAdapter(themes);
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

				if (record.Selected)
				{
                    timer.Visibility = timer.Visibility == ViewStates.Invisible ? ViewStates.Visible : ViewStates.Invisible;

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
						_seconds = 0;

						while (_isrecording)
						{
							timer.Text = TimeSpan.FromSeconds(_seconds++).ToString((@"mm\:ss"));
							await Task.Delay(1000);
						}
					});
				}
				else
				{
                    ModalToVerifyRecordingEnd();
                    // This ensures that the state of the Play/Stop icon does not change
                    record.Selected = true;
				}
			};
		}

        public override void OnBackPressed()
        {
            if (_isrecording) {

                var alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                alert.SetTitle("You are currently recording");
                alert.SetMessage("Are you sure you want to go back?");
                alert.SetIcon(Android.Resource.Drawable.IcDialogAlert);

                alert.SetPositiveButton(Resources.GetText(Resource.String.popup_record_button), (dialog, id) =>
                {
                    StopRecording();
                    base.OnBackPressed();
                });

                alert.SetNegativeButton(Resources.GetText(Resource.String.popup_finish), (dialog, id) =>
                {
                    ((Android.Support.V7.App.AlertDialog)dialog).Dismiss();
                });

                alert.Create().Show();
            }
            else {
                base.OnBackPressed();
            }
        }

        void ProjectSelected(object sender, int position)
        {
            var recordButton = FindViewById<FloatingActionButton>(Resource.Id.start);

            if (!FirstPromptSelected) {
                FirstPromptSelected = true;
                FindViewById<TextView>(Resource.Id.recordInstructions).Visibility = ViewStates.Invisible;
                recordButton.Visibility = ViewStates.Visible;
                recordButton.PerformClick();
            }

            if (_isrecording) 
            {
                SelectedAnnotationsAsJSON.Add(new Annotation
                {
                    Prompt = themes[position].prompt,
                    Start = _seconds,
                    // Because we want to have the end being where the next annotation was clicked
                    // we must CALCULATE this before saving the interview
                    End = 0
                });

                adapter.PromptSeleted(position);
            }
            
        }

        void ModalToVerifyRecordingEnd()
        {
            var alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            alert.SetMessage(Resources.GetText(Resource.String.popup_record_question));

            alert.SetPositiveButton(Resources.GetText(Resource.String.popup_record_button), (dialog, id) =>
            {
                StopRecording();
                SaveRecording();

                var intent = new Intent(this, typeof(MainActivity));
                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                intent.PutExtra("RECORDED_GABBER", "ta");
                StartActivity(intent);
                Finish();
            });

            alert.SetNegativeButton(Resources.GetText(Resource.String.popup_finish), (dialog, id) =>
            {
                ((Android.Support.V7.App.AlertDialog)dialog).Dismiss();
            });

            alert.Create().Show();
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

            // Only one annotation was chosen for the entire recording
            if (SelectedAnnotationsAsJSON.Count == 1) 
            {
                SelectedAnnotationsAsJSON[0].End = _seconds;
            }
            // This ensures if two (or more) annotations are made then the first and last will be updated correctly
            if (SelectedAnnotationsAsJSON.Count > 1) 
            {
                // By default, the end time for the first annotation 
                SelectedAnnotationsAsJSON[0].End = SelectedAnnotationsAsJSON[1].Start;
                // The last annotation continues until the end of the audio
                SelectedAnnotationsAsJSON[SelectedAnnotationsAsJSON.Count - 1].End = _seconds;
            }

            // ensures the end time between the first and last annotations are correct
            if (SelectedAnnotationsAsJSON.Count > 2)
            {
                // Start at the second and go to the second from last
                // Skip the last element as it is equal to seconds ...
                for (int i = 1; i < SelectedAnnotationsAsJSON.Count - 1; i++) 
                {
                    SelectedAnnotationsAsJSON[i].End = SelectedAnnotationsAsJSON[i + 1].Start;
                }

            }

			var story = new Story
			{
				AudioPath = _path,
				Location = currentlocation,
				SessionID = Intent.GetStringExtra("session"),
                // No PromptText here as the user is not responding to a particular prompt, 
                // but rather to a set of prompts related to a project ...
                Theme = Intent.GetStringExtra("theme"),
				InterviewerEmail = prefs.GetString("username", ""),
				ParticipantsAsJSON = Intent.GetStringExtra("participants"),
				Uploaded = false,
                AnnotationsAsJSON = JsonConvert.SerializeObject(SelectedAnnotationsAsJSON),
                Type = "interview"
			};

			// Store locally so we know what users recorded what experiences.
			var model = new DatabaseManager(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal));
			model.InsertStory(story);

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
