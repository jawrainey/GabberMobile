using Android.App;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using System;
using System.Collections.Generic;
using Android.Views;
using Android.Support.V7.Widget;
using Android.Support.Design.Widget;
using Java.Lang;

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

	public class FragmentAdapter : FragmentPagerAdapter
	{
		readonly List<Android.Support.V4.App.Fragment> _fragments = new List<Android.Support.V4.App.Fragment>();
		readonly List<string> _fragmentTitle = new List<string> { "Stages:", "1", "2", "3" };

		public FragmentAdapter(Android.Support.V4.App.FragmentManager fm) : base(fm) { }

		public override int Count
		{
			get { return _fragments.Count; }
		}

		public override Android.Support.V4.App.Fragment GetItem(int position)
		{
			return _fragments[position];
		}

		public void AddFragmentView(Func<LayoutInflater, ViewGroup, Bundle, View> view)
		{
			_fragments.Add(new GenericViewPagerFragment(view));
		}

		public override ICharSequence GetPageTitleFormatted(int position)
		{
			return new Java.Lang.String(_fragmentTitle[position]);
		}
	}

	public class GenericViewPagerFragment : Android.Support.V4.App.Fragment
	{
		readonly Func<LayoutInflater, ViewGroup, Bundle, View> _view;

		public GenericViewPagerFragment(Func<LayoutInflater, ViewGroup, Bundle, View> view)
		{
			_view = view;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			return _view(inflater, container, savedInstanceState);
		}
	}
}