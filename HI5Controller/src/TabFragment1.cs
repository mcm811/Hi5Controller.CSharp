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
using Android.Views.InputMethods;
using Android.Text;
using System.IO;

namespace Com.Changmin.HI5Controller.src
{
	public class TabFragment1 : Fragment
	{
		View mView;
		private EditText mEtDirPath;
		private FloatingActionButton mFab;
		private FileListFragment mFileListFragment;

		private void LogDebug(string msg)
		{
			Log.Debug(Context.PackageName, "TabFragement1: " + msg);
		}

		private void ToastShow(string str)
		{
			Toast.MakeText(Context, str, ToastLength.Short).Show();
			LogDebug(str);
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

		private void Button()
		{
			//wcdListViewButton = mView.FindViewById<Button>(Resource.Id.button1);
			//wcdListViewButton.Click += (sender, e) =>
			//{
			//	var intent = new Intent(Context, typeof(WcdListActivity));
			//	intent.PutExtra("dir_path", PrefPath);
			//	StartActivity(intent);
			//};
			//
			//wcdTextButton = mView.FindViewById<Button>(Resource.Id.button2);
			//wcdTextButton.Click += (sender, e) =>
			//{
			//	var intent = new Intent(Context, typeof(WcdTextActivity));
			//	intent.PutExtra("dir_path", PrefPath);
			//	StartActivity(intent);
			//};
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			LogDebug("OnCreate");
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			LogDebug("OnCreateView");
			mView = inflater.Inflate(Resource.Layout.tab_fragment_1, container, false);
			InputMethodManager imm = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);

			string path = PrefPath;
			mFileListFragment = (FileListFragment)this.ChildFragmentManager.FindFragmentById(Resource.Id.file_list_fragment);
			mFileListFragment.RefreshFilesList(path);

			mEtDirPath = mView.FindViewById<EditText>(Resource.Id.etDirPath);
			mEtDirPath.Text = path;
			//mEtDirPath.InputType = InputTypes.TextFlagNoSuggestions | InputTypes.ClassText;
			mEtDirPath.KeyPress += (object sender, View.KeyEventArgs e) =>
			{
				// KeyEventArgs.Handled
				// 라우트된 이벤트를 처리된 것으로 표시하려면 true이고,
				// 라우트된 이벤트를 처리되지 않은 것으로 두어 이벤트가 추가로 라우트되도록 허용하려면 false입니다.
				// 기본값은 false입니다. 
				e.Handled = false;
				if (e.KeyCode == Keycode.Enter) {
					try {
						var dir = new DirectoryInfo(mEtDirPath.Text);
						if (dir.IsDirectory()) {
							PrefPath = mEtDirPath.Text;
						} else {
							ToastShow("잘못된 경로: " + mEtDirPath.Text);
							mEtDirPath.Text = PrefPath;
						}
					} catch {
						ToastShow("잘못된 경로: " + mEtDirPath.Text);
                        mEtDirPath.Text = PrefPath;
					}
					mFileListFragment.RefreshFilesList(mEtDirPath.Text);
					imm.HideSoftInputFromWindow(mEtDirPath.WindowToken, 0);
					e.Handled = true;
				}
			};

			mFab = mView.FindViewById<FloatingActionButton>(Resource.Id.fab);
			mFab.Elevation = 6;
			mFab.Click += (sender, e) =>
			{
				mEtDirPath.Text = mFileListFragment.DirPath;
				PrefPath = mEtDirPath.Text;
			};
			Button();

			return mView;
		}

		public override void OnResume()
		{
			LogDebug("OnResume");
			base.OnResume();
		}

		public override void OnPause()
		{
			LogDebug("OnPause");
			base.OnPause();
		}

		public override void OnActivityResult(int requestCode, int resultCode, Intent data)
		{
			if (requestCode == 1) {
				try {
					PrefPath = data.GetStringExtra("dir_path");
					//ToastShow(resultCode.ToString() + " ::: " + requestCode.ToString() + ":::::" + PrefPath);
				} catch {
					//PrefPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
				}
			}
			base.OnActivityResult(requestCode, resultCode, data);
		}
	}
}
