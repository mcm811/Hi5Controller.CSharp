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
		private LinearLayout workPathLayout;
		private EditText etWorkPath;
		private FileListFragment workPathFragment;
		private Toolbar workPathToolbar;

		private FloatingActionButton fab;
		private CoordinatorLayout coordinatorLayout;

		private void LogDebug(string msg)
		{
			try {
				Log.Debug(Context.PackageName, "WorkPathFragment: " + msg);
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
			if (forced) {
				etWorkPath.Text = Pref.WorkPath;
				workPathFragment.RefreshFilesList(etWorkPath.Text);
			}
		}

		public bool Refresh(string path)
		{
			if (path != null) {
				try {
					var dir = new DirectoryInfo(path);
					if (dir.IsDirectory()) {
						workPathFragment.RefreshFilesList(path);
					}
					return true;
				} catch { }
			}
			return false;
		}

		public string RefreshParent()
		{
			var parent = Path.GetDirectoryName(workPathFragment.DirPath);
            return workPathFragment.RefreshFilesList(parent);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			LogDebug("OnCreateView");
			view = inflater.Inflate(Resource.Layout.work_path_fragment, container, false);
			workPathLayout = view.FindViewById<LinearLayout>(Resource.Id.work_path_layout);
			coordinatorLayout = view.FindViewById<CoordinatorLayout>(Resource.Id.coordinator_layout);

			string workPath = Pref.WorkPath;
			workPathFragment = (FileListFragment)ChildFragmentManager.FindFragmentById(Resource.Id.work_path_fragment);
			workPathFragment.RefreshFilesList(workPath);
			//workPathFragment.PrefKey = Pref.WorkPathKey;

			etWorkPath = view.FindViewById<EditText>(Resource.Id.etWorkPath);
			etWorkPath.Text = workPath;
			etWorkPath.FocusChange += (object sender, View.FocusChangeEventArgs e) =>
			{
				try {
					var dir = new DirectoryInfo(etWorkPath.Text);
					if (dir.IsDirectory()) {
						Pref.WorkPath = etWorkPath.Text;
					} else {
						Show("잘못된 경로: " + etWorkPath.Text);
						etWorkPath.Text = Pref.WorkPath;
					}
				} catch {
					Show("잘못된 경로: " + etWorkPath.Text);
					etWorkPath.Text = Pref.WorkPath;
				}
				workPathFragment.RefreshFilesList(etWorkPath.Text);
			};
			etWorkPath.KeyPress += (object sender, View.KeyEventArgs e) =>
			{
				e.Handled = false;
				if (e.KeyCode == Keycode.Enter || e.KeyCode == Keycode.Back || e.KeyCode == Keycode.Escape) {
					var imm = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);
					imm.HideSoftInputFromWindow(etWorkPath.WindowToken, 0);
					etWorkPath.ClearFocus();
					e.Handled = true;
				}
			};

			workPathToolbar = view.FindViewById<Toolbar>(Resource.Id.work_path_toolbar);
			//workPathToolbar.Title = "이동";
			workPathToolbar.InflateMenu(Resource.Menu.toolbar_work_path_menu);
			workPathToolbar.MenuItemClick += (sender, e) =>
			{
				//Toast.MakeText(this, "Bottom toolbar pressed: " + e.Item.TitleFormatted, ToastLength.Short).Show();
				switch (e.Item.ItemId) {
					case Resource.Id.toolbar_work_path_menu_up:
					RefreshParent();
					break;
					case Resource.Id.toolbar_work_path_menu_home:
					Refresh(Pref.WorkPath);
					break;
					case Resource.Id.toolbar_work_path_menu_storage:
					Refresh("/storage");
					break;
					case Resource.Id.toolbar_work_path_menu_sdcard:
					Refresh(Environment.ExternalStorageDirectory.AbsolutePath);
					break;
					case Resource.Id.toolbar_work_path_menu_extsdcard:
					try {
						var dir = new DirectoryInfo("/storage");
						foreach (var item in dir.GetDirectories()) {
							if (item.Name.ToLower().StartsWith("ext") || item.Name.ToLower().StartsWith("sdcard1")) {
								foreach (var subItem in item.GetFileSystemInfos()) {
									if (Refresh(item.FullName)) {
										//SnackbarLong("경로 이동: " + item.FullName);
										break;
									}
								}
							}
						}
					} catch { }
					break;
					case Resource.Id.toolbar_work_path_menu_usbstorage:
					try {
						var dir = new DirectoryInfo("/storage");
						foreach (var item in dir.GetDirectories()) {
							if (item.Name.ToLower().StartsWith("usb")) {
								foreach (var subItem in item.GetFileSystemInfos()) {
									if (Refresh(item.FullName)) {
										//SnackbarLong("경로 이동: " + item.FullName);
										break;
									}
								}
							}
						}
					} catch { }
					break;
					//case Resource.Id.toolbar_work_path_menu_backup:
					//Refresh(Pref.BackupPath);
					//break;
				}
			};

			fab = view.FindViewById<FloatingActionButton>(Resource.Id.fab);
			fab.Click += (sender, e) =>
			{
				//fab.SetImageResource(Resource.Drawable.ic_save_white);
				etWorkPath.Text = workPathFragment.DirPath;
				Pref.WorkPath = etWorkPath.Text;
				workPathFragment.RefreshFilesList();
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
