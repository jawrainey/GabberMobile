using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gabber
{
	[Activity(MainLauncher=true)]
	public class MainActivity : AppCompatActivity
	{
		// A project commissioned through the web-service.
		List<Project> _projects;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			// Used to display interviews that the logged in user has recorded
			// This ensures if many log into the device, then they will see theirs.
			var prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);

			// Used to redirect unauthenticated users
			if (string.IsNullOrWhiteSpace(prefs.GetString("username", "")))
			{
				StartActivity(typeof(HomeActivity));
				Finish();
			}

			SetupProjects();

			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.main);

			RecyclerView mView = FindViewById<RecyclerView>(Resource.Id.projects);
			mView.SetLayoutManager(new LinearLayoutManager(this));

			var mAdapter = new RecyclerAdapter(_projects);
			mAdapter.ProjectClicked += OnProjectClick;
			mView.SetAdapter(mAdapter);
		}

		void SetupProjects()
		{
			// Create this once so the async call are run queued properly
			// TODO: filter to show user-associated projects first.
			// TODO: have a spinning animation whilst we wait for it to load?
			// TODO: refresh when they go off-line and come back online.

			// The entire request, which will be stored in the database
			var response = new RestAPI().GetProjects().Result;
			_projects = response.projects;
			var model = new Model();

			// If there are no results [e.g. no Internet], then use cached version.
			if (_projects.Count == 0)
			{
				_projects = model.GetProjects();
			}
			else
			{
				// Otherwise update our data. Since we will get all in a request, just update.
				model.SaveRequest(JsonConvert.SerializeObject(response));
			}
		}

		void OnProjectClick(object sender, int position)
		{
			var intent = new Intent(this, typeof(PreparationActivity));
			// The unique ID used to lookup associated prompts (URLs and text).
			intent.PutExtra("theme", _projects[position].theme);
			StartActivity(intent);
		}
	}
}