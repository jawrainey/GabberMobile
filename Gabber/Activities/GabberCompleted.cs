using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using GabberPCL.Models;

namespace Gabber.Activities
{
    [Activity]
    public class GabberCompleted : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.next_steps);
            SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));
            SupportActionBar.Title = "NEXT STEPS";

            var pager = FindViewById<ViewPager>(Resource.Id.next_steps_pager);

            var pages = new List<OnboardingPageContent> {
                new OnboardingPageContent {
                    Image=Resource.Drawable.onboarding_second,
                    Title="Your Consent",
                    Content="All participants will receive an email to review and provide consent for the Gabber recording"
                },
                new OnboardingPageContent {
                    Image=Resource.Drawable.onboarding_third,
                    Title="Annotate Gabbers",
                    Content="If all participants consent, your Gabber becomes available for project members to listen and annotate"
                }
            };
            pager.Adapter = new Adapters.SharedPager(this, pages);

            var tabs = FindViewById<TabLayout>(Resource.Id.next_steps_tabs);
            tabs.SetupWithViewPager(pager, true);
            tabs.SetSelectedTabIndicatorHeight(0);

            FindViewById<AppCompatButton>(Resource.Id.projects).Click += (s, e) => NavigateTo("projects");
            FindViewById<AppCompatButton>(Resource.Id.sessions).Click += (s, e) => NavigateTo("gabbers");
        }

        void NavigateTo(string fragmentName)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.PutExtra("FRAGMENT_TO_SHOW", fragmentName);
            Finish();
            StartActivity(intent);
        }
    }
}