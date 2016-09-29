using System;
using System.Collections.Generic;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Java.Lang;

namespace Gabber
{
	public class FragmentAdapter : FragmentPagerAdapter
	{
		readonly List<Fragment> _fragments = new List<Fragment>();
		readonly List<string> _fragmentTitle = new List<string> { "Stages:", "1", "2", "3" };

		public FragmentAdapter(FragmentManager fm) : base(fm) { }

		public override int Count
		{
			get { return _fragments.Count; }
		}

		public override Fragment GetItem(int position)
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

	public class GenericViewPagerFragment : Fragment
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
