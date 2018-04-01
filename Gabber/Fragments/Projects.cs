using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
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
    public class Projects : Android.Support.V4.App.Fragment
    {
        List<Project> _projects;
        // Made availiable to update projects on data load.
        ProjectsAdapter adapter;
        // Prevents multiple calls being made to the API when one is in progress.
        Task LoadingProjects;
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
            base.OnCreate(savedInstanceState);
             _projects = Queries.AllProjects();

            // It is not possible to SetAdapter in OnActivityCreated as accessing the rootView.
            adapter = new ProjectsAdapter(_projects);
            adapter.ProjectClicked += OnProjectClick;
            Activity.FindViewById<RecyclerView>(Resource.Id.projects).SetAdapter(adapter);
            // As this method is called each time the Fragment is in view (similar to onResume),
            // only call it if there are no projects, i.e. on first use.
            if (_projects.Count <= 0) LoadDataIfNotLoading(true);

            var refresher = Activity.FindViewById<SwipeRefreshLayout>(Resource.Id.projectsRefresher);
            refresher.SetColorSchemeResources(Resource.Color.primary_material_dark);
            refresher.Refresh += async delegate {
                await LoadData();
                refresher.Refreshing = false;
            };
        }

        public async Task LoadData(bool withLoadingBar=false)
        {
            var progressBar = Activity.FindViewById<RelativeLayout>(Resource.Id.progressBarLayout);
            if (withLoadingBar) progressBar.Visibility = ViewStates.Visible;
            var response = await new RestClient().GetProjects(ShowErrorMessage);
            if (withLoadingBar) progressBar.Visibility = ViewStates.Gone;

            if (response.Count > 0)
            {
                Queries.AddProjects(response);
                _projects = response;
                adapter.UpdateProjects(_projects);
            }
        }

        public void LoadDataIfNotLoading(bool withLoadingBar=false)
        {
            if (LoadingProjects == null || LoadingProjects.IsCompleted)
            {
                LoadingProjects = LoadData(withLoadingBar);
            }
        }

        void ShowErrorMessage(string errorMessage) => Toast.MakeText(Activity, errorMessage, ToastLength.Long).Show();

        void OnProjectClick(object sender, int position)
        {
            var _prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            var intent = new Intent(Activity.ApplicationContext, typeof(PreparationActivity));
            // The unique ID used to lookup associated prompts (URLs and text).
            _prefs.Edit().PutInt("SelectedProjectID", _projects[position].ID).Commit();
            StartActivity(intent);
        }
    }
}
