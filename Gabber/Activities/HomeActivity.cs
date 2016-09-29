using Android.App;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;

namespace Gabber
{
	[Activity]
	public class HomeActivity : FragmentActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.home);

			var adaptor = new FragmentAdapter(SupportFragmentManager);

			adaptor.AddFragmentView((i, v, _) => { return i.Inflate(Resource.Layout.frag_home, v, false); });
			adaptor.AddFragmentView((i, v, _) => { return i.Inflate(Resource.Layout.frag_collect, v, false); });
			adaptor.AddFragmentView((i, v, _) => { return i.Inflate(Resource.Layout.frag_topic, v, false); });
			adaptor.AddFragmentView((i, v, _) => { return i.Inflate(Resource.Layout.frag_record, v, false); });

			var pager = FindViewById<ViewPager>(Resource.Id.pager);
			pager.Adapter = adaptor;

			var tabs = FindViewById<TabLayout>(Resource.Id.tabs);
			tabs.SetupWithViewPager(pager);

			FindViewById<AppCompatButton>(Resource.Id.login).Click += delegate
			{
				StartActivity(typeof(LoginActivity));
			};

			FindViewById<AppCompatButton>(Resource.Id.register).Click += delegate
			{
				StartActivity(typeof(RegisterActivity));
			};
		}
	}
}