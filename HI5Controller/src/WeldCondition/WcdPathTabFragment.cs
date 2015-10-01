using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Util;
using Fragment = Android.Support.V4.App.Fragment;
using FloatingActionButton = Android.Support.Design.Widget.FloatingActionButton;
using com.xamarin.recipes.filepicker;
using Android.Views.InputMethods;
using System.IO;

namespace Com.Changyoung.HI5Controller
{
	public class WcdPathTabFragment : Fragment, IRefresh
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
		
		public void Refresh(bool forced = false)
		{
			if (forced) {
				string path = Pref.Path;
				mEtDirPath.Text = path;
				mFileListFragment.RefreshFilesList(path);
			}
        }

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			LogDebug("OnCreateView");
			mView = inflater.Inflate(Resource.Layout.wcd_path_tab_fragment, container, false);
			InputMethodManager imm = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);

			string path = Pref.Path;
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
							Pref.Path = mEtDirPath.Text;
						} else {
							ToastShow("잘못된 경로: " + mEtDirPath.Text);
							mEtDirPath.Text = Pref.Path;
						}
					} catch {
						ToastShow("잘못된 경로: " + mEtDirPath.Text);
                        mEtDirPath.Text = Pref.Path;
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
				Pref.Path = mEtDirPath.Text;
			};

			return mView;
		}

		public override void OnActivityResult(int requestCode, int resultCode, Intent data)
		{
			if (requestCode == 1) {
				try {
					Pref.Path = data.GetStringExtra("dir_path");
				} catch {
				}
			}
			base.OnActivityResult(requestCode, resultCode, data);
		}
	}
}
