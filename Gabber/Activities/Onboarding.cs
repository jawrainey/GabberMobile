using System.Collections.Generic;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views.Animations;
using Android.Widget;
using Firebase.Analytics;
using Gabber.Helpers;
using GabberPCL.Models;
using GabberPCL.Resources;

namespace Gabber.Activities
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MyTheme.NoActionBar")]
    public class Onboarding : AppCompatActivity, ViewPager.IOnPageChangeListener
    {
        FirebaseAnalytics firebaseAnalytics;
        ViewPager pager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            firebaseAnalytics = FirebaseAnalytics.GetInstance(this);
            base.OnCreate(savedInstanceState);
            Localise.SetLayoutDirectionByCulture(this);
            SetContentView(Resource.Layout.onboarding);

            pager = FindViewById<ViewPager>(Resource.Id.pager);

            var pages = new List<OnboardingPageContent> {
                new OnboardingPageContent {
                    Image=Resource.Drawable.AppLogo,
                    Title=StringResources.onboarding_ui_page_zero_title,
                    Content=StringResources.onboarding_ui_page_zero_content
                },
                new OnboardingPageContent {
                    Image=Resource.Drawable.onboarding_02,
                    Title=StringResources.onboarding_ui_page_first_title,
                    Content=StringResources.onboarding_ui_page_first_content
                },
                new OnboardingPageContent {
                    Image=Resource.Drawable.onboarding_03,
                    Title=StringResources.onboarding_ui_page_second_title,
                    Content=StringResources.onboarding_ui_page_second_content
                },
                new OnboardingPageContent {
                    Image=Resource.Drawable.onboarding_04,
                    Title=StringResources.onboarding_ui_page_third_title,
                    Content=StringResources.onboarding_ui_page_third_content
                },
                new OnboardingPageContent {
                    Image=Resource.Drawable.onboarding_05,
                    Title=StringResources.onboarding_ui_page_fourth_title,
                    Content=StringResources.onboarding_ui_page_fourth_content
                }
            };
            pager.Adapter = new Adapters.SharedPager(this, pages);
            pager.AddOnPageChangeListener(this);

            var tabs = FindViewById<TabLayout>(Resource.Id.tabs);
            tabs.SetupWithViewPager(pager, true);
            tabs.SetSelectedTabIndicatorHeight(0);

            var login = FindViewById<AppCompatButton>(Resource.Id.login);
            login.Text = StringResources.login_ui_submit_button;
            login.Click += (sender, e) => StartActivity(typeof(LoginActivity));

            var register = FindViewById<AppCompatButton>(Resource.Id.register);
            register.Text = StringResources.register_ui_submit_button;
            register.Click += (sender, e) => StartActivity(typeof(RegisterActivity));
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels) { }

        public void OnPageScrollStateChanged(int state) { }

        public void OnPageSelected(int position)
        {
            LOG_ONBOARDING_SWIPE(position);
            var la = FindViewById<LinearLayout>(Resource.Id.showAuthButtons);

            if (position == pager.Adapter.Count - 1)
            {
                la.StartAnimation(AnimationUtils.LoadAnimation(this, Resource.Drawable.slideup));
                la.Visibility = Android.Views.ViewStates.Visible;
            }
            else
            {
                la.Visibility = Android.Views.ViewStates.Gone;
            }
        }

        void LOG_ONBOARDING_SWIPE(int position)
        {
            var bundle = new Bundle();
            bundle.PutInt("POSITION", position);
            bundle.PutString("TIMESTAMP", System.DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());
            firebaseAnalytics.LogEvent("ONBOARDING_SWIPE", bundle);
        }
    }
}