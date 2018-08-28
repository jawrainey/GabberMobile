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
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Content;
using GabberPCL;
using System.Collections.Generic;
using Android.Support.V7.Widget;
using GabberPCL.Models;
using Gabber.Adapters;
using GabberPCL.Resources;
using Android.Graphics;
using Android.Views.Animations;
using System.Linq;
using Firebase.Analytics;
using Android.Content.PM;
using Android.Preferences;
using Android.Support.V4.View;

namespace Gabber
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class RecordStoryActivity : AppCompatActivity
    {
        FirebaseAnalytics firebaseAnalytics;
        // TODO: move all recording logic to a seperate class, which is useful when creating a PCL
        MediaRecorder _recorder;
        // Gosh: https://code.google.com/p/android/issues/detail?id=800
        bool _isrecording;
        // The path to the experience recorded.
        string _path;
        // Holds the prompts for this project
        List<Prompt> themes;
        // Exposed as used to identify when a prompt was selected
        TopicAdapter adapter;
        // Exposed as we want to get this once a prompt is selected
        int _seconds;
        // Each interview recorded has a unique SID (GUID) to associate annotations with a session.
        string InterviewSessionID;
        // Which project are we recording an interview for?
        int SelectedProjectID;
        // The consent chosen by participants about to Gabber
        string ConsentType;
        private int langId;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            firebaseAnalytics = FirebaseAnalytics.GetInstance(ApplicationContext);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.record);
            SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));
            SupportActionBar.Title = StringResources.recording_ui_title;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var instructionsHeader = FindViewById<TextView>(Resource.Id.recordInstructionsHeader);
            instructionsHeader.Text = StringResources.recording_ui_instructions_header;

            InterviewSessionID = Guid.NewGuid().ToString();

            var _prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            ConsentType = _prefs.GetString("SESSION_CONSENT", "");
            SelectedProjectID = _prefs.GetInt("SelectedProjectID", 0);
            langId = _prefs.GetInt("SESSION_LANG", 1);
            var selectedProject = Queries.ProjectById(SelectedProjectID);

            var promptRecyclerView = FindViewById<RecyclerView>(Resource.Id.prompts);
            promptRecyclerView.SetLayoutManager(new GridLayoutManager(this, 1));

            themes = selectedProject.Prompts;
            adapter = new TopicAdapter(themes);
            adapter.ProjectClicked += ProjectSelected;
            promptRecyclerView.SetAdapter(adapter);

            var record = FindViewById<FloatingActionButton>(Resource.Id.start);
            ViewCompat.SetBackgroundTintList(record, Android.Content.Res.ColorStateList.ValueOf(Color.LightGray));
            record.Enabled = false;
            var timer = FindViewById<TextView>(Resource.Id.timer);
            timer.SetTextColor(Color.LightGray);

            // Note: record has two states: start and stop record.
            record.Click += delegate
            {
                LOG_EVENT("RECORD_CLICKED");
                // Change icon between record to stop.
                record.Selected = !record.Selected;

                if (record.Selected)
                {
                    // Override path for re-use as user may record many audios. Store only once.
                    if (string.IsNullOrWhiteSpace(_path))
                    {
                        var personal = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                        _path = System.IO.Path.Combine(personal, DateTimeOffset.Now.ToUnixTimeSeconds() + ".mp3");
                    }

                    StartRecording();

                    // TODO: do we want users to record for as long as they desire?
                    RunOnUiThread(async () =>
                    {
                        _seconds = 0;

                        while (_isrecording)
                        {
                            SupportActionBar.Title = StringResources.recording_ui_title_active;
                            timer.Text = Queries.FormatFromSeconds(_seconds++);
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
            if (_isrecording)
            {

                var alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                alert.SetTitle(StringResources.recording_ui_dialog_back_title);
                alert.SetMessage(StringResources.recording_ui_dialog_back_body);

                alert.SetPositiveButton(StringResources.recording_ui_dialog_back_positive, (dialog, id) =>
                {
                    StopRecording();
                    LOG_EVENT_WITH_ACTION("BACK_PRESSED_WHILE_RECORDING", "CONTINUE");
                    base.OnBackPressed();
                });

                alert.SetNegativeButton(StringResources.recording_ui_dialog_back_negative, (dialog, id) =>
                {
                    LOG_EVENT_WITH_ACTION("CANCEL_BACK_PRESSED_WHILE_RECORDING", "DISMISSED");
                    ((Android.Support.V7.App.AlertDialog)dialog).Dismiss();
                });
                LOG_EVENT_WITH_ACTION("PRESSED_GO_BACK_WHEN_RECORDING", "DISPLAYED");
                alert.Create().Show();
            }
            else
            {
                LOG_EVENT("BACK_WITHOUT_RECORD");
                base.OnBackPressed();
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            OnBackPressed();
            return true;
        }

        void ProjectSelected(object sender, int position)
        {
            ItemSelected(position);
            var recordButton = FindViewById<FloatingActionButton>(Resource.Id.start);
            // Has the first topic been selected, i.e. one of the states has changed
            if (themes.FindAll((p) => p.SelectionState != Prompt.SelectedState.never).Count == 1)
            {
                var record = FindViewById<FloatingActionButton>(Resource.Id.start);
                record.SetImageResource(Resource.Drawable.stop_recording);
                ViewCompat.SetBackgroundTintList(record, Android.Content.Res.ColorStateList.ValueOf(Color.White));
                record.Enabled = true;
                FindViewById<TextView>(Resource.Id.timer).SetTextColor(Color.White);
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
            var previousSelected = themes.FindIndex((Prompt p) => p.SelectionState == Prompt.SelectedState.current);
            var selectedItems = new List<int> { currentSelected };
            if (previousSelected != -1)
            {
                // The item selected was the same as the last (nothing changed) so do nothing.
                if (themes[previousSelected].Equals(themes[currentSelected])) return;
                themes[previousSelected].SelectionState = Prompt.SelectedState.previous;
                selectedItems.Add(previousSelected);
            }
            LOG_TOPIC_SELECTED(themes[currentSelected]);
            themes[currentSelected].SelectionState = Prompt.SelectedState.current;
        }

        void ModalToVerifyRecordingEnd()
        {
            var alert = new Android.Support.V7.App.AlertDialog.Builder(this);
            var uniqueTopics = new HashSet<int>(Queries.AnnotationsForLastSession().Select((i) => i.PromptID));
            var message = string.Format(StringResources.recording_ui_dialog_finish_title, uniqueTopics.Count, themes.Count);
            alert.SetMessage(message);

            alert.SetPositiveButton(StringResources.recording_ui_dialog_finish_positive, (dialog, id) =>
            {
                StopRecording();
                SaveRecording();

                var intent = new Intent(this, typeof(MainActivity));
                intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask | ActivityFlags.NewTask);
                intent.PutExtra("FRAGMENT_TO_SHOW", "gabbers");
                Finish();
                StartActivity(intent);
            });

            alert.SetNegativeButton(StringResources.recording_ui_dialog_finish_negative, (dialog, id) =>
            {
                ((Android.Support.V7.App.AlertDialog)dialog).Dismiss();
            });

            alert.Create().Show();
        }

        void SaveRecording()
        {
            // Only once a recording is complete can End for each annotation be computed
            InterviewPrompt.ComputeEndForAllAnnotationsInSession(_seconds);

            InterviewSession thisSession = new InterviewSession
            {
                ConsentType = ConsentType,
                SessionID = InterviewSessionID,
                RecordingURL = _path,
                CreatedAt = DateTime.UtcNow,
                Lang = langId,
                CreatorEmail = Session.ActiveUser.Email,
                ProjectID = SelectedProjectID,

                Prompts = Queries.AnnotationsForLastSession(),

                IsUploaded = false
            };

            // Added before to simplify accessing the participants involved next.
            thisSession = Queries.AddSelectedParticipantsToInterviewSession(thisSession);

            Queries.AddInterviewSession(thisSession);
            // Now the session has been stored to the database we no longer need it
            var _prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            _prefs.Edit().Remove("SESSION_CONSENT").Commit();
            _prefs.Edit().Remove("SESSION_LANG").Commit();
            _prefs.Edit().PutBoolean("SESSION_RECORDED", true).Commit();
        }

        void StartRecording()
        {
            _recorder = new MediaRecorder();
            _isrecording = true;
            // Set how we want the audio formatting to be.
            _recorder.SetAudioSource(AudioSource.Mic);
            _recorder.SetOutputFormat(OutputFormat.AmrWb);
            _recorder.SetAudioEncoder(AudioEncoder.AmrWb);
            _recorder.SetAudioSamplingRate(8000);
            _recorder.SetAudioEncodingBitRate(23850);

            _recorder.SetOutputFile(_path);
            _recorder.Prepare();
            _recorder.Start();
            LOG_EVENT("START_RECORDING");
        }

        void StopRecording()
        {
            if (_isrecording)
            {
                _isrecording = false;
                _recorder.Stop();
                _recorder.Reset();
                LOG_EVENT("STOP_RECORDING");
            }
        }

        void LOG_EVENT_WITH_ACTION(string eventName, string action)
        {
            var bundle = new Bundle();
            bundle.PutString("ACTION", action);
            bundle.PutString("TIMESTAMP", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
            firebaseAnalytics.LogEvent(eventName, bundle);
        }

        void LOG_EVENT(string eventName)
        {
            var bundle = new Bundle();
            bundle.PutString("TIMESTAMP", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
            firebaseAnalytics.LogEvent(eventName, bundle);
        }

        public void LOG_TOPIC_SELECTED(Prompt topic)
        {
            var bundle = new Bundle();
            bundle.PutString("TEXT", topic.Text);
            bundle.PutInt("ID", topic.ID);

            var previous = themes.Find((obj) => obj.SelectionState == Prompt.SelectedState.previous);
            bundle.PutString("PREVIOUS_TEXT", previous != null ? previous.Text : "");
            bundle.PutInt("PREVIOUS_ID", previous != null ? previous.ID : -1);

            firebaseAnalytics.LogEvent("TOPIC_SELECTED", bundle);
        }
    }
}
