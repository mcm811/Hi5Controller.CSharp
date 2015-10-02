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
using Android.Support.Design.Widget;

namespace Com.Changyoung.HI5Controller
{
	public class WcdPathTabFragment : Fragment, IRefresh
	{
		View view;
		private EditText etDirPath;
		private FloatingActionButton fab;
		private FileListFragment fileListFragment;

		private void LogDebug(string msg)
		{
			Log.Debug(Context.PackageName, "TabFragement1: " + msg);
		}

		private void ToastShow(string str)
		{
			//Toast.MakeText(Context, str, ToastLength.Short).Show();
			Snackbar.Make(view, str, Snackbar.LengthLong).Show();
			LogDebug(str);
		}

		public void Refresh(bool forced = false)
		{
			if (forced) {
				etDirPath.Text = Pref.Path;
				fileListFragment.RefreshFilesList(etDirPath.Text);
			}
		}

		public bool Refresh(string path)
		{
			if (path != null) {
				try {
					var dir = new DirectoryInfo(path);
					if (dir.IsDirectory()) {
						Pref.Path = path;
						etDirPath.Text = path;
						fileListFragment.RefreshFilesList(path);
					}
					return true;
				} catch { }
			}
			return false;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			LogDebug("OnCreateView");
			view = inflater.Inflate(Resource.Layout.wcd_path_tab_fragment, container, false);
			InputMethodManager imm = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);

			string path = Pref.Path;
			fileListFragment = (FileListFragment)this.ChildFragmentManager.FindFragmentById(Resource.Id.file_list_fragment);
			fileListFragment.RefreshFilesList(path);

			etDirPath = view.FindViewById<EditText>(Resource.Id.etDirPath);
			etDirPath.Text = path;
			etDirPath.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
			{
				try {
					var dir = new DirectoryInfo(etDirPath.Text);
					if (dir.IsDirectory()) {
						Pref.Path = etDirPath.Text;
					} else {
						ToastShow("잘못된 경로: " + etDirPath.Text);
						etDirPath.Text = Pref.Path;
					}
				} catch {
					ToastShow("잘못된 경로: " + etDirPath.Text);
					etDirPath.Text = Pref.Path;
				}
				fileListFragment.RefreshFilesList(etDirPath.Text);
			};
			etDirPath.KeyPress += (object sender, View.KeyEventArgs e) =>
			{
				e.Handled = false;
				if (e.KeyCode == Keycode.Enter || e.KeyCode == Keycode.Back || e.KeyCode == Keycode.Escape) {
					imm.HideSoftInputFromWindow(etDirPath.WindowToken, 0);
					etDirPath.ClearFocus();
					e.Handled = true;
				}
			};

			fab = view.FindViewById<FloatingActionButton>(Resource.Id.fab);
			//fab.Elevation = 6;
			fab.Click += (sender, e) =>
			{
				etDirPath.Text = fileListFragment.DirPath;
				Pref.Path = etDirPath.Text;
			};

			return view;
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
