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
			//Snackbar.Make(viewParent, str, Snackbar.LengthLong)
			//		.SetAction("Undo", (view) => { /*Undo message sending here.*/ })
			//		.SetAction("Redo", (view) => { /*Undo message sending here.*/ })
			//		.Show();
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

		public string RefreshParent()
		{
			var parent = Path.GetDirectoryName(backupPathFragment.DirPath);
			return backupPathFragment.RefreshFilesList(parent);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			LogDebug("OnCreateView");
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
			//backupPathToolbar.Title = "백업";
			backupPathToolbar.InflateMenu(Resource.Menu.toolbar_backup_path_menu);
			backupPathToolbar.MenuItemClick += (sender, e) =>
			{
				//Toast.MakeText(this, "Bottom toolbar pressed: " + e.Item.TitleFormatted, ToastLength.Short).Show();
				switch (e.Item.ItemId) {
					case Resource.Id.toolbar_backup_path_menu_up:
					RefreshParent();
					break;
					case Resource.Id.toolbar_backup_path_menu_home:
					Refresh(Pref.BackupPath);
					break;
					case Resource.Id.toolbar_backup_path_menu_restore:
					Restore();
					break;
					//case Resource.Id.toolbar_backup_path_menu_backup:
					//Backup();
					//break;
				}
				// 백업, 복원(현재 경로에 ROBOT.SWD가 있으면 파일들을 작업경로로 복사),
				// 디렉토리 삭제
			};

			fab = view.FindViewById<FloatingActionButton>(Resource.Id.fab);
			fab.Click += (sender, e) =>
			{
				//fab.SetImageResource(Resource.Drawable.ic_archive_white);
				//etBackupPath.Text = backupPathFragment.DirPath;
				//Pref.BackupPath = etBackupPath.Text;
				Backup();
			};

			return view;
		}

		public bool Restore()
		{
			var workPath = Pref.WorkPath;
			var targetFullPath = Path.GetFullPath(workPath);
			var targetFileName = Path.GetFileName(workPath);
			var targetDirName = Path.GetDirectoryName(workPath);

			var backupPath = backupPathFragment.DirPath;
			var sourceFullPath = Path.GetFullPath(backupPath);
			var sourceFileName = Path.GetFileName(backupPath);
			var sourceDirName = Path.GetDirectoryName(backupPath);

			//ToastShow("복원 원본: " + sourceFullPath + "\n복원 대상: " + targetFullPath);

			// 복원할 파일을 먼저 확인
			bool sourceChecked = false;
			if (Directory.Exists(sourceFullPath)) {
				string[] files = Directory.GetFiles(sourceFullPath);
				foreach (string s in files) {
					var fileName = Path.GetFileName(s).ToUpper();
					if (fileName.StartsWith("HX") || fileName.EndsWith("JOB") || fileName.StartsWith("ROBOT")) {
						sourceChecked = true;
						break;
					}
				}
			}

			var ret = false;
			if (sourceChecked) {
				//Directory.Delete(targetFullPath, true);
				if (Directory.Exists(targetFullPath)) {
					string[] files = Directory.GetFiles(targetFullPath);
					foreach (string s in files) {
						File.Delete(s);
						LogDebug("Delete: " + s);
					}
				} else {
					Directory.CreateDirectory(targetFullPath);
				}

				if (Directory.Exists(sourceFullPath)) {
					string[] files = Directory.GetFiles(sourceFullPath);
					foreach (string s in files) {
						var fileName = Path.GetFileName(s);
						var destFile = Path.Combine(targetFullPath, fileName);

						var attributes = File.GetAttributes(s);
						//var creationTime = File.GetCreationTime(s);
						//var lastWriteTime = File.GetLastWriteTime(s);

						File.Copy(s, destFile, true);

						File.SetAttributes(destFile, attributes);
						//File.SetCreationTime(destFile, creationTime);
						//File.SetLastWriteTime(destFile, lastWriteTime);

						LogDebug("Copy " + s + " To " + destFile);
					}
					ret = true;
					Show("복원 완료: " + sourceFileName);
				} else {
					Show("복원 폴더가 없습니다");
				}
			} else {
				Show("복원 파일이 아닙니다");
			}

			return ret;
		}

		public bool Backup()
		{
			var workPath = Pref.WorkPath;
			var sourceFullPath = Path.GetFullPath(workPath);
			var sourceFileName = Path.GetFileName(workPath);
			var sourceDirName = Path.GetDirectoryName(workPath);

			//var backupPath = Pref.BackupPath;
			var targetFileName = sourceFileName + System.DateTime.Now.ToString("_yyyyMMdd_hhmmss");
			var targetDirName = Path.Combine(sourceFullPath, "Backup");
			var targetFullPath = Path.Combine(targetDirName, targetFileName);

			//ToastShow("백업 원본: " + sourceFullPath + "\n백업 대상: " + targetFullPath);

			if (!Directory.Exists(targetFullPath)) {
				Directory.CreateDirectory(targetFullPath);
			}

			var ret = false;
			if (Directory.Exists(sourceFullPath)) {
				string[] files = Directory.GetFiles(sourceFullPath);
				foreach (string s in files) {
					var fileName = Path.GetFileName(s);
					var destFile = Path.Combine(targetFullPath, fileName);

					var attributes = File.GetAttributes(s);
					//var creationTime = File.GetCreationTime(s);
					//var lastWriteTime = File.GetLastWriteTime(s);

					File.Copy(s, destFile, true);

					File.SetAttributes(destFile, attributes);
					//File.SetCreationTime(destFile, creationTime);
					//File.SetLastWriteTime(destFile, lastWriteTime);

					LogDebug("Copy " + s + " To " + destFile);
				}
				ret = true;
				Show("백업 완료: " + targetFileName);
			} else {
				Show("대상 폴더가 없습니다");
			}
			Refresh(Pref.BackupPath);

			return ret;
		}

		public override void OnActivityResult(int requestCode, int resultCode, Intent data)
		{
			if (requestCode == 1) {
				try {
					Pref.WorkPath = data.GetStringExtra("backup_path");
				} catch { }
			}
			base.OnActivityResult(requestCode, resultCode, data);
		}
	}
}
