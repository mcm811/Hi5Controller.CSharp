using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.OS;
using Android.Util;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;

using Toolbar = Android.Support.V7.Widget.Toolbar;
using ViewPager = Android.Support.V4.View.ViewPager;
using TabLayout = Android.Support.Design.Widget.TabLayout;
using ActionBar = Android.Support.V7.App.ActionBar;
using Android.Graphics.Drawables;
using System.IO;

namespace Com.Changyoung.HI5Controller
{
	[Activity(Label = "@string/ApplicationName", MainLauncher = false, Icon = "@drawable/robot_industrial", Theme = "@style/MyTheme")]
	public class MainActivity : AppCompatActivity
	{
		private View view;
		private Toolbar toolbar;
		private ActionBar actionBar;
		private DrawerLayout drawerLayout;
		private NavigationView navigationView;

		private TabLayout tabLayout;
		private ViewPager viewPager;
		private PagerAdapter pagerAdapter;

		private void LogDebug(string msg)
		{
			Log.Debug(Application.PackageName, "WcdActivity: " + msg);
		}

		private void ToastShow(string str)
		{
			//Toast.MakeText(this, str, ToastLength.Short).Show();
			Snackbar.Make(view, str, Snackbar.LengthLong).Show();
			LogDebug(str);
		}

		private void SnackbarShort(string str)
		{
			//Snackbar.Make(viewParent, str, Snackbar.LengthLong)
			//		.SetAction("Undo", (view) => { /*Undo message sending here.*/ })
			//		.Show(); // Don’t forget to show!
			Snackbar.Make(view, str, Snackbar.LengthShort).Show();
			LogDebug(str);
		}

		private void SnackbarLong(string str)
		{
			//Snackbar.Make(viewParent, str, Snackbar.LengthLong)
			//		.SetAction("Undo", (view) => { /*Undo message sending here.*/ })
			//		.Show(); // Don’t forget to show!
			Snackbar.Make(view, str, Snackbar.LengthLong).Show();
			LogDebug(str);
		}

		private void SetFullscreen()
		{
			//Window.RequestFeature(WindowFeatures.NoTitle);
			//Window.ClearFlags(WindowManagerFlags.Fullscreen);
			Window.AddFlags(WindowManagerFlags.Fullscreen);
		}

		private void NaviViewHeader()
		{
			//View header = navigationView.InflateHeaderView(Resource.Layout.drawer_header_layout);
			//RelativeLayout drawerHeader = header.FindViewById<RelativeLayout>(Resource.Id.drawerHeader);
			//drawerHeader.Click += (sender, e) =>
			//{
			//	var intent = new Intent(this, typeof(WcdActivity));
			//	intent.PutExtra("work_path", Pref.WorkPath);
			//	StartActivity(intent);
			//};
		}

		private Android.Support.V4.App.Fragment GetFragment(int position = 0)
		{
			return (Android.Support.V4.App.Fragment)((PagerAdapter)viewPager.Adapter)[position];
		}

		private bool StorageRefresh(string storagePath)
		{
			var f = (WorkPathFragment)GetFragment();
			if (f != null)
				return f.Refresh(storagePath);
			return false;
		}

		private void NaviView()
		{
			// 서랍 메뉴
			drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
			navigationView.NavigationItemSelected += (sender, e) =>
			{
				e.MenuItem.SetChecked(true);
				switch (e.MenuItem.ItemId) {
					case Resource.Id.nav_weld_count:
					viewPager.SetCurrentItem(1, true);
					break;
					case Resource.Id.nav_weld_condition:
					viewPager.SetCurrentItem(2, true);
					break;
					case Resource.Id.nav_storage:
					viewPager.SetCurrentItem(0, true);
					StorageRefresh("/storage");
					break;
					case Resource.Id.nav_sdcard0:
					viewPager.SetCurrentItem(0, true);
					StorageRefresh(Environment.ExternalStorageDirectory.AbsolutePath);
					break;
					case Resource.Id.nav_extsdcard:
					viewPager.SetCurrentItem(0, true);
					try {
						var dir = new DirectoryInfo("/storage");
						foreach (var item in dir.GetDirectories()) {
							if (item.Name.ToLower().StartsWith("ext") || item.Name.ToLower().StartsWith("sdcard1")) {
								foreach (var subItem in item.GetFileSystemInfos()) {
									if (StorageRefresh(item.FullName)) {
										//SnackbarLong("경로 이동: " + item.FullName);
										break;
									}
								}
							}
						}
					} catch { }
					break;
					case Resource.Id.nav_usbstorage:
					viewPager.SetCurrentItem(0, true);
					try {
						var dir = new DirectoryInfo("/storage");
						foreach (var item in dir.GetDirectories()) {
							if (item.Name.ToLower().StartsWith("usb")) {
								foreach (var subItem in item.GetFileSystemInfos()) {
									if (StorageRefresh(item.FullName)) {
										//SnackbarLong("경로 이동: " + item.FullName);
										break;
									}
								}
							}
						}
					} catch { }
					break;
					case Resource.Id.nav_exit:
					Finish();
					break;
				}
				drawerLayout.CloseDrawers();
			};
			NaviViewHeader();
		}

		private void MainView()
		{
			//// 기본 화면 구성
			//etDirPath = FindViewById<EditText>(Resource.Id.etDirPath);
			//etDirPath.Text = Pref.WorkPath;

			//// 떠 있는 액션버튼
			//fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
			//fab.Elevation = 6;
			//fab.Click += (sender, e) =>
			//{
			//	var intent = new Intent(this, typeof(FilePickerActivity));
			//	intent.PutExtra("work_path", etDirPath.Text);
			//	StartActivityForResult(intent, 1);
			//};

			//wcdListViewButton = FindViewById<Button>(Resource.Id.button1);
			//wcdListViewButton.Click += (sender, e) =>
			//{
			//	var intent = new Intent(this, typeof(WcdListViewActivity));
			//	intent.PutExtra("work_path", etDirPath.Text);
			//	StartActivity(intent);
			//};

			//wcdTextButton = FindViewById<Button>(Resource.Id.button2);
			//wcdTextButton.Click += (sender, e) =>
			//{
			//	var intent = new Intent(this, typeof(WcdTextViewActivity));
			//	intent.PutExtra("work_path", etDirPath.Text);
			//	StartActivity(intent);
			//};
		}

		// TabListener that replaces a Fragment when a tab is clicked.
		private class TabLayoutOnTabSelectedListener : Java.Lang.Object, TabLayout.IOnTabSelectedListener
		{
			Context context;
			ViewPager viewPager;
			ActionBar actionBar;
			TabLayout tabLayout;

			public TabLayoutOnTabSelectedListener(Context context, ViewPager viewPager, ActionBar actionBar, TabLayout tabLayout)
			{
				this.context = context;
				this.viewPager = viewPager;
				this.actionBar = actionBar;
				this.tabLayout = tabLayout;
			}

			public void OnTabReselected(TabLayout.Tab tab)
			{ }

			private void SetBackground(int actionBarColorId, int tabLayoutColorId, int tabIndicatorColorId)
			{
				actionBar.SetBackgroundDrawable(new ColorDrawable(context.Resources.GetColor(actionBarColorId)));
				tabLayout.Background = new ColorDrawable(context.Resources.GetColor(tabLayoutColorId));
				tabLayout.SetSelectedTabIndicatorColor(context.Resources.GetColor(tabIndicatorColorId));
			}

			public void OnTabSelected(TabLayout.Tab tab)
			{
				viewPager.SetCurrentItem(tab.Position, true);
				switch ((PagerAdapter.FragmentPosition)tab.Position) {
					case PagerAdapter.FragmentPosition.WorkPathFragment:
					SetBackground(Resource.Color.tab1_actionbar_background, Resource.Color.tab1_tablayout_background, Resource.Color.tab1_tabindicator_background);
					break;
					case PagerAdapter.FragmentPosition.WeldCountFragment:
					SetBackground(Resource.Color.tab2_actionbar_background, Resource.Color.tab2_tablayout_background, Resource.Color.tab2_tabindicator_background);
					break;
					case PagerAdapter.FragmentPosition.WeldConditionFragment:
					SetBackground(Resource.Color.tab3_actionbar_background, Resource.Color.tab3_tablayout_background, Resource.Color.tab3_tabindicator_background);
					break;
					case PagerAdapter.FragmentPosition.BackupPathFragment:
					SetBackground(Resource.Color.tab4_actionbar_background, Resource.Color.tab4_tablayout_background, Resource.Color.tab4_tabindicator_background);
					break;
				}
				var ir = (IRefresh)((PagerAdapter)viewPager.Adapter)[tab.Position];
				if (ir != null)
					ir.Refresh();
			}

			public void OnTabUnselected(TabLayout.Tab tab)
			{ }
		}

