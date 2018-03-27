using Android.OS;
using Android.Views;

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
            return inflater.Inflate(Resource.Layout.about_frag, null);
        }
    }
}
