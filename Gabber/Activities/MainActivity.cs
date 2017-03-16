using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Newtonsoft.Json;
using System.Collections.Generic;
using GabberPCL;

namespace Gabber
{
	[Activity(MainLauncher=true)]
	public class MainActivity : AppCompatActivity
	{
		// A project commissioned through the web-service.
		List<Project> _projects;
		// To faciliate access in OnProjectClick
		ISharedPreferences _prefs;
		// Access local Gabber details on project click
		DatabaseManager _model;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			// Used to display interviews that the logged in user has recorded
			// This ensures if many log into the device, then they will see theirs.
			_prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);

			// Used to redirect unauthenticated users
			if (string.IsNullOrWhiteSpace(_prefs.GetString("username", "")))
			{
				StartActivity(typeof(LoginActivity));
				Finish();
			}
			else
			{
				_model = new DatabaseManager(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal));

				SetupProjects(_model);
			}

			// Register the implementation to the global interface within the PCL.
			RestClient.GlobalIO = new DiskIO();

			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.main);
			SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));
			SupportActionBar.Title = Resources.GetText(Resource.String.select_project);

			var mView = FindViewById<RecyclerView>(Resource.Id.projects);
			mView.SetLayoutManager(new LinearLayoutManager(this));

			var mAdapter = new RecyclerAdapter(_projects);
			mAdapter.ProjectClicked += OnProjectClick;
			mView.SetAdapter(mAdapter);
		}

		void SetupProjects(DatabaseManager model)
		{
			// Create this once so the async call are run queued properly
			// TODO: filter to show user-associated projects first.
			// TODO: have a spinning animation whilst we wait for it to load?
			// TODO: refresh when they go off-line and come back online.

			// The entire request, which will be stored in the database
			var response = new RestClient().GetProjects();
			_projects = response;

			// If there are no results [e.g. no Internet], then use cached version.
			// Otherwise update our data. Since we will get all in a request, just update.
			// TODO: what if there is no cached version?
			if (_projects.Count == 0) _projects = model.GetProjects();
			else model.SaveRequest(JsonConvert.SerializeObject(response));
		}

		void OnProjectClick(object sender, int position)
		{
			foreach (var gabber in _model.GetStories(_prefs.GetString("username", "")))
			{
				if (!gabber.Uploaded)
				{
					RunOnUiThread(async () =>
					{
						if (await new RestClient().Upload(gabber))
						{
							gabber.Uploaded = true;
							_model.UpdateStory(gabber);
						}
					});
				}
			}

			var intent = new Intent(this, typeof(PreparationActivity));
			// The unique ID used to lookup associated prompts (URLs and text).
			// Storing as pref as access is required 
			_prefs.Edit().PutString("theme", _projects[position].theme).Commit();
			StartActivity(intent);
		}
	}
}