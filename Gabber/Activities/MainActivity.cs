using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.Text.Emoji;
using Android.Support.Text.Emoji.Bundled;
using Android.Support.V7.App;
using Android.Views;
using Firebase.Analytics;
using Gabber.Fragments;
using Gabber.Helpers;
using GabberPCL;
using GabberPCL.Models;
using GabberPCL.Resources;
using Newtonsoft.Json;

namespace Gabber
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MyTheme")]
    public class MainActivity : AppCompatActivity
    {
        public static FirebaseAnalytics FireBaseAnalytics;
        BottomNavigationView nav;

        PrefsFragment prefsFragment;
        ProjectsFragment projectsFragment;
        UploadsFragment sessionsFragment;
        Android.Support.V4.App.Fragment activeFragment;
        List<LanguageChoice> SupportedLanguages;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.main);
            SupportActionBar.Title = StringResources.projects_ui_title;
            EmojiCompat.Init(new BundledEmojiCompatConfig(this));
            
            // Create the active user anytime they reopen app
            if (Session.ActiveUser == null)
            {
                var preferences = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
                var user = Queries.UserByEmail(preferences.GetString("username", ""));
                var tokens = JsonConvert.DeserializeObject<JWToken>(preferences.GetString("tokens", ""));
                Queries.SetActiveUser(new DataUserTokens { User = user, Tokens = tokens });
                FireBaseAnalytics.SetUserId(Session.ActiveUser.Id.ToString());
                Session.ActiveUser.AppLang = user.AppLang;
            }

            prefsFragment = new PrefsFragment();
            sessionsFragment = new UploadsFragment();
            projectsFragment = new ProjectsFragment();
            activeFragment = projectsFragment;

            SupportFragmentManager.BeginTransaction().Add(Resource.Id.content_frame, prefsFragment, "settings").Hide(prefsFragment).Commit();
            SupportFragmentManager.BeginTransaction().Add(Resource.Id.content_frame, sessionsFragment, "sessions").Hide(sessionsFragment).Commit();
            SupportFragmentManager.BeginTransaction().Add(Resource.Id.content_frame, projectsFragment, "projects").Commit();

            var suppressAsync = GetLangData();
            LoadUploadFragmentAfterSession();
            SupportActionBar.Title = StringResources.login_ui_title;
        }

        private async Task GetLangData()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            ProgressDialog progress = ProgressDialog.Show(this, null, StringResources.common_comms_loading);
