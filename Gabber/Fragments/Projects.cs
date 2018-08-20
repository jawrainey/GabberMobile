using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Firebase.Analytics;
using Gabber.Adapters;
using GabberPCL;
using GabberPCL.Models;
using GabberPCL.Resources;

namespace Gabber.Fragments
{
    public class Projects : Android.Support.V4.App.Fragment
    {
		private FirebaseAnalytics firebaseAnalytics;
        private List<Project> _projects;
        private SwipeRefreshLayout refresher;
        // Made availiable to update projects on data load.
        private ProjectsAdapter adapter;
        // Prevents multiple calls being made to the API when one is in progress.
        private Task LoadingProjects;
        // One instance to rule them all
        static Projects instance;

        public static Projects NewInstance()
        {
            if (instance == null) instance = new Projects { Arguments = new Bundle() };
            return instance;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var rootView = inflater.Inflate(Resource.Layout.projects_frag, null);
            var projects = rootView.FindViewById<RecyclerView>(Resource.Id.projects);
            projects.SetLayoutManager(new LinearLayoutManager(Activity));

            var instructions = rootView.FindViewById<TextView>(Resource.Id.projectInstructions);
            instructions.Text = StringResources.projects_ui_instructions;

            var toolbar = rootView.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            ((AppCompatActivity)Activity).SetSupportActionBar(toolbar);
            ((AppCompatActivity)Activity).SupportActionBar.Title = StringResources.projects_ui_title;

            return rootView;
        }

		public override void OnActivityCreated(Bundle savedInstanceState)
        {
			firebaseAnalytics = FirebaseAnalytics.GetInstance(Context);

            base.OnCreate(savedInstanceState);
             _projects = Queries.AllProjects();

            // It is not possible to SetAdapter in OnActivityCreated as accessing the rootView.
            adapter = new ProjectsAdapter(_projects);
            adapter.ProjectClicked += OnProjectClick;
            Activity.FindViewById<RecyclerView>(Resource.Id.projects).SetAdapter(adapter);
            
            refresher = Activity.FindViewById<SwipeRefreshLayout>(Resource.Id.projectsRefresher);
            refresher.SetColorSchemeResources(Resource.Color.primary_material_dark);
            refresher.Refresh += Refresher_Refresh;

            // As this method is called each time the Fragment is in view (similar to onResume),
            // only call it if there are no projects, i.e. on first use.
            if (_projects.Count <= 0) LoadDataIfNotLoading();
        }

        private void Refresher_Refresh(object sender, System.EventArgs e)
        {
            LoadDataIfNotLoading();
        }

        public async Task LoadData()
        {
            refresher.Refreshing = true;

            //LOG_SWIPE_REFRESH();
            List<Project> response = await RestClient.GetProjects(ErrorDelegate);

            refresher.Refreshing = false;

            if (response != null && response.Count > 0)
            {
                Queries.AddProjects(response);
                _projects = response;
                adapter.UpdateProjects(_projects);
            }
        }

        private void LoadDataIfNotLoading()
        {
            if (LoadingProjects == null || LoadingProjects.IsCompleted)
            {
                LoadingProjects = LoadData();
            }
        }

        private void ErrorDelegate(string errorMessage)
        {
            Toast.MakeText(Activity, errorMessage, ToastLength.Long).Show();
        }

        void OnProjectClick(object sender, int position)
        {
            var _prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
			LOG_SELECTED_PROJECT(position);
            var intent = new Intent(Activity.ApplicationContext, typeof(PreparationActivity));
            // The unique ID used to lookup associated prompts (URLs and text).
            _prefs.Edit().PutInt("SelectedProjectID", _projects[position].ID).Commit();
            StartActivity(intent);
        }
      
		private void LOG_SWIPE_REFRESH()
        {
            var bundle = new Bundle();
			bundle.PutInt("PROJECT_COUNT", _projects.Count);
			firebaseAnalytics.LogEvent("SWIPE_REFRESH", bundle);
        }

		private void LOG_SELECTED_PROJECT(int position)
		{
			var bundle = new Bundle();
            bundle.PutString("PROJECT", _projects[position].Title);
			bundle.PutString("USER", Session.ActiveUser.Email);
			firebaseAnalytics.LogEvent("PROJECT_SELECTED", bundle);
		}
    }
}
