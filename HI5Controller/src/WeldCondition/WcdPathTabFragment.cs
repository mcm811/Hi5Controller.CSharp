
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Util;
using Fragment = Android.Support.V4.App.Fragment;
using Android.Support.V4.Widget;
using FloatingActionButton = Android.Support.Design.Widget.FloatingActionButton;
using com.xamarin.recipes.filepicker;
using Android.Views.InputMethods;
using System.IO;

namespace Com.Changyoung.HI5Controller
{
	public class WcdPathTabFragment1 : Fragment
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

		public void Refresh(bool forced = false)
		{
			if (forced) {
				string path = PrefPath;
				mEtDirPath.Text = path;
				mFileListFragment.RefreshFilesList(path);
			}
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			LogDebug("OnCreateView");
			mView = inflater.Inflate(Resource.Layout.WcdPathTabFragment, container, false);
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

			var refresher = mView.FindViewById<SwipeRefreshLayout>(Resource.Id.srl);
			if (refresher != null) {
				refresher.Refresh += delegate
				{
					Refresh(forced: true);
					refresher.Refreshing = false;
				};
			}

			return mView;
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
