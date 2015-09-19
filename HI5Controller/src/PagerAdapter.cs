using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using FragmentStatePagerAdapter = Android.Support.V4.App.FragmentStatePagerAdapter;
using com.xamarin.recipes.filepicker;

namespace Com.Changmin.HI5Controller.src
{
	public class PagerAdapter : FragmentStatePagerAdapter
	{
		int mNumOfTabs;

		public PagerAdapter(FragmentManager fm, int NumOfTabs) : base(fm)
		{
			this.mNumOfTabs = NumOfTabs;
		}

		public override int Count
		{
			get
			{
				return mNumOfTabs;
			}
		}

		public override Fragment GetItem(int position)
		{
			switch (position) {
				case 0:
				return new TabFragment1();

				case 1:
				return new WcdListFragment();

				case 2:
				return new WcdTextFragment();

				case 3:
				return new FileListFragment();

				default:
				return null;
			}
		}
	}
}