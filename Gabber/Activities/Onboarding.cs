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

            var pages = new int[] {
                Resource.Drawable.onboarding_first,
                Resource.Drawable.onboarding_second,
                Resource.Drawable.onboarding_third
            };
            pager.Adapter = new SharedPagerAdapter(this, pages);

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

    class SharedPagerAdapter : PagerAdapter
    {
        Context context;
        int[] images;
        // TODO: images should be a list of <image, text> as the text in images would not be translated.
        public SharedPagerAdapter(Context _context, int[] _images)
        {
            context = _context;
            images = _images;
        }

        public override int Count => images.Length;

        public override Object InstantiateItem(View container, int position)
        {
            var imageView = new ImageView(context);
            imageView.SetImageResource(images[position]);
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
