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
	public class BackupPathFragment : Fragment, IRefresh
	{
		View view;
		private LinearLayout backupPathLayout;
		private EditText etBackupPath;
		private FileListFragment backupPathFragment;
		private Toolbar backupPathToolbar;

		private FloatingActionButton fab;
		private CoordinatorLayout coordinatorLayout;

		private void LogDebug(string msg)
		{
			try {
				Log.Debug(Context.PackageName, "BackupPathFragment: " + msg);
			} catch { }
		}

		public void Show(string str)
		{
			try {
				Snackbar.Make(coordinatorLayout, str, Snackbar.LengthShort).Show();
			} catch { }
			LogDebug(str);
		}

		public void Refresh(bool forced = false)
		{
			etBackupPath.Text = Pref.BackupPath;
			backupPathFragment.RefreshFilesList(etBackupPath.Text);
		}

		public bool Refresh(string path)
		{
			if (path != null) {
				try {
					var dir = new DirectoryInfo(path);
					if (dir.IsDirectory()) {
						backupPathFragment.RefreshFilesList(path);
					}
					return true;
				} catch { }
			}
			return false;
		}

		public string OnBackPressedFragment()
		{
			var parent = Path.GetDirectoryName(backupPathFragment.DirPath);
			return backupPathFragment.RefreshFilesList(parent);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate(Resource.Layout.backup_path_fragment, container, false);
			backupPathLayout = view.FindViewById<LinearLayout>(Resource.Id.backup_path_layout);
			coordinatorLayout = view.FindViewById<CoordinatorLayout>(Resource.Id.coordinator_layout);

			string backupPath = Pref.BackupPath;
			backupPathFragment = (FileListFragment)ChildFragmentManager.FindFragmentById(Resource.Id.backup_path_fragment);
			backupPathFragment.RefreshFilesList(backupPath);
			backupPathFragment.SnackbarView = coordinatorLayout;
			//backupPathFragment.PrefKey = Pref.BackupPathKey;

			etBackupPath = view.FindViewById<EditText>(Resource.Id.etBackupPath);
			etBackupPath.Text = backupPath;
			etBackupPath.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
			{
				try {
					var dir = new DirectoryInfo(etBackupPath.Text);
					if (dir.IsDirectory()) {
						Pref.BackupPath = etBackupPath.Text;
					} else {
						Show("잘못된 경로: " + etBackupPath.Text);
						etBackupPath.Text = Pref.BackupPath;
					}
				} catch {
					Show("잘못된 경로: " + etBackupPath.Text);
					etBackupPath.Text = Pref.BackupPath;
				}
				backupPathFragment.RefreshFilesList(etBackupPath.Text);
			};
			etBackupPath.KeyPress += (object sender, View.KeyEventArgs e) =>
			{
				e.Handled = false;
				if (e.KeyCode == Keycode.Enter || e.KeyCode == Keycode.Back || e.KeyCode == Keycode.Escape) {
					var imm = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);
					imm.HideSoftInputFromWindow(etBackupPath.WindowToken, 0);
					etBackupPath.ClearFocus();
					e.Handled = true;
				}
			};

			backupPathToolbar = view.FindViewById<Toolbar>(Resource.Id.backup_path_toolbar);
			backupPathToolbar.InflateMenu(Resource.Menu.toolbar_backup_path_menu);
			backupPathToolbar.MenuItemClick += (sender, e) =>
			{
				switch (e.Item.ItemId) {
					case Resource.Id.toolbar_backup_path_menu_up:
						OnBackPressedFragment();
						break;
					case Resource.Id.toolbar_backup_path_menu_home:
						Refresh(Pref.BackupPath);
						break;
					case Resource.Id.toolbar_backup_path_menu_restore:
						Show(Restore());
						break;
				}
			};

			fab = view.FindViewById<FloatingActionButton>(Resource.Id.fab);
			fab.Click += (sender, e) =>
			{
				Show(Backup());
			};

			return view;
		}

		public string Restore()
		{
			var workPath = Pref.WorkPath;
			var targetFullPath = Path.GetFullPath(workPath);
			var targetDirName = Path.GetDirectoryName(workPath);

			var backupPath = backupPathFragment.DirPath;
			var sourceFullPath = Path.GetFullPath(backupPath);
			var sourceFileName = Path.GetFileName(backupPath);

			string ret = "복원 실패";
			try {
				// 복원할 파일을 먼저 확인
				bool sourceChecked = false;
				if (Directory.Exists(sourceFullPath)) {
					var srcFiles = Directory.GetFiles(sourceFullPath);
					foreach (var file in srcFiles) {
						var fileName = Path.GetFileName(file).ToUpper();
						if (fileName.StartsWith("HX") || fileName.EndsWith("JOB") || fileName.StartsWith("ROBOT")) {
							sourceChecked = true;
							break;
						}
					}
					if (sourceChecked) {
						if (Directory.Exists(targetFullPath)) {
							var targetFiles = Directory.GetFiles(targetFullPath);
							foreach (var file in targetFiles) {
								File.Delete(file);
							}
						} else {
							Directory.CreateDirectory(targetFullPath);
						}
						foreach (string s in srcFiles) {
							var fileName = Path.GetFileName(s);
							var destFile = Path.Combine(targetFullPath, fileName);
							File.Copy(s, destFile, true);
						}
						ret = "복원 완료: " + sourceFileName;
					}
				}
			} catch { }

			return ret;
		}

		public string Backup()
		{
			var workPath = Pref.WorkPath;
			var sourceFullPath = Path.GetFullPath(workPath);
			var sourceFileName = Path.GetFileName(workPath);

			var targetFileName = sourceFileName + System.DateTime.Now.ToString("_yyyyMMdd_hhmmss");
			var targetDirName = Path.Combine(sourceFullPath, "Backup");
			var targetFullPath = Path.Combine(targetDirName, targetFileName);

			string ret = "백업 실패";
			try {
				if (Directory.Exists(sourceFullPath)) {
					string[] files = Directory.GetFiles(sourceFullPath);
					if (!Directory.Exists(targetFullPath))
						Directory.CreateDirectory(targetFullPath);
					foreach (string s in files) {
						var fileName = Path.GetFileName(s);
						var destFile = Path.Combine(targetFullPath, fileName);
						File.Copy(s, destFile, true);
					}
					ret = "백업 완료: " + targetFileName;
				}
				Refresh(Pref.BackupPath);
			} catch { }

			return ret;
		}
	}
}
