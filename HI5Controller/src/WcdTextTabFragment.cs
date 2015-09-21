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
using System.Threading.Tasks;
using System.IO;
using Android.Util;
using Android.Support.V4.Widget;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using DialogFragment = Android.Support.V4.App.DialogFragment;
using FloatingActionButton = Android.Support.Design.Widget.FloatingActionButton;
using com.xamarin.recipes.filepicker;

namespace Com.Changmin.HI5Controller.src
{
	public class WcdTextTabFragment : Android.Support.V4.App.Fragment
	{
		private View mView;
		private TextView mTvPath;
		private TextView mTvWcd;

		private FloatingActionButton mFab;	// 다시 읽어오기

		private string dirPath;
		private string robotPath;

		private void LogDebug(string msg)
		{
			Log.Debug(Context.PackageName, "WcdTextFragement: " + msg);
		}

		private void ToastShow(string str)
		{
			Toast.MakeText(Context, str, ToastLength.Short).Show();
			LogDebug(str);
		}

		private void SnackbarShow(View viewParent, string str)
		{
			Snackbar.Make(viewParent, str, Snackbar.LengthLong)
					.SetAction("Undo", (mView) => { /*Undo message sending here.*/ })
					.Show(); // Don’t forget to show!
		}

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
					if (PrefPath != value) {
						using (var prefs = Application.Context.GetSharedPreferences(Context.PackageName, FileCreationMode.Private)) {
							var prefEditor = prefs.Edit();
							prefEditor.PutString("dirpath_file", value);
							prefEditor.Commit();
							ToastShow("경로 저장: " + value);
						}
					}
				} catch {

				}
			}
		}

		async private Task<string> ReadFileAsync(string fileName)
		{
			string st = "";
			try {
				using (StreamReader sr = new StreamReader(fileName)) {
					st = await sr.ReadToEndAsync();
					sr.Close();
				}
			} catch {
				ToastShow("파일이 없습니다: " + fileName);
			}

			return st;
		}

		private string ReadFile(string fileName)
		{
			string st = "";
			try {
				using (StreamReader sr = new StreamReader(fileName)) {
					st = sr.ReadToEnd();
					sr.Close();
				}
			} catch {
				ToastShow("파일이 없습니다: " + fileName);
			}

			return st;
		}

		public override void OnCreate(Bundle bundle)
		{
			LogDebug("OnCreate");
			base.OnCreate(bundle);
		}

		public override void OnResume()
		{
			LogDebug("OnResume");
			if (dirPath != PrefPath || mTvWcd.Text.Length == 0) {
				LogDebug("OnResume: " + dirPath + " : " + PrefPath);
				dirPath = PrefPath;
				robotPath = Path.Combine(dirPath, "ROBOT.SWD");
				mTvPath.Text = robotPath;
				mTvWcd.Text = ReadFile(robotPath);
			}
			base.OnResume();
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			LogDebug("OnCreateView");
			mView = inflater.Inflate(Resource.Layout.WcdTextTabFragment, container, false);

			mTvPath = mView.FindViewById<TextView>(Resource.Id.pathTextView);
			mTvWcd = mView.FindViewById<TextView>(Resource.Id.wcdTextView);

			// 떠 있는 액션버튼
			mFab = mView.FindViewById<FloatingActionButton>(Resource.Id.fab);
			mFab.Elevation = 6;
			mFab.Click += (sender, e) =>
			{
				dirPath = PrefPath;
				robotPath = Path.Combine(dirPath, "ROBOT.SWD");
				mTvPath.Text = robotPath;
				mTvWcd.Text = ReadFile(robotPath);
			};

			return mView;
		}

		// 액션바 우측 옵션
		//public override bool OnCreateOptionsMenu(IMenu menu)
		//{
		//	MenuInflater.Inflate(Resource.Menu.home, menu);
		//	return base.OnCreateOptionsMenu(menu);
		//}

		//// 액션바 옵션 선택시 처리
		//public override bool OnOptionsItemSelected(IMenuItem item)
		//{
		//	switch (item.ItemId) {
		//		case Android.Resource.Id.Home:
		//		drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
		//		return true;
		//	}
		//	return base.OnOptionsItemSelected(item);
		//}
	}
}