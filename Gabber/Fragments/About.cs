using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using GabberPCL.Resources;

namespace Gabber.Fragments
{
    public class About : Android.Support.V4.App.Fragment
    {
        static About instance;

        public static About NewInstance()
        {
            if (instance == null) instance = new About { Arguments = new Bundle() };
            return instance;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var rootView = inflater.Inflate(Resource.Layout.onboarding_page, null);
            var image = rootView.FindViewById<ImageView>(Resource.Id.onboardingImage);
            var title = rootView.FindViewById<TextView>(Resource.Id.onboardingTitle);
            var content = rootView.FindViewById<TextView>(Resource.Id.onboardingContent);

            image.SetBackgroundResource(Resource.Drawable.onboarding_first);
            title.Text = StringResources.about_ui_title;
            content.Text = StringResources.about_ui_content;

            return rootView;
        }
    }
}