		private void TabLayoutViewPager()
		{
			tabLayout = FindViewById<TabLayout>(Resource.Id.tab_layout);
			tabLayout.TabGravity = TabLayout.GravityFill;
			tabLayout.TabMode = TabLayout.ModeScrollable;

			// PagerAdapter.FragmentPosition과 순서를 맞출것
			tabLayout.AddTab(tabLayout.NewTab().SetText(Resources.GetString(Resource.String.work_path_fragment)));
			tabLayout.AddTab(tabLayout.NewTab().SetText(Resources.GetString(Resource.String.weld_count_fragment)));
			tabLayout.AddTab(tabLayout.NewTab().SetText(Resources.GetString(Resource.String.weld_condition_fragment)));
			tabLayout.AddTab(tabLayout.NewTab().SetText(Resources.GetString(Resource.String.backup_path_fragment)));

			viewPager = FindViewById<ViewPager>(Resource.Id.pager);
			pagerAdapter = new PagerAdapter(SupportFragmentManager, tabLayout.TabCount);
			viewPager.Adapter = pagerAdapter;
			viewPager.AddOnPageChangeListener(new TabLayout.TabLayoutOnPageChangeListener(tabLayout));
			tabLayout.SetOnTabSelectedListener(new TabLayoutOnTabSelectedListener(this, viewPager, actionBar, tabLayout));
		}

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.main_activity);
			SetFullscreen();

			view = FindViewById(Resource.Id.main_parent_view);

			// 액션바
			toolbar = FindViewById<Toolbar>(Resource.Id.main_toolbar);
			SetSupportActionBar(toolbar);
			actionBar = SupportActionBar;
			actionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu_white);
			actionBar.SetDisplayHomeAsUpEnabled(true);
			actionBar.Title = Resources.GetString(Resource.String.ApplicationName);
			//actionBar.Elevation = 0;
			actionBar.Show();

			TabLayoutViewPager();
			NaviView();
			MainView();

			//var position = tabLayout.SelectedTabPosition;
			//int startTabPosition = 0;
			//viewPager.SetCurrentItem(startTabPosition, true);
			//switch (startTabPosition % 3) {
			//	case 0:
			actionBar.SetBackgroundDrawable(new ColorDrawable(Resources.GetColor(Resource.Color.tab1_actionbar_background)));
			tabLayout.Background = new ColorDrawable(Resources.GetColor(Resource.Color.tab1_tablayout_background));
			tabLayout.SetSelectedTabIndicatorColor(Resources.GetColor(Resource.Color.tab1_tabindicator_background));
			//	break;
			//	case 1:
			//	actionBar.SetBackgroundDrawable(new ColorDrawable(Resources.GetColor(Resource.Color.tab2_actionbar_background)));
			//	tabLayout.Background = new ColorDrawable(Resources.GetColor(Resource.Color.tab2_tablayout_background));
			//	tabLayout.SetSelectedTabIndicatorColor(Resources.GetColor(Resource.Color.tab2_tabindicator_background));
			//	break;
			//	case 2:
			//	actionBar.SetBackgroundDrawable(new ColorDrawable(Resources.GetColor(Resource.Color.tab3_actionbar_background)));
			//	tabLayout.Background = new ColorDrawable(Resources.GetColor(Resource.Color.tab3_tablayout_background));
			//	tabLayout.SetSelectedTabIndicatorColor(Resources.GetColor(Resource.Color.tab3_tabindicator_background));
			//	break;
			//}
		}

		protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
		{
			if (resultCode == Result.Ok && requestCode == 1) {
				Pref.WorkPath = data.GetStringExtra("work_path");
			}
			base.OnActivityResult(requestCode, resultCode, data);
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			// 액션바 우측 옵션
			MenuInflater.Inflate(Resource.Menu.main_actionbar_menu, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			// 액션바 옵션 선택시 처리
			switch (item.ItemId) {
				case Android.Resource.Id.Home:
				drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
				return true;

				case Resource.Id.menu_backup:
				OnBackup();
				return true;

				case Resource.Id.menu_exit:
				Finish();
				return true;

				//case Resource.Id.menu_settings:
				//viewPager.SetCurrentItem(0, true);
				//return true;
			}
			return base.OnOptionsItemSelected(item);
		}

		public override void OnBackPressed()
		{
			var position = tabLayout.SelectedTabPosition;
			switch (position) {
				case 0:
				var f0 = (WorkPathFragment)GetFragment(position);
				if (f0 != null)
					f0.RefreshParent();
				break;
				case 1:
				//base.OnBackPressed();
				break;
				case 2:
				//base.OnBackPressed();
				break;
				case 3:
				var f3 = (BackupPathFragment)GetFragment(position);
				if (f3 != null)
					f3.RefreshParent();
				break;
			}
		}

		public void OnBackup()
		{
			var f = (BackupPathFragment)GetFragment(3);
			if (f != null)
				f.Backup();
		}
	}
}
