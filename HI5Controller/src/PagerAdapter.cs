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
		private int mNumOfTabs;
		private Fragment[] mFragments;

		public PagerAdapter(FragmentManager fm, int NumOfTabs) : base(fm)
		{
			this.mNumOfTabs = NumOfTabs;
			mFragments = new Fragment[NumOfTabs];
		}

		public Fragment this[int index]
		{
			get { return mFragments[index]; }
		}

		public Fragment GetFragment(int i)
		{
			return mFragments[i];
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
			if (mFragments[position] == null) {
				switch (position) {
					case 0:
					mFragments[position] = new WcdPathTabFragment1();
					break;
					case 1:
					mFragments[position] = new WeldCountTabFragment();
					break;
					case 2:
					mFragments[position] = new WcdListTabFragment();
					break;
					case 3:
					mFragments[position] = new SpotCnEditTabFragment();
					break;
					case 4:
					mFragments[position] = new WcdTextTabFragment();
					break;
					default:
					return null;
				}
			}
			return mFragments[position];
		}
	}
}