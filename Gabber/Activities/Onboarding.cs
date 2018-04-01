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
                    Image=Resource.Drawable.onboarding_second,
                    Title="Structuring Gabbers",
                    Content="Gabber projects define a set of textual topics to structure the audio recording. " +
                        "Projects are created on the Gabber website for the mobile experience focus on the capturing."
                },
                new OnboardingPageContent {
                    Image=Resource.Drawable.onboarding_first,
                    Title="Adding Participants",
                    Content="Add participants whose experience you want to capture. " +
                        "Once recorded, participants become members of the project and " +
                        "will receive an email to review their consent for how their recording should be used."
                },
                new OnboardingPageContent {
                    Image=Resource.Drawable.onboarding_third,
                    Title="Tagging Recording",
                    Content="Project topics appear in a list to structure the audio recording. " +
                        "These can be tapped to tag the audio from the last time a topic was tapped. " +
                        "This creates regions on top of the audio that are used to identify what is being discussed in the recording."
                },
                new OnboardingPageContent {
                    Image=Resource.Drawable.onboarding_third,
                    Title="Review Consent",
                    Content="Once a recording is uploaded all participants will receive an email " +
                        "where they can review the audio and their consent for how their recording " +
                        "will be viewed, used and shared within the Gabber Project."
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
