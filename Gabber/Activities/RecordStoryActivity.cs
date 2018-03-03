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
using GabberPCL;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using GabberPCL.Models;

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
        // Exposed as used to identify when a prompt was selected
        RVPromptAdapter adapter;
        // This must be held outside ProjectSelected or it would be overridden
        bool FirstPromptSelected;
        // Exposed as we want to get this once a prompt is selected
        int _seconds;
        // Each interview recorded has a unique SID (GUID) to associate annotations with a session.
        string InterviewSessionID;
        // Which project are we recording an interview for?
        int SelectedProjectID;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.record);
            InterviewSessionID = Guid.NewGuid().ToString();

            var _prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            SelectedProjectID = _prefs.GetInt("SelectedProjectID", 0);
            var selectedProject = Queries.ProjectById(SelectedProjectID);

            var promptRecyclerView = FindViewById<RecyclerView>(Resource.Id.prompts);
            promptRecyclerView.SetLayoutManager(new GridLayoutManager(this, 1));

            themes = selectedProject.Prompts;
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
            ItemSelected(position);
            var recordButton = FindViewById<FloatingActionButton>(Resource.Id.start);
            // Has the first topic been selected, i.e. one of the states has changed
            if (themes.FindAll((p) => p.SelectionState != Prompt.SelectedState.never).Count == 1) {
                FirstPromptSelected = true;
                FindViewById<TextView>(Resource.Id.recordInstructions).Visibility = ViewStates.Invisible;
                recordButton.Visibility = ViewStates.Visible;
                recordButton.PerformClick();
            }

            if (_isrecording) 
            {
                var current = themes.Find((p) => p.SelectionState == Prompt.SelectedState.current);
                Queries.CreateAnnotation(_seconds, InterviewSessionID, current.ID);
                adapter.PromptSeleted(position);
            }
            
        }

        void ItemSelected(int currentSelected)
        {
            int previousSelected = themes.FindIndex((Prompt p) => p.SelectionState == Prompt.SelectedState.current);
            var selectedItems = new List<int> { currentSelected };
            if (previousSelected != -1)
            {
                // The item selected was the same as the last (nothing changed) so do nothing.
                if (themes[previousSelected].Equals(themes[currentSelected])) return;
                themes[previousSelected].SelectionState = Prompt.SelectedState.previous;
                selectedItems.Add(previousSelected);
            }
            themes[currentSelected].SelectionState = Prompt.SelectedState.current;
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

        void SaveRecording()
		{
            // Only once a recording is complete can End for each annotation be computed
            InterviewPrompt.ComputeEndForAllAnnotationsInSession(_seconds);

            // Added before to simplify accessing the participants involved next.
            Queries.AddSelectedParticipantsToInterviewSession(InterviewSessionID);

            var InterviewSession = new InterviewSession
            {
                SessionID = InterviewSessionID,
                RecordingURL = _path,

                CreatorEmail = Session.ActiveUser.Email,
                ProjectID = SelectedProjectID,

                Prompts = Queries.AnnotationsForLastSession(),
                Participants = Queries.ParticipantsForSession(InterviewSessionID),

                IsUploaded = false
            };

            Queries.AddInterviewSession(InterviewSession);
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
