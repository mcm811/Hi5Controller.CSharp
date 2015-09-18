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
using Android.Util;
using Android.Support.V4.App;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using FragmentStatePagerAdapter = Android.Support.V4.App.FragmentStatePagerAdapter;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using DialogFragment = Android.Support.V4.App.DialogFragment;
using FloatingActionButton = Android.Support.Design.Widget.FloatingActionButton;
using ViewPager = Android.Support.V4.View.ViewPager;
using PagerAdapter = Android.Support.V4.View.PagerAdapter;
using TabLayout = Android.Support.Design.Widget.TabLayout;
using ActionBar = Android.Support.V7.App.ActionBar;
using com.xamarin.recipes.filepicker;
using Com.Google.Ads;


namespace com.changmin.HI5Controller
{
	public class TabFragment1 : Fragment
	{
		private EditText etDirPath;
		private Button wcdListViewButton;
		private Button wcdTextButton;
		private FloatingActionButton fab;

		private const string AdmobID = "ca-app-pub-4103700007170181/6544598462";
		GADBannerView adView;
		bool viewOnScreen = false;

		public string PrefPath
		{
			get
			{
				try {
					using (var prefs = Application.Context.GetSharedPreferences(Context.PackageName, FileCreationMode.Private)) {
						return prefs.GetString("dirpath_file", Android.OS.Environment.ExternalStorageDirectory.AbsolutePath);
					}
				} catch {
					return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
				}
			}
			set
			{
				try {
					using (var prefs = Application.Context.GetSharedPreferences(Context.PackageName, FileCreationMode.Private)) {
						var prefEditor = prefs.Edit();
						prefEditor.PutString("dirpath_file", value);
						prefEditor.Commit();
						ToastShow("경로 저장2: " + value);
					}
				} catch {

				}
			}
		}

		private void ToastShow(string str)
		{
			Toast.MakeText(Activity, str, ToastLength.Short).Show();
		}

		private void BaseView(View view)
		{
			// 기본 화면 구성
			etDirPath = view.FindViewById<EditText>(Resource.Id.etDirPath);
			etDirPath.Text = PrefPath;

			// 떠 있는 액션버튼
			fab = view.FindViewById<FloatingActionButton>(Resource.Id.fab);
			fab.Elevation = 6;
			fab.Click += (sender, e) =>
			{
				var intent = new Intent(Context, typeof(FilePickerActivity));
				intent.PutExtra("dir_path", PrefPath);
				StartActivityForResult(intent, 1);
			};

			wcdListViewButton = view.FindViewById<Button>(Resource.Id.button1);
			wcdListViewButton.Click += (sender, e) =>
			{
				var intent = new Intent(Context, typeof(WcdListViewActivity));
				intent.PutExtra("dir_path", PrefPath);
				StartActivity(intent);
			};

			wcdTextButton = view.FindViewById<Button>(Resource.Id.button2);
			wcdTextButton.Click += (sender, e) =>
			{
				var intent = new Intent(Context, typeof(WcdTextViewActivity));
				intent.PutExtra("dir_path", PrefPath);
				StartActivity(intent);
			};
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.tab_fragment_1, container, false);
			BaseView(view);
			return view;
		}

		public override void OnResume()
		{
			etDirPath.Text = PrefPath;
			base.OnResume();
		}

		public override void OnPause()
		{
			if (PrefPath != etDirPath.Text)
				PrefPath = etDirPath.Text;
			base.OnPause();
		}

		public override void OnActivityResult(int requestCode, int resultCode, Intent data)
		{
			if (requestCode == 1) {
				PrefPath = data.GetStringExtra("dir_path") ?? Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
				ToastShow(resultCode.ToString() + " ::: " + requestCode.ToString() + ":::::" + PrefPath);
			}
			base.OnActivityResult(requestCode, resultCode, data);
		}
	}
}
