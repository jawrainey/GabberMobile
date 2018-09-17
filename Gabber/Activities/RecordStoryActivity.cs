using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Firebase.Analytics;
using Gabber.Activities;
using Gabber.Adapters;
using Gabber.Helpers;
using GabberPCL;
using GabberPCL.Models;
using GabberPCL.Resources;

namespace Gabber
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, ParentActivity = typeof(ConversationConsent))]
    public class RecordStoryActivity : AppCompatActivity
    {
        FloatingActionButton record;
        FirebaseAnalytics firebaseAnalytics;
        // TODO: move all recording logic to a seperate class, which is useful when creating a PCL
        MediaRecorder _recorder;
        // Gosh: https://code.google.com/p/android/issues/detail?id=800
        bool _isrecording;
        // The path to the experience recorded.
        string _path;
        // Holds the prompts for this project
        List<Topic> themes;
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
        TextView timer;

        readonly string[] micPerms = { Manifest.Permission.RecordAudio };
        const int permsReq = 99;
        int posWaitingOnPerm;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            firebaseAnalytics = FirebaseAnalytics.GetInstance(ApplicationContext);
            base.OnCreate(savedInstanceState);
            Localise.SetLayoutDirectionByPreference(this);
            SetContentView(Resource.Layout.record);

            SupportActionBar.Title = StringResources.recording_ui_title;

            var instructionsHeader = FindViewById<TextView>(Resource.Id.recordInstructionsHeader);
            instructionsHeader.Text = StringResources.recording_ui_instructions_header;

            InterviewSessionID = Guid.NewGuid().ToString();

            var _prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            ConsentType = _prefs.GetString("SESSION_CONSENT", "");
            SelectedProjectID = _prefs.GetInt("SelectedProjectID", 0);
            langId = _prefs.GetInt("SESSION_LANG", 1);
            var selectedProject = Queries.ProjectById(SelectedProjectID);

            RecyclerView promptRecyclerView = FindViewById<RecyclerView>(Resource.Id.prompts);
            promptRecyclerView.SetLayoutManager(new GridLayoutManager(this, 1));

            Content project = Localise.ContentByLanguage(selectedProject, langId);
            List<Topic> activeTopics = project.Topics.Where((p) => p.IsActive).ToList();
            themes = activeTopics;
            adapter = new TopicAdapter(themes);
            adapter.ProjectClicked += CheckRecPerm;
            promptRecyclerView.SetAdapter(adapter);

            record = FindViewById<FloatingActionButton>(Resource.Id.start);

            FindViewById<TextView>(Resource.Id.themeTitle).Text = project.Title;

            Color highlightColor = new Color(ContextCompat.GetColor(this, Resource.Color.colorControlHighlight));

            ViewCompat.SetBackgroundTintList(record, Android.Content.Res.ColorStateList.ValueOf(highlightColor));
            record.Enabled = false;
            timer = FindViewById<TextView>(Resource.Id.timer);
            timer.SetTextColor(highlightColor);

            record.Click += HandleRecordClick;
        }

        private void HandleRecordClick(object sender, EventArgs e)
        {
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

        private void CheckRecPerm(object sender, int position)
        {
            LOG_EVENT("RECORD_CLICKED");
            posWaitingOnPerm = position;

            if ((int)Build.VERSION.SdkInt >= 23 && !record.Selected)
            {
                if (CheckSelfPermission(micPerms[0]) == (int)Permission.Granted)
                {
                    ProjectSelected(position);
                    return;
                }

                // Need permission
                RequestPermissions(micPerms, permsReq);
            }
            else
            {
                ProjectSelected(position);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode == permsReq)
            {
                if (grantResults[0] == Permission.Granted)
                {
                    ProjectSelected(posWaitingOnPerm);
                }
                else
                {
                    Toast.MakeText(this, StringResources.recording_ui_permission_body, ToastLength.Long).Show();
                }
            }
        }

        private void ProjectSelected(int position)
        {
            ItemSelected(position);
            var recordButton = FindViewById<FloatingActionButton>(Resource.Id.start);
            // Has the first topic been selected, i.e. one of the states has changed
            if (themes.FindAll((p) => p.SelectionState != Topic.SelectedState.never).Count == 1)
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
                var current = themes.Find((p) => p.SelectionState == Topic.SelectedState.current);
                Queries.CreateAnnotation(_seconds, InterviewSessionID, current.ID);
                adapter.PromptSeleted(position);
            }

        }

        void ItemSelected(int currentSelected)
        {
            var previousSelected = themes.FindIndex((Topic p) => p.SelectionState == Topic.SelectedState.current);
            var selectedItems = new List<int> { currentSelected };
            if (previousSelected != -1)
            {
                // The item selected was the same as the last (nothing changed) so do nothing.
                if (themes[previousSelected].Equals(themes[currentSelected])) return;
                themes[previousSelected].SelectionState = Topic.SelectedState.previous;
                selectedItems.Add(previousSelected);
            }
            LOG_TOPIC_SELECTED(themes[currentSelected]);
            themes[currentSelected].SelectionState = Topic.SelectedState.current;
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
                intent.PutExtra("FRAGMENT_TO_SHOW", "settings");
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

        public void LOG_TOPIC_SELECTED(Topic topic)
        {
            var bundle = new Bundle();
            bundle.PutString("TEXT", topic.Text);
            bundle.PutInt("ID", topic.ID);

            var previous = themes.Find((obj) => obj.SelectionState == Topic.SelectedState.previous);
            bundle.PutString("PREVIOUS_TEXT", previous != null ? previous.Text : "");
            bundle.PutInt("PREVIOUS_ID", previous != null ? previous.ID : -1);

            firebaseAnalytics.LogEvent("TOPIC_SELECTED", bundle);
        }
    }
}
