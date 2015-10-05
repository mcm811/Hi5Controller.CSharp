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

		public string OnBackPressedFragment()
		{
			var parent = Path.GetDirectoryName(workPathFragment.DirPath);
			return workPathFragment.RefreshFilesList(parent);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate(Resource.Layout.work_path_fragment, container, false);
			workPathLayout = view.FindViewById<LinearLayout>(Resource.Id.work_path_layout);
			coordinatorLayout = view.FindViewById<CoordinatorLayout>(Resource.Id.coordinator_layout);

			string workPath = Pref.WorkPath;
			workPathFragment = (FileListFragment)ChildFragmentManager.FindFragmentById(Resource.Id.work_path_fragment);
			workPathFragment.RefreshFilesList(workPath);
			workPathFragment.SnackbarView = coordinatorLayout;
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
			workPathToolbar.InflateMenu(Resource.Menu.toolbar_work_path_menu);
			workPathToolbar.MenuItemClick += (sender, e) =>
			{
				//Toast.MakeText(this, "Bottom toolbar pressed: " + e.Item.TitleFormatted, ToastLength.Short).Show();
				bool ret;
				switch (e.Item.ItemId) {
					case Resource.Id.toolbar_work_path_menu_up:
					OnBackPressedFragment();
					break;
					case Resource.Id.toolbar_work_path_menu_home:
					if (!Refresh(Pref.WorkPath))
						Show("경로 이동 실패: " + Pref.WorkPath);
					break;
					case Resource.Id.toolbar_work_path_menu_storage:
					if (!Refresh("/storage"))
						Show("경로 이동 실패: " + "/storage");
					break;
					case Resource.Id.toolbar_work_path_menu_sdcard:
					if (!Refresh(Environment.ExternalStorageDirectory.AbsolutePath))
						Show("경로 이동 실패: " + Environment.ExternalStorageDirectory.AbsolutePath);
					break;
					case Resource.Id.toolbar_work_path_menu_extsdcard:
					ret = false;
					try {
						var dir = new DirectoryInfo("/storage");
						foreach (var item in dir.GetDirectories()) {
							if (item.Name.ToLower().StartsWith("ext") || item.Name.ToLower().StartsWith("sdcard1")) {
								foreach (var subItem in item.GetFileSystemInfos()) {
									if (Refresh(item.FullName)) {
										ret = true;
										break;
									}
								}
							}
						}
					} catch { }
					if (!ret)
						Show("경로 이동 실패: " + "SD 카드");
					break;
					case Resource.Id.toolbar_work_path_menu_usbstorage:
					ret = false;
					try {
						var dir = new DirectoryInfo("/storage");
						foreach (var item in dir.GetDirectories()) {
							if (item.Name.ToLower().StartsWith("usb")) {
								foreach (var subItem in item.GetFileSystemInfos()) {
									if (Refresh(item.FullName)) {
										ret = true;
										break;
									}
								}
							}
						}
					} catch { }
					if (!ret)
						Show("경로 이동 실패: " + "USB 저장소");
					break;
				}
			};

			fab = view.FindViewById<FloatingActionButton>(Resource.Id.fab);
			fab.Click += (sender, e) =>
			{
				etWorkPath.Text = workPathFragment.DirPath;
				Pref.WorkPath = etWorkPath.Text;
				workPathFragment.RefreshFilesList();
				Show("경로 설정 완료: " + etWorkPath.Text);
			};

			return view;
		}
	}
}
