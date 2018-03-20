using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using GabberPCL.Models;

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
                    Image=Resource.Drawable.onboarding_first,
                    Title="What's Gabber?",
                    Content="A digital tool for structuring and capturing audio conversations"
                },
                new OnboardingPageContent {
                    Image=Resource.Drawable.onboarding_second,
                    Title="Gabber projects",
                    Content="Each project contains a set of topics to guide your Gabber"
                },
                new OnboardingPageContent {
                    Image=Resource.Drawable.onboarding_third,
                    Title="Gabber recordings",
                    Content="Once recorded, you can listen and have extend the conversation online"
                }
            };
            pager.Adapter = new Adapters.SharedPager(this, pages);

            var tabs = FindViewById<TabLayout>(Resource.Id.tabs);
            tabs.SetupWithViewPager(pager, true);
            tabs.SetSelectedTabIndicatorHeight(0);

            FindViewById<AppCompatButton>(Resource.Id.login).Click += delegate {
                StartActivity(typeof(LoginActivity));
            };
            FindViewById<AppCompatButton>(Resource.Id.register).Click += delegate {
                StartActivity(typeof(RegisterActivity));
            };
        }
    }
}
