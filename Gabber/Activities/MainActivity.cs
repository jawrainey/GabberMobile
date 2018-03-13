using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using GabberPCL;
using System.Linq;
using Android.Support.Design.Widget;
using GabberPCL.Models;
using Gabber.Helpers;
using Newtonsoft.Json;
using Android.Views;
using Android.Widget;

namespace Gabber
{
	[Activity(MainLauncher=true)]
	public class MainActivity : AppCompatActivity
	{
		// A project commissioned through the web-service.
		List<Project> _projects;
		// To faciliate access in OnProjectClick
		ISharedPreferences _prefs;

		protected async override void OnCreate(Bundle savedInstanceState)
		{
			// Used to display interviews that the logged in user has recorded
			// This ensures if many log into the device, then they will see theirs.
			_prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);

            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.main);
            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar));
            SupportActionBar.Title = Resources.GetText(Resource.String.select_project);

            var mView = FindViewById<RecyclerView>(Resource.Id.projects);
            mView.SetLayoutManager(new LinearLayoutManager(this));

            // Used by the PCL for database interactions so must be defined early.
            Session.PrivatePath = new PrivatePath();
            // Register the implementation to the global interface within the PCL.
            RestClient.GlobalIO = new DiskIO();

			// Used to redirect unauthenticated users
			if (string.IsNullOrWhiteSpace(_prefs.GetString("username", "")))
			{
				StartActivity(typeof(LoginActivity));
				Finish();
			}
			else
			{
                if (Session.ActiveUser == null)
                {
                    // Although this is set on Login/Register, it is forgotten when the app is closed.
                    Session.ActiveUser = Queries.FindOrInsertUser(_prefs.GetString("username", ""));
                    Session.ActiveUser.IsActive = true;
                    Session.Token = JsonConvert.DeserializeObject<JWToken>(_prefs.GetString("tokens", ""));
                }

                FindViewById<ProgressBar>(Resource.Id.progressBar).Visibility = ViewStates.Visible;
                var api = new RestClient();
                var response = await api.GetProjects((errorMessage) => Snackbar.Make(mView, errorMessage, 0).Show());
                _projects = response;
                FindViewById<ProgressBar>(Resource.Id.progressBar).Visibility = ViewStates.Gone;

                // If there are no results [e.g. no Internet], then use cached version.
                // Otherwise update our data. Since we will get all in a request, just update.
                // TODO: what if there is no cached version?
                if (_projects.Count == 0) _projects = Queries.AllProjects();
                else Queries.AddProjects(response);

                // An interview session was just completed, so we want to create a message
                // to inform the user that the session will now be uploaded.
                if (!string.IsNullOrWhiteSpace(Intent.GetStringExtra("RECORDED_GABBER")))
                {
                    // For active user as a user may log in to another persons device, unlikely, but possible.
                    var interviews = Queries.AllInterviewSessionsForActiveUser();
                    int numToUpload = interviews.Count - interviews.Count((interview) => interview.IsUploaded);
                    var snackText = "You have " + numToUpload.ToString() + " Gabbers to upload";
                    Snackbar.Make(mView, snackText, Snackbar.LengthIndefinite)
                            .SetAction("Upload", (view) => Queries.UploadInterviewSessionsAsync())
                            .Show();
                }
			}
			var mAdapter = new RecyclerAdapter(_projects);
			mAdapter.ProjectClicked += OnProjectClick;
			mView.SetAdapter(mAdapter);
		}

        void UploadGabbers()
        {
            foreach (var InterviewSession in Queries.AllInterviewSessionsForActiveUser())
            {
                if (!InterviewSession.IsUploaded)
                {
                    RunOnUiThread(async () =>
                    {
                        if (await new RestClient().Upload(InterviewSession))
                        {
                            InterviewSession.IsUploaded = true;
                            Session.Connection.Update(InterviewSession);
                            Snackbar.Make(FindViewById<RecyclerView>(Resource.Id.projects), "Gabber uploaded successfully", 0).Show();
                        }
                    });
                }
            }
        }

		void OnProjectClick(object sender, int position)
		{
            UploadGabbers();
			var intent = new Intent(this, typeof(PreparationActivity));
			// The unique ID used to lookup associated prompts (URLs and text).
            _prefs.Edit().PutInt("SelectedProjectID", _projects[position].ID).Commit();
			StartActivity(intent);
		}
	}
}