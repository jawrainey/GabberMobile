using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
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

            var lastInterviewSession = GabberPCL.Queries.LastInterviewSession;

            var parts = GabberPCL.Queries.ParticipantsForSession(lastInterviewSession.SessionID);
            var PartNames = new List<string>();
            foreach (var p in parts) PartNames.Add(p.Name);

            var names = PartNames.Count() > 1 ? string.Join(", ", PartNames.Take(PartNames.Count() - 1)) + " and " + PartNames.Last() : PartNames.FirstOrDefault();

            var pages = new List<OnboardingPageContent> {
                new OnboardingPageContent {
                    Image=Resource.Drawable.onboarding_first,
                    Title="Recording success",
                    Content="Thanks for recording your Gabber. Once you upload the recording we will send you an email to review the recording and provide consent."
                },
                new OnboardingPageContent {
                    Image=Resource.Drawable.onboarding_second,
                    Title="Your Consent",
                    Content=$"{names} will receive an email to review and provide consent for the Gabber recording. Once a decision on consent is made others may access the recording on the Gabber website."
                },
                new OnboardingPageContent {
                    Image=Resource.Drawable.onboarding_third,
                    Title="Annotate Gabbers",
                    Content="If all participants consent, your Gabber becomes available for other project members to listen, annotate and learn from the experiences you shared."
                }
            };

            var pager = FindViewById<ViewPager>(Resource.Id.next_steps_pager);
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