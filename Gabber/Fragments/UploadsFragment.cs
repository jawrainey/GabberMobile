using System.Linq;
using System.Threading.Tasks;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Firebase.Analytics;
using Gabber.Adapters;
using GabberPCL;
using GabberPCL.Models;
using GabberPCL.Resources;
using Android.Content;
using Gabber.Activities;
using Android.Net.Wifi;

namespace Gabber.Fragments
{
    public class UploadsFragment : Android.Support.V4.App.Fragment
    {
        private FirebaseAnalytics firebaseAnalytics;
        private static UploadsFragment instance;
        private SessionAdapter adapter;
        private Task IsUploading;

        public static UploadsFragment NewInstance()
        {
            if (instance == null) instance = new UploadsFragment { Arguments = new Bundle() };
            return instance;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View rootView = inflater.Inflate(Resource.Layout.sessions_frag, null);
            RecyclerView sessions = rootView.FindViewById<RecyclerView>(Resource.Id.sessions);
            sessions.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false));

            TextView instructions = rootView.FindViewById<TextView>(Resource.Id.sessionsInstructions);
            instructions.Text = StringResources.sessions_ui_header_instructions;

            TextView bodyInstructions = rootView.FindViewById<TextView>(Resource.Id.sessionsBodyInstructions);
            bodyInstructions.Text = StringResources.sessions_ui_body_instructions;

            AppCompatButton sessions_upload = rootView.FindViewById<AppCompatButton>(Resource.Id.upload_sessions);
            sessions_upload.Text = StringResources.sessions_ui_submit_button;
            // If the user clicks upload all, navigates to projects, then comes back,
            // we must ensure that the upload all button is disabled, otherwise a session
            // being uploaded may attempt to be uploaded again. Double email or index oor.
            sessions_upload.Enabled = (IsUploading == null || IsUploading.IsCompleted);

            ((AppCompatActivity)Activity).SupportActionBar.Title = StringResources.sessions_ui_title;

            return rootView;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            firebaseAnalytics = FirebaseAnalytics.GetInstance(Context);
            base.OnCreate(savedInstanceState);

            var sessions = Queries.AllNotUploadedInterviewSessionsForActiveUser();
            sessions = sessions.FindAll(t => !t.IsUploaded);
            adapter = new SessionAdapter(sessions);
            Activity.FindViewById<RecyclerView>(Resource.Id.sessions).SetAdapter(adapter);

            var sessionsUploadButton = Activity.FindViewById<AppCompatButton>(Resource.Id.upload_sessions);

            ShowHideInstructions();

            sessionsUploadButton.Click += (s, e) => UploadIfNot(0, true);
            adapter.SessionClicked += (s, p) => UploadIfNot(p, false);

            var prefs = PreferenceManager.GetDefaultSharedPreferences(Activity.ApplicationContext);
            // Ensures that the dialog only shows after completing a recording.
            if (prefs.GetBoolean("SESSION_RECORDED", false))
            {
                if (IsConnectedToWifi())
                {
                    var suppressAsync = UploadSessions(0, true);
                }
                else
                {
                    new AlertDialog.Builder(Activity)
                        .SetTitle(StringResources.sessions_ui_wifiwarning_title)
                        .SetMessage(StringResources.sessions_ui_wifiwarning_message)
                        .SetNegativeButton(StringResources.sessions_ui_wifiwarning_cancel, (a, b) => { })
                        .SetPositiveButton(StringResources.sessions_ui_wifiwarning_confirm, (a, b) =>
                        {
                            var suppressAsync = UploadSessions(0, true);
                        })
                        .Show();
                }

                prefs.Edit().PutBoolean("SESSION_RECORDED", false).Commit();
            }
        }

        private bool IsConnectedToWifi()
        {
            WifiManager wifiManager = (WifiManager)Activity.GetSystemService(Context.WifiService);

            if (wifiManager.IsWifiEnabled && wifiManager.ConnectionInfo?.NetworkId != -1)
            {
                return true;
            }

            return false;
        }

        // Prevents multiple clicks to the same session, which will spawn threads
        // to upload the session multiple times, which is not good!
        private void UploadIfNot(int position, bool recursive)
        {
            if (IsUploading == null || IsUploading.IsCompleted) IsUploading = UploadSessions(position, recursive);
            else Toast.MakeText(Activity, StringResources.sessions_ui_message_upload_inprogress, ToastLength.Long).Show();
        }

        private void ShowHideInstructions()
        {
            AppCompatButton sessionsUploadButton = Activity.FindViewById<AppCompatButton>(Resource.Id.upload_sessions);
            TextView bodyInstructions = Activity.FindViewById<TextView>(Resource.Id.sessionsBodyInstructions);
            bool noSessions = adapter.Sessions.Count <= 0;
            sessionsUploadButton.Visibility = noSessions ? ViewStates.Gone : ViewStates.Visible;
            bodyInstructions.Visibility = noSessions ? ViewStates.Visible : ViewStates.Gone;
        }

        public async Task<bool> UploadSessions(int index, bool recursive)
        {
            if (adapter.Sessions.ElementAtOrDefault(index) == null) return false;
            var sessionsUploadButton = Activity.FindViewById<AppCompatButton>(Resource.Id.upload_sessions);

            sessionsUploadButton.Enabled = false;

            adapter.SessionIsUploading(index);

            LOG_EVENT_WITH_ACTION("UPLOAD_SESSION", "ATTEMPT");
            var didUpload = await RestClient.Upload(adapter.Sessions[index]);

            if (didUpload)
            {
                LOG_EVENT_WITH_ACTION("UPLOAD_SESSION", "SUCCESS");
                LOG_UPLOAD_ONE(adapter.Sessions[index]);
                adapter.SessionIsUploaded(index);
                Toast.MakeText(Activity, StringResources.sessions_ui_message_upload_success, ToastLength.Long).Show();

                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);

                if (!prefs.GetBoolean("HAS_DISMISSED_DEBRIEF", false))
                {
                    StartActivity(new Intent(Activity, typeof(FirstDebriefActivity)));
                }
                else
                {
                    if (recursive) await UploadSessions(0, true);
                }
            }
            else
            {
                LOG_EVENT_WITH_ACTION("UPLOAD_SESSION", "FAIL");
                adapter.SessionUploadFail(index);
                Toast.MakeText(Activity, StringResources.sessions_ui_message_upload_fail, ToastLength.Long).Show();
            }
            sessionsUploadButton.Enabled = true;
            ShowHideInstructions();
            return true;
        }

        void LOG_EVENT_WITH_ACTION(string eventName, string action)
        {
            var bundle = new Bundle();
            bundle.PutString("ACTION", action);
            bundle.PutString("TIMESTAMP", System.DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
            firebaseAnalytics.LogEvent(eventName, bundle);
        }

        void LOG_UPLOAD_ONE(InterviewSession session)
        {
            var bundle = new Bundle();
            bundle.PutInt("NUM_PARTS", session.Participants.Count);
            bundle.PutString("ID", session.SessionID);
            bundle.PutInt("NUM_TOPICS", session.Prompts.Count);
            firebaseAnalytics.LogEvent("UPLOAD_SESSION", bundle);
        }
    }
}
