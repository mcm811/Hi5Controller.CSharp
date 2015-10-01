using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using FragmentStatePagerAdapter = Android.Support.V4.App.FragmentStatePagerAdapter;

namespace Com.Changyoung.HI5Controller
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

		public enum FragmentPosition
		{
			WcdPathTabFragment,
			WeldCountTabFragment,
			WcdListTabFragment,
			JobEditTabFragment,
			WcdTextTabFragment
		}

		public override Fragment GetItem(int position)
		{
			if (mFragments[position] == null) {
				switch ((FragmentPosition)position) {
					case FragmentPosition.WcdPathTabFragment:
					mFragments[position] = new WcdPathTabFragment();
					break;
					case FragmentPosition.WeldCountTabFragment:
					mFragments[position] = new WeldCountTabFragment();
					break;
					case FragmentPosition.WcdListTabFragment:
					mFragments[position] = new WcdListTabFragment();
					break;
					case FragmentPosition.JobEditTabFragment:
					mFragments[position] = new JobEditTabFragment();
					break;
					case FragmentPosition.WcdTextTabFragment:
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