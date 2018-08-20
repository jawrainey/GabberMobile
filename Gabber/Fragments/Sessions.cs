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
using Android.Text;

namespace Gabber.Fragments
{
    public class Sessions : Android.Support.V4.App.Fragment
    {
		FirebaseAnalytics firebaseAnalytics;
        static Sessions instance;
        SessionAdapter adapter;
        Task IsUploading;

        public static Sessions NewInstance()
        {
            if (instance == null) instance = new Sessions { Arguments = new Bundle() };
            return instance;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var rootView = inflater.Inflate(Resource.Layout.sessions_frag, null);
            var sessions = rootView.FindViewById<RecyclerView>(Resource.Id.sessions);
            sessions.SetLayoutManager(new LinearLayoutManager(Activity, LinearLayoutManager.Vertical, false));

            var instructions = rootView.FindViewById<TextView>(Resource.Id.sessionsInstructions);
            instructions.Text = StringResources.sessions_ui_header_instructions;

            var bodyInstructions = rootView.FindViewById<TextView>(Resource.Id.sessionsBodyInstructions);
            bodyInstructions.Text = StringResources.sessions_ui_body_instructions;

            var sessions_upload = rootView.FindViewById<AppCompatButton>(Resource.Id.upload_sessions);
            sessions_upload.Text = StringResources.sessions_ui_submit_button;
            // If the user clicks upload all, navigates to projects, then comes back,
            // we must ensure that the upload all button is disabled, otherwise a session
            // being uploaded may attempt to be uploaded again. Double email or index oor.
            sessions_upload.Enabled = (IsUploading == null || IsUploading.IsCompleted);

            var toolbar = rootView.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            ((AppCompatActivity)Activity).SetSupportActionBar(toolbar);
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
                prefs.Edit().PutBoolean("SESSION_RECORDED", false).Commit();
                ShowDebriefingDialog();
            }
        }

        // Prevents multiple clicks to the same session, which will spawn threads
        // to upload the session multiple times, which is not good!
        void UploadIfNot(int position, bool recursive)
        {
            if (IsUploading == null || IsUploading.IsCompleted) IsUploading = UploadSessions(position, recursive);
            else Toast.MakeText(Activity, StringResources.sessions_ui_message_upload_inprogress, ToastLength.Long).Show();
        }

        void ShowDebriefingDialog()
        {
            var alert = new AlertDialog.Builder(Activity);
            var session = Queries.LastInterviewSession();
            var content = string.Format(
                StringResources.debriefing_ui_page_first_content, 
                Queries.ProjectById(session.ProjectID).Title, 
                session.ConsentType
            );
            alert.SetTitle(StringResources.debriefing_ui_page_first_title);
            alert.SetMessage(Html.FromHtml(content));

            alert.SetNegativeButton(
                StringResources.debriefing_ui_button_hide,
                (dialog, id) =>
                {
                    ((AlertDialog)dialog).Dismiss();
                    LOG_EVENT_WITH_ACTION("DEBRIEF", "DISMISSED");
                }
            );

            alert.SetPositiveButton(
                StringResources.debriefing_ui_button_upload,
                async (dialog, id) =>
                {
                    ((AlertDialog)dialog).Dismiss();
                    LOG_EVENT_WITH_ACTION("DEBRIEF", "UPLOADED");
                    await UploadSessions(0, false);
                }
            );

            alert.Create().Show();
        }

        void ShowHideInstructions()
        {
            var sessionsUploadButton = Activity.FindViewById<AppCompatButton>(Resource.Id.upload_sessions);
            var bodyInstructions = Activity.FindViewById<TextView>(Resource.Id.sessionsBodyInstructions);
            var noSessions = adapter.Sessions.Count <= 0;
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
                if (recursive) await UploadSessions(0, true);
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
