using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using GabberPCL;
using GabberPCL.Resources;

namespace Gabber.Fragments
{
    public class Sessions : Android.Support.V4.App.Fragment
    {
        static Sessions instance;

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
                
            var toolbar = rootView.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            ((AppCompatActivity)Activity).SetSupportActionBar(toolbar);
            ((AppCompatActivity)Activity).SupportActionBar.Title = StringResources.sessions_ui_title;

            return rootView;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var sessions = Queries.AllNotUploadedInterviewSessionsForActiveUser();
            var adapter = new Adapters.SessionAdapter(sessions);
            var sessionsView = Activity.FindViewById<RecyclerView>(Resource.Id.sessions);

            sessionsView.SetAdapter(adapter);

            var sessions_upload = Activity.FindViewById<AppCompatButton>(Resource.Id.upload_sessions);
            var sessions_to_upload = sessions.FindAll(t => !t.IsUploaded).ToArray();

            if (sessions_to_upload.Length > 0) {
                sessions_upload.Visibility = ViewStates.Visible;
                Activity.FindViewById<TextView>(Resource.Id.sessionsBodyInstructions).Visibility = ViewStates.Gone;
            }

            sessions_upload.Click += async delegate
            {
                if (sessions_to_upload.Length <= 0) return;

                for (int i = 0; i < sessions.Count; i++)
                {
                    var session = sessions[i];
                    if (!session.IsUploaded)
                    {
                        // Changes the UI element to a ProgressBar
                        adapter.SessionIsUploading(i);
                        sessions_upload.Enabled = false;

                        var didUpload = await new RestClient().Upload(session);

                        // Changes UI depending on response.
                        if (didUpload)
                        {
                            session.IsUploaded = didUpload;
                            Session.Connection.Update(session);
                            adapter.SessionIsUploaded(i);
                            Toast.MakeText(Activity, StringResources.sessions_ui_message_upload_success, ToastLength.Long).Show();
                        }
                        else
                        {
                            adapter.SessionUploadFail(i);
                            Toast.MakeText(Activity, StringResources.sessions_ui_message_upload_fail, ToastLength.Long).Show();
                        }
                    }
                }
                if (Queries.AllNotUploadedInterviewSessionsForActiveUser().Count <= 0) {
                    sessions_upload.Visibility = ViewStates.Invisible;
                    Activity.FindViewById<TextView>(Resource.Id.sessionsBodyInstructions).Visibility = ViewStates.Visible;
                }
                sessions_upload.Enabled = true;
            };

        }
    }
}
