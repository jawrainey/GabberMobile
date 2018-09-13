using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Analytics;
using Gabber.Adapters;
using GabberPCL;
using GabberPCL.Models;
using GabberPCL.Resources;

namespace Gabber.Fragments
{
    public class ProjectsFragment : Android.Support.V4.App.Fragment
    {
        private FirebaseAnalytics firebaseAnalytics;
        private List<Project> _projects;
        private SwipeRefreshLayout refresher;
        private ProjectsAdapter adapter;
        private ExpandableListView listView;
        // Prevents multiple calls being made to the API when one is in progress.
        private Task LoadingProjects;
        // One instance to rule them all
        static ProjectsFragment instance;

        public static bool HasRefreshedProjects;

        public static ProjectsFragment NewInstance()
        {
            if (instance == null) instance = new ProjectsFragment { Arguments = new Bundle() };
            return instance;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var rootView = inflater.Inflate(Resource.Layout.projects_frag, null);

            var instructions = rootView.FindViewById<TextView>(Resource.Id.projectInstructions);
            instructions.Text = StringResources.projects_ui_instructions;
            listView = rootView.FindViewById<ExpandableListView>(Resource.Id.projects);

            ((AppCompatActivity)Activity).SupportActionBar.Title = StringResources.projects_ui_title;

            return rootView;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            DisplayMetrics displaymetrics = new DisplayMetrics();
            Activity.WindowManager.DefaultDisplay.GetMetrics(displaymetrics);
            int screenWidth = displaymetrics.WidthPixels;

            int leftPadding = IsRTL() ? -70 : 70;
            int rightPadding = IsRTL() ? 30 : -115;
            var width = DpToPx(302);
            int leftWidth = width - DpToPx(leftPadding);
            int rightWidth = width - DpToPx(rightPadding);

            if (Build.VERSION.SdkInt < BuildVersionCodes.JellyBeanMr2)
            {
                listView.SetIndicatorBounds(leftWidth, rightWidth);
            }
            else
            {
                listView.SetIndicatorBoundsRelative(leftWidth, rightWidth);
            }
        }

        public bool IsRTL() => Activity.Window.DecorView.LayoutDirection == Android.Views.LayoutDirection.Rtl;

        public int DpToPx(int dp) => (int)((dp * Context.Resources.DisplayMetrics.Density) + 0.5);

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
        }

        public override void OnStart()
        {
            base.OnStart();

            firebaseAnalytics = FirebaseAnalytics.GetInstance(Context);

            _projects = Queries.AllProjects();

            // It is not possible to SetAdapter in OnActivityCreated as accessing the rootView.
            adapter = new ProjectsAdapter(_projects, Context);
            adapter.ProjectClicked += OnProjectClick;
            listView.SetAdapter(adapter);

            refresher = Activity.FindViewById<SwipeRefreshLayout>(Resource.Id.projectsRefresher);
            refresher.SetColorSchemeResources(Resource.Color.primary_material_dark);
            refresher.Refresh += Refresher_Refresh;

            if (!HasRefreshedProjects) LoadDataIfNotLoading();
        }

        private void Refresher_Refresh(object sender, System.EventArgs e)
        {
            LoadDataIfNotLoading();
        }

        public async Task LoadData()
        {
            refresher.Refreshing = true;

            List<Project> response = await RestClient.GetProjects(ErrorDelegate);

            if (response != null && response.Count > 0)
            {
                Queries.AddProjects(response);
                _projects = response;
                adapter.UpdateProjects(_projects);
            }

            refresher.Refreshing = false;
            HasRefreshedProjects = true;
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
            var project = Helpers.Localise.ContentByLanguage(_projects[position]);
            bundle.PutString("PROJECT", project.Title);
            bundle.PutString("USER", Session.ActiveUser.Email);
            firebaseAnalytics.LogEvent("PROJECT_SELECTED", bundle);
        }
    }
}
