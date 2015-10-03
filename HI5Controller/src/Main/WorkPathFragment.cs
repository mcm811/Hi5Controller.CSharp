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
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Com.Changyoung.HI5Controller
{
	public class WorkPathFragment : Fragment, IRefresh
	{
		View view;
		private ViewSwitcher viewSwitcher;
		private FloatingActionButton fab;

		private LinearLayout workPathLayout;
		private EditText etWorkPath;
		private FileListFragment workPathFragment;
		private Toolbar workPathToolbar;

		private LinearLayout backupPathLayout;
		private EditText etBackupPath;
		private FileListFragment backupPathFragment;
		private Toolbar backupPathToolbar;

		private void LogDebug(string msg)
		{
			Log.Debug(Context.PackageName, "WorkPathFragment: " + msg);
		}

		private void ToastShow(string str)
		{
			//Toast.MakeText(Context, str, ToastLength.Short).Show();
			Snackbar.Make(view, str, Snackbar.LengthLong).Show();
			LogDebug(str);
		}

		public void Refresh(bool forced = false)
		{
			if (viewSwitcher.CurrentView == workPathLayout) {
				etWorkPath.Text = Pref.WorkPath;
				workPathFragment.RefreshFilesList(etWorkPath.Text);
				fab.SetImageResource(Resource.Drawable.ic_save_white);
			} else if (viewSwitcher.CurrentView == backupPathLayout) {
				etBackupPath.Text = Pref.BackupPath;
				backupPathFragment.RefreshFilesList(etBackupPath.Text);
				fab.SetImageResource(Resource.Drawable.ic_archive_white);
			}
		}

		public bool Refresh(string path)
		{
			if (path != null) {
				try {
					var dir = new DirectoryInfo(path);
					if (dir.IsDirectory()) {
						if (viewSwitcher.CurrentView == workPathLayout) {
							//Pref.WorkPath = path;
							etWorkPath.Text = path;
							workPathFragment.RefreshFilesList(path);
							fab.SetImageResource(Resource.Drawable.ic_save_white);
						} else if (viewSwitcher.CurrentView == backupPathLayout) {
							//Pref.BackupPath = path;
							etBackupPath.Text = path;
							backupPathFragment.RefreshFilesList(path);
							fab.SetImageResource(Resource.Drawable.ic_archive_white);
						}
					}
					return true;
				} catch { }
			}
			return false;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			LogDebug("OnCreateView");
			InputMethodManager imm = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);
			view = inflater.Inflate(Resource.Layout.work_path_fragment, container, false);
			viewSwitcher = view.FindViewById<ViewSwitcher>(Resource.Id.view_switcher);

			// WorkPath
			workPathLayout = view.FindViewById<LinearLayout>(Resource.Id.work_path_layout);
			workPathToolbar = view.FindViewById<Toolbar>(Resource.Id.work_path_toolbar);
			workPathToolbar.Title = "작업 경로";
			workPathToolbar.InflateMenu(Resource.Menu.toolbar_work_path_menu);
			workPathToolbar.MenuItemClick += (sender, e) =>
			{
				//Toast.MakeText(this, "Bottom toolbar pressed: " + e.Item.TitleFormatted, ToastLength.Short).Show();
				viewSwitcher.ShowNext();
				if (viewSwitcher.CurrentView == workPathLayout) {
					fab.SetImageResource(Resource.Drawable.ic_save_white);
					workPathFragment.RefreshFilesList();
					etWorkPath.Text = workPathFragment.DirPath;
				} else if (viewSwitcher.CurrentView == backupPathLayout) {
					fab.SetImageResource(Resource.Drawable.ic_archive_white);
					backupPathFragment.RefreshFilesList();
					etBackupPath.Text = backupPathFragment.DirPath;
				}
			};

			string workPath = Pref.WorkPath;
			workPathFragment = (FileListFragment)ChildFragmentManager.FindFragmentById(Resource.Id.work_path_fragment);
			workPathFragment.PrefKey = Pref.WorkPathKey;
			workPathFragment.RefreshFilesList(workPath);

			etWorkPath = view.FindViewById<EditText>(Resource.Id.etWorkPath);
			etWorkPath.Text = workPath;
			etWorkPath.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
			{
				try {
					var dir = new DirectoryInfo(etWorkPath.Text);
					if (dir.IsDirectory()) {
						Pref.WorkPath = etWorkPath.Text;
					} else {
						ToastShow("잘못된 경로: " + etWorkPath.Text);
						etWorkPath.Text = Pref.WorkPath;
					}
				} catch {
					ToastShow("잘못된 경로: " + etWorkPath.Text);
					etWorkPath.Text = Pref.WorkPath;
				}
				workPathFragment.RefreshFilesList(etWorkPath.Text);
			};
			etWorkPath.KeyPress += (object sender, View.KeyEventArgs e) =>
			{
				e.Handled = false;
				if (e.KeyCode == Keycode.Enter || e.KeyCode == Keycode.Back || e.KeyCode == Keycode.Escape) {
					imm.HideSoftInputFromWindow(etWorkPath.WindowToken, 0);
					etWorkPath.ClearFocus();
					e.Handled = true;
				}
			};

			// BackupPath
			backupPathLayout = view.FindViewById<LinearLayout>(Resource.Id.backup_path_layout);
			backupPathToolbar = view.FindViewById<Toolbar>(Resource.Id.backup_path_toolbar);
			backupPathToolbar.Title = "백업 경로";
			backupPathToolbar.InflateMenu(Resource.Menu.toolbar_backup_path_menu);
			backupPathToolbar.MenuItemClick += (sender, e) =>
			{
				//Toast.MakeText(this, "Bottom toolbar pressed: " + e.Item.TitleFormatted, ToastLength.Short).Show();
				viewSwitcher.ShowNext();
				if (viewSwitcher.CurrentView == workPathLayout) {
					fab.SetImageResource(Resource.Drawable.ic_save_white);
					workPathFragment.RefreshFilesList();
					etWorkPath.Text = workPathFragment.DirPath;
				} else if (viewSwitcher.CurrentView == backupPathLayout) {
					fab.SetImageResource(Resource.Drawable.ic_archive_white);
					backupPathFragment.RefreshFilesList();
					etBackupPath.Text = backupPathFragment.DirPath;
				}
			};

			string backupPath = Pref.BackupPath;
			backupPathFragment = (FileListFragment)ChildFragmentManager.FindFragmentById(Resource.Id.backup_path_fragment);
			backupPathFragment.PrefKey = Pref.BackupPathKey;
			backupPathFragment.RefreshFilesList(backupPath);

			etBackupPath = view.FindViewById<EditText>(Resource.Id.etBackupPath);
			etBackupPath.Text = backupPath;
			etBackupPath.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
			{
				try {
					var dir = new DirectoryInfo(etBackupPath.Text);
					if (dir.IsDirectory()) {
						Pref.BackupPath = etBackupPath.Text;
					} else {
						ToastShow("잘못된 경로: " + etBackupPath.Text);
						etBackupPath.Text = Pref.BackupPath;
					}
				} catch {
					ToastShow("잘못된 경로: " + etBackupPath.Text);
					etBackupPath.Text = Pref.BackupPath;
				}
				backupPathFragment.RefreshFilesList(etBackupPath.Text);
			};
			etBackupPath.KeyPress += (object sender, View.KeyEventArgs e) =>
			{
				e.Handled = false;
				if (e.KeyCode == Keycode.Enter || e.KeyCode == Keycode.Back || e.KeyCode == Keycode.Escape) {
					imm.HideSoftInputFromWindow(etBackupPath.WindowToken, 0);
					etBackupPath.ClearFocus();
					e.Handled = true;
				}
			};

			fab = view.FindViewById<FloatingActionButton>(Resource.Id.fab);
			fab.Click += (sender, e) =>
			{
				viewSwitcher.ShowNext();
				if (viewSwitcher.CurrentView == workPathLayout) {
					fab.SetImageResource(Resource.Drawable.ic_save_white);
					workPathFragment.RefreshFilesList();
					etWorkPath.Text = workPathFragment.DirPath;
				} else if (viewSwitcher.CurrentView == backupPathLayout) {
					fab.SetImageResource(Resource.Drawable.ic_archive_white);
					backupPathFragment.RefreshFilesList();
					etBackupPath.Text = backupPathFragment.DirPath;
				}
			};

			return view;
		}

		public override void OnActivityResult(int requestCode, int resultCode, Intent data)
		{
			if (requestCode == 1) {
				try {
					Pref.WorkPath = data.GetStringExtra("work_path");
				} catch {
				}
			}
			base.OnActivityResult(requestCode, resultCode, data);
		}
	}
}
