using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

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
            title.Text = "What is Gabber?";
            content.Text = "Gabber is a digital workflow for structuring, capturing and creating conversations on audio recordings. " +
                "\r\nThe mobile application is used to structure and capture these conversations, " +
                "and the website is used for viewing and commenting on the structured audios.";

            return rootView;
        }
    }
}