#pragma warning restore CS0618 // Type or member is obsolete

            SupportedLanguages = (await LanguageChoiceManager.GetLanguageChoices()).OrderBy((lang) => lang.Code).ToList();

            // First time users logs in, set the language to their culture if we support it, or English.
            if (Session.ActiveUser.AppLang == 0)
            {
                var currentMobileLang = Localise.GetCurrentCultureInfo().TwoLetterISOLanguageName;
                var isSupportedLang = SupportedLanguages.FirstOrDefault((lang) => lang.Code == currentMobileLang);
                Session.ActiveUser.AppLang = isSupportedLang != null ? isSupportedLang.Id : 1;
                // This will save the choice for future: reopening app, other activities, etc.
                Queries.SaveActiveUser();
            }

            var found = SupportedLanguages.Find((lang) => lang.Id == Session.ActiveUser.AppLang);
            StringResources.Culture = new CultureInfo(found.Code);
            Localise.SetLocale(StringResources.Culture);

            SetLayoutDirection();

            nav = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);
            LoadNavigationTitles();
            nav.NavigationItemSelected += NavigationItemSelected;

            LanguageChoiceManager.RefreshIfNeeded();

            progress.Dismiss();
        }

        public void SetLayoutDirection()
        {
            var found = SupportedLanguages.Find((lang) => lang.Id == Session.ActiveUser.AppLang);
            Window.DecorView.LayoutDirection = found.Code == "ar" ? LayoutDirection.Rtl : LayoutDirection.Ltr;
        }

        public void RefreshFragments()
        {
            // This is required when changing the language from PrefsFragment to update the text across fragments.
            SupportFragmentManager.BeginTransaction().Detach(prefsFragment).Attach(prefsFragment).Commit();
            SupportFragmentManager.BeginTransaction().Detach(sessionsFragment).Attach(sessionsFragment).Commit();
            SupportFragmentManager.BeginTransaction().Detach(projectsFragment).Attach(projectsFragment).Commit();
        }

        void LoadUploadFragmentAfterSession()
        {
            if (!string.IsNullOrWhiteSpace(Intent.GetStringExtra("FRAGMENT_TO_SHOW")))
            {
                SupportFragmentManager.BeginTransaction().Hide(activeFragment).Show(sessionsFragment).Commit();
                nav.SelectedItemId = Resource.Id.menu_gabbers;
                activeFragment = sessionsFragment;
                LOG_FRAGMENT_SELECTED("uploads");
            }
        }

        public void LoadNavigationTitles()
        {
            nav.Menu.FindItem(Resource.Id.menu_projects).SetTitle(StringResources.common_menu_projects);
            nav.Menu.FindItem(Resource.Id.menu_gabbers).SetTitle(StringResources.common_menu_gabbers);
            nav.Menu.FindItem(Resource.Id.menu_settings).SetTitle(StringResources.common_menu_settings);

            int selectedTabId = nav.SelectedItemId;

            switch (nav.SelectedItemId)
            {
                case Resource.Id.menu_projects:
                    SupportActionBar.Title = StringResources.projects_ui_title;
                    break;
                case Resource.Id.menu_gabbers:
                    SupportActionBar.Title = StringResources.sessions_ui_title;
                    break;
                case Resource.Id.menu_settings:
                    SupportActionBar.Title = StringResources.common_menu_settings;
                    break;
            }
        }

        private void NavigationItemSelected(object sender, BottomNavigationView.NavigationItemSelectedEventArgs e)
        {
            Android.Support.V4.App.Fragment toShow = null;

            switch (e.Item.ItemId)
            {
                case Resource.Id.menu_projects:
                    SupportActionBar.Title = StringResources.projects_ui_title;
                    toShow = projectsFragment;
                    LOG_FRAGMENT_SELECTED("projects");
                    break;
                case Resource.Id.menu_gabbers:
                    SupportActionBar.Title = StringResources.sessions_ui_title;
                    toShow = sessionsFragment;
                    LOG_FRAGMENT_SELECTED("uploads");
                    break;
                case Resource.Id.menu_settings:
                    SupportActionBar.Title = StringResources.common_menu_settings;
                    toShow = prefsFragment;
                    LOG_FRAGMENT_SELECTED("settings");
                    break;
                default:
                    toShow = projectsFragment;
                    break;
            }

            SupportFragmentManager.BeginTransaction().Hide(activeFragment).Show(toShow).Commit();
            activeFragment = toShow;
        }

        private void LOG_FRAGMENT_SELECTED(string name)
        {
            var bundle = new Bundle();
            bundle.PutString("FRAGMENT", name);
            FireBaseAnalytics.LogEvent("FRAGMENT_SHOWN", bundle);
        }

        public void LogOut()
        {
            // reset all data stored in prefs
            var prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            prefs.Edit().Clear().Commit();

            // Make sure the projects fragment pulls from the server when we log back in 
            ProjectsFragment.HasRefreshedProjects = false;

            // reset to system language
            StringResources.Culture = Localise.GetCurrentCultureInfo();

            // nuke the database
            Session.NukeItFromOrbit();

            //return to login
            GoToOnboarding();
        }

        private void GoToOnboarding()
        {
            // We must clear the navigation stack here otherwise this activity is behind onboarding.
            var intent = new Intent(this, typeof(Activities.Onboarding));
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.ClearTask);
            StartActivity(intent);
            Finish();
        }
    }
}
