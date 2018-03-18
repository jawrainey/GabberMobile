using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Widget;
using GabberPCL;

namespace Gabber.Activities
{
    [Activity]
    public class Sessions : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.sessions);
            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar));
            var sessions = Queries.AllInterviewSessionsForActiveUser();
            SupportActionBar.Title = $"Your sessions ({sessions.Count})";

            var sessionsView = FindViewById<RecyclerView>(Resource.Id.sessions);
            sessionsView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Vertical, false));

            var adapter = new SessionAdapter(sessions);
            sessionsView.SetAdapter(adapter);

            var sessions_upload = FindViewById<AppCompatButton>(Resource.Id.upload_sessions);
            var sessions_to_upload = sessions.FindAll(t => !t.IsUploaded).ToArray();
            if (sessions_to_upload.Length <= 0) sessions_upload.Enabled = false;

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
                            Snackbar.Make(sessionsView, "Session uploaded successfully", 0).Show();
                        }
                        else 
                        {
                            adapter.SessionUploadFail(i);
                            Snackbar.Make(sessionsView, "Failed to upload. Try again soon.", 0).Show();
                        }
                    }
                }
                sessions_upload.Enabled = true;
            };

        }    
    }
}