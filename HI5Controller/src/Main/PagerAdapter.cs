using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using FragmentStatePagerAdapter = Android.Support.V4.App.FragmentStatePagerAdapter;

namespace Com.Changyoung.HI5Controller
{
	public class PagerAdapter : FragmentStatePagerAdapter
	{
		private int numOfTabs;
		private Fragment[] fragments;

		public PagerAdapter(FragmentManager fm, int NumOfTabs) : base(fm)
		{
			this.numOfTabs = NumOfTabs;
			fragments = new Fragment[NumOfTabs];
		}

		public Fragment this[int index]
		{
			get { return fragments[index]; }
		}

		public Fragment GetFragment(int i)
		{
			return fragments[i];
		}

		public override int Count
		{
			get
			{
				return numOfTabs;
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
			if (fragments[position] == null) {
				switch ((FragmentPosition)position) {
					case FragmentPosition.WcdPathTabFragment:
					fragments[position] = new WorkPathFragment();
					break;
					case FragmentPosition.WeldCountTabFragment:
					fragments[position] = new WeldCountFragment();
					break;
					case FragmentPosition.WcdListTabFragment:
					fragments[position] = new WcdListFragment();
					break;
					case FragmentPosition.JobEditTabFragment:
					fragments[position] = new JobEditFragment();
					break;
					case FragmentPosition.WcdTextTabFragment:
					fragments[position] = new WcdTextFragment();
					break;
					default:
					return null;
				}
			}
			return fragments[position];
		}
	}
}