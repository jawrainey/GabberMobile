using System.Linq;
using System.Threading.Tasks;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabber.Adapters;
using GabberPCL;
using GabberPCL.Resources;

namespace Gabber.Fragments
{
    public class Sessions : Android.Support.V4.App.Fragment
    {
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
            base.OnCreate(savedInstanceState);

            var sessions = Queries.AllNotUploadedInterviewSessionsForActiveUser();
            sessions = sessions.FindAll(t => !t.IsUploaded);
            adapter = new SessionAdapter(sessions);

            Activity.FindViewById<RecyclerView>(Resource.Id.sessions).SetAdapter(adapter);

            var sessionsUploadButton = Activity.FindViewById<AppCompatButton>(Resource.Id.upload_sessions);

            ShowHideInstructions();

            sessionsUploadButton.Click += (s, e) => UploadIfNot(0, true);
            adapter.SessionClicked += (s, p) => UploadIfNot(p, false);

            // As we get to this fragment via MainActivity, passing data via intents
            // is not ideal. Instead, based on existing sessions, we can show the dialog.
            if (sessions.Count == 1)
            {
                var prefs = PreferenceManager.GetDefaultSharedPreferences(Activity.ApplicationContext);
                if (!prefs.GetBoolean("FIRST_RECORDING_DIALOG", false))
                {
                    prefs.Edit().PutBoolean("FIRST_RECORDING_DIALOG", true).Commit();
                    ShowDebriefingDialog();
                }
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
            alert.SetTitle(StringResources.debriefing_ui_page_first_title);
            alert.SetMessage(StringResources.debriefing_ui_page_first_content);

            alert.SetNeutralButton(
                StringResources.debriefing_ui_button_hide, 
                (dialog, id) => ((AlertDialog)dialog).Dismiss()
            );

            alert.SetPositiveButton(
                StringResources.debriefing_ui_button_upload, 
                async (dialog, id) => {
                    ((AlertDialog)dialog).Dismiss();
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
            var didUpload = await new RestClient().Upload(adapter.Sessions[index]);

            if (didUpload)
            {
                adapter.SessionIsUploaded(index);
                Toast.MakeText(Activity, StringResources.sessions_ui_message_upload_success, ToastLength.Long).Show();
                if (recursive) await UploadSessions(0, true);
            }
            else
            {
                adapter.SessionUploadFail(index);
                Toast.MakeText(Activity, StringResources.sessions_ui_message_upload_fail, ToastLength.Long).Show();
            }
            sessionsUploadButton.Enabled = true;
            ShowHideInstructions();
            return true;
        }
    }
}
