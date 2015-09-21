using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using FragmentStatePagerAdapter = Android.Support.V4.App.FragmentStatePagerAdapter;

namespace Com.Changmin.HI5Controller.src
{
	public class WeldCount
	{
		int total;  // SPOT �� �� ī��Ʈ
		int gn1;	// SPOT �ܾ� ������ ������ ù��° �ܾ� �м� �ؼ� ���� ����(GN1, GN2, GN3, G1, G2)
		int gn2;
		int gn3;
		int g1;
		int g2;
		int step;	// S1 S2 S3 ���� �͵� �� ������ S��ȣ ��
		string preview;

		FileSystemInfo fsi;
		string fileName;
		int fileSize;
		int fileTime;
	}

	public class WeldCountTabFragment : Fragment
	{
		List<WeldCount> mWeldCountList;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.tab_fragment_2, container, false);
			return view;
		}
	}
}