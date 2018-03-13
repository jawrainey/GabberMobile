using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Java.Lang;

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
            pager.Adapter = new OnboardingAdapter(this);

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

    class OnboardingAdapter : PagerAdapter
    {
        Context context;
        static int[] IMAGES = { 
            Resource.Drawable.onboarding_first, 
            Resource.Drawable.onboarding_second, 
            Resource.Drawable.onboarding_third 
        };
        public OnboardingAdapter(Context context) => this.context = context;
        public override int Count => IMAGES.Length;

        public override Object InstantiateItem(View container, int position)
        {
            var imageView = new ImageView(context);
            imageView.SetImageResource(IMAGES[position]);
            container.JavaCast<ViewPager>().AddView(imageView);
            return imageView;
        }

        public override void DestroyItem(View container, int position, Object @object)
        {
            container.JavaCast<ViewPager>().RemoveView(@object as View);
        }

        public override bool IsViewFromObject(View view, Object @object) => view == @object;
    }
}
