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

            var toolbar = rootView.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            ((AppCompatActivity)Activity).SetSupportActionBar(toolbar);
            ((AppCompatActivity)Activity).SupportActionBar.Title = Resources.GetText(Resource.String.select_project);

            return rootView;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _projects = Queries.AllProjects();
            adapter = new ProjectsAdapter(_projects);
            adapter.ProjectClicked += OnProjectClick;
            Activity.FindViewById<RecyclerView>(Resource.Id.projects).SetAdapter(adapter);

            LoadDataIfNotLoading();

            var refresher = Activity.FindViewById<SwipeRefreshLayout>(Resource.Id.projectsRefresher);
            refresher.SetColorSchemeResources(Resource.Color.primary_material_dark);
            refresher.Refresh += async delegate {
                await LoadData();
                refresher.Refreshing = false;
            };
        }

        public override void OnResume()
        {
            base.OnResume();

            if (LoadingProjects == null || LoadingProjects.IsCompleted)
            {
                var progressBar = Activity.FindViewById<ProgressBar>(Resource.Id.progressBar);
                progressBar.Visibility = ViewStates.Visible;
                LoadingProjects = LoadData();
                progressBar.Visibility = ViewStates.Gone;
            }
        }

        public async Task LoadData()
        {
            var response = await new RestClient().GetProjects(ShowErrorMessage);

            if (response.Count > 0)
            {
                Queries.AddProjects(response);
                _projects = response;
                adapter.UpdateProjects(_projects);
            }
        }

        public void LoadDataIfNotLoading()
        {
            if (LoadingProjects == null || LoadingProjects.IsCompleted)
            {
                var progressBar = Activity.FindViewById<ProgressBar>(Resource.Id.progressBar);
                progressBar.Visibility = ViewStates.Visible;
                LoadingProjects = LoadData();
                progressBar.Visibility = ViewStates.Gone;
            }
        }

        void ShowErrorMessage(string errorMessage)
        {
            Toast.MakeText(Activity, errorMessage, ToastLength.Long).Show();
        }

        void UploadGabbers()
        {
            foreach (var InterviewSession in Queries.AllInterviewSessionsForActiveUser())
            {
                if (!InterviewSession.IsUploaded)
                {
                    Activity.RunOnUiThread(async () =>
                    {
                        if (await new RestClient().Upload(InterviewSession))
                        {
                            InterviewSession.IsUploaded = true;
                            Session.Connection.Update(InterviewSession);
                            Snackbar.Make(Activity.FindViewById<RecyclerView>(Resource.Id.projects), "Gabber uploaded successfully", 0).Show();
                        }
                    });
                }
            }
        }

        void OnProjectClick(object sender, int position)
        {
            UploadGabbers();
            var _prefs = PreferenceManager.GetDefaultSharedPreferences(Activity);
            var intent = new Intent(Activity.ApplicationContext, typeof(PreparationActivity));
            // The unique ID used to lookup associated prompts (URLs and text).
            _prefs.Edit().PutInt("SelectedProjectID", _projects[position].ID).Commit();
            StartActivity(intent);
        }
    }
}
