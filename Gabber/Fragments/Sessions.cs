using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabber.Adapters;
using GabberPCL;
using GabberPCL.Models;
using GabberPCL.Resources;

namespace Gabber.Fragments
{
    public class Sessions : Android.Support.V4.App.Fragment
    {
        static Sessions instance;
        SessionAdapter adapter;

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
            sessions = sessions.FindAll(t => !t.IsUploaded);
            adapter = new Adapters.SessionAdapter(sessions);
            Activity.FindViewById<RecyclerView>(Resource.Id.sessions).SetAdapter(adapter);

            var sessionsUploadButton = Activity.FindViewById<AppCompatButton>(Resource.Id.upload_sessions);

            ShowHideInstructions();

            sessionsUploadButton.Click += async delegate {
                System.Console.WriteLine("ABOUT TO DISABLE IT...");
                sessionsUploadButton.Enabled = false;
                await UploadSessions(0, true);
                System.Console.WriteLine("ABOUT TO ENABLE IT...");
                sessionsUploadButton.Enabled = true;
            };

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
            ShowHideInstructions();
            return true;
        }
    }
}
