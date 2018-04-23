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
            var rootView = inflater.Inflate(Resource.Layout.about_frag, null);
            var image = rootView.FindViewById<ImageView>(Resource.Id.aboutImage);
            image.SetBackgroundResource(Resource.Drawable.onboarding_first);

            var content = rootView.FindViewById<TextView>(Resource.Id.aboutContent);
            content.Text = StringResources.about_ui_content;

            var link = rootView.FindViewById<TextView>(Resource.Id.aboutLink);
            link.Text = StringResources.about_ui_link;

            var toolbar = rootView.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            ((AppCompatActivity)Activity).SetSupportActionBar(toolbar);
            ((AppCompatActivity)Activity).SupportActionBar.Title = StringResources.about_ui_title;

            return rootView;
        }
    }
}
