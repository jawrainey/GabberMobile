using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using GabberPCL.Models;
using GabberPCL.Resources;

namespace Gabber.Activities
{
    [Activity]
    public class Onboarding : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.onboarding);

            var pager = FindViewById<ViewPager>(Resource.Id.pager);

            var pages = new List<OnboardingPageContent> {
                new OnboardingPageContent {
                    Image=Resource.Drawable.onboarding_second,
                    Title=StringResources.onboarding_ui_page_first_title,
                    Content=StringResources.onboarding_ui_page_first_content
                },
                new OnboardingPageContent {
                    Image=Resource.Drawable.onboarding_first,
                    Title=StringResources.onboarding_ui_page_second_title,
                    Content=StringResources.onboarding_ui_page_second_content
                },
                new OnboardingPageContent {
                    Image=Resource.Drawable.onboarding_third,
                    Title=StringResources.onboarding_ui_page_third_title,
                    Content=StringResources.onboarding_ui_page_third_content
                },
                new OnboardingPageContent {
                    Image=Resource.Drawable.onboarding_third,
                    Title=StringResources.onboarding_ui_page_fourth_title,
                    Content=StringResources.onboarding_ui_page_fourth_content
                }
            };
            pager.Adapter = new Adapters.SharedPager(this, pages);

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
    }
}
