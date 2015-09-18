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

namespace com.changmin.HI5Controller
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
				TabFragment1 tab1 = new TabFragment1();
				return tab1;
				case 1:
				TabFragment2 tab2 = new TabFragment2();
				return tab2;
				case 2:
				TabFragment3 tab3 = new TabFragment3();
				return tab3;
				default:
				return null;
			}
		}
	}
}