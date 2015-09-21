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
		int total;  // SPOT 의 총 카운트
		int gn1;	// SPOT 단어 다음에 나오는 첫번째 단어 분석 해서 종류 결정(GN1, GN2, GN3, G1, G2)
		int gn2;
		int gn3;
		int g1;
		int g2;
		int step;	// S1 S2 S3 붙은 것들 젤 마지막 S번호 값
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