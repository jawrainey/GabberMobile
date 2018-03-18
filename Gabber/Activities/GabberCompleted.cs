using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;

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

            var pages = new int[] { 
                Resource.Drawable.onboarding_first,
                Resource.Drawable.onboarding_second
            };
            pager.Adapter = new SharedPagerAdapter(this, pages);

            var tabs = FindViewById<TabLayout>(Resource.Id.next_steps_tabs);
            tabs.SetupWithViewPager(pager, true);
            tabs.SetSelectedTabIndicatorHeight(0);

            FindViewById<AppCompatButton>(Resource.Id.projects).Click += delegate {
                StartActivity(typeof(MainActivity));
                Finish();
            };

            FindViewById<AppCompatButton>(Resource.Id.sessions).Click += delegate {
                StartActivity(typeof(Sessions));
                Finish();
            };
        }
    }
}