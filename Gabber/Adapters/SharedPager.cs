using System.Collections.Generic;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using GabberPCL.Models;

namespace Gabber.Adapters
{
    public class SharedPager : PagerAdapter
    {
        Context context;
        List<OnboardingPageContent> Pages;

        public SharedPager(Context _context, List<OnboardingPageContent> _pages)
        {
            context = _context;
            Pages = _pages;
        }

        public override int Count => Pages.Count;

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            var onboardingPage = LayoutInflater.From(container.Context).Inflate(
                Resource.Layout.onboarding_page, container, false);

            var image = onboardingPage.FindViewById<ImageView>(Resource.Id.onboardingImage);
            var title = onboardingPage.FindViewById<TextView>(Resource.Id.onboardingTitle);
            var content = onboardingPage.FindViewById<TextView>(Resource.Id.onboardingContent);

            var page = Pages[position];
            image.SetImageResource(page.Image);
            title.Text = page.Title;
            content.Text = page.Content;

            container.JavaCast<ViewPager>().AddView(onboardingPage);
            return onboardingPage;
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            container.JavaCast<ViewPager>().RemoveView(@object as View);
        }

        public override bool IsViewFromObject(View view, Java.Lang.Object @object) => view == @object;
    }
}
