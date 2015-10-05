using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.OS;
using Android.Util;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;

using Fragment = Android.Support.V4.App.Fragment;
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

		private int exitCount;

		private void LogDebug(string msg)
		{
			Log.Debug(Application.PackageName, "WcdActivity: " + msg);
		}

		public void Show(string str)
		{
			try {
				var r = (IRefresh)((PagerAdapter)viewPager.Adapter)[tabLayout.SelectedTabPosition];
				if (r != null)
					r.Show(str);
			} catch { }
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

		private Fragment GetFragment(int position = 0)
		{
			return ((PagerAdapter)viewPager.Adapter)[position];
		}

		private bool StorageRefresh(string storagePath)
		{
			var r = (IRefresh)GetFragment(0);
			if (r != null)
				return r.Refresh(storagePath);
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
				bool ret;
				switch (e.MenuItem.ItemId) {
					case Resource.Id.nav_weld_count:
					viewPager.SetCurrentItem(1, true);
					break;
					case Resource.Id.nav_weld_condition:
					viewPager.SetCurrentItem(2, true);
					break;
					case Resource.Id.nav_storage:
					viewPager.SetCurrentItem(0, true);
					if (!StorageRefresh("/storage"))
						Show("경로 이동 실패: " + "/storage");
					break;
					case Resource.Id.nav_sdcard0:
					viewPager.SetCurrentItem(0, true);
					if (!StorageRefresh(Environment.ExternalStorageDirectory.AbsolutePath))
						Show("경로 이동 실패: " + Environment.ExternalStorageDirectory.AbsolutePath);
					break;
					case Resource.Id.nav_extsdcard:
					viewPager.SetCurrentItem(0, true);
					ret = false;
					try {
						var dir = new DirectoryInfo("/storage");
						foreach (var item in dir.GetDirectories()) {
							if (item.Name.ToLower().StartsWith("ext") || item.Name.ToLower().StartsWith("sdcard1")) {
								foreach (var subItem in item.GetFileSystemInfos()) {
									if (StorageRefresh(item.FullName)) {
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
					case Resource.Id.nav_usbstorage:
					viewPager.SetCurrentItem(0, true);
					ret = false;
					try {
						var dir = new DirectoryInfo("/storage");
						foreach (var item in dir.GetDirectories()) {
							if (item.Name.ToLower().StartsWith("usb")) {
								foreach (var subItem in item.GetFileSystemInfos()) {
									if (StorageRefresh(item.FullName)) {
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
					case Resource.Id.nav_exit:
					Finish();
					break;
				}
				drawerLayout.CloseDrawers();
			};
			NaviViewHeader();
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
			actionBar.Show();

			TabLayoutViewPager();
			NaviView();

			//var position = tabLayout.SelectedTabPosition;
			//viewPager.SetCurrentItem(0, true);
			actionBar.SetBackgroundDrawable(new ColorDrawable(Resources.GetColor(Resource.Color.tab1_actionbar_background)));
			tabLayout.Background = new ColorDrawable(Resources.GetColor(Resource.Color.tab1_tablayout_background));
			tabLayout.SetSelectedTabIndicatorColor(Resources.GetColor(Resource.Color.tab1_tabindicator_background));
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
				viewPager.SetCurrentItem((int)PagerAdapter.FragmentPosition.BackupPathFragment, true);
				var f = (BackupPathFragment)GetFragment((int)PagerAdapter.FragmentPosition.BackupPathFragment);
				if (f != null)
					Show(f.Backup());
				return true;

				case Resource.Id.menu_exit:
				Finish();
				return true;
			}
			return base.OnOptionsItemSelected(item);
		}

		public override void OnBackPressed()
		{
			try {
				var r = (IRefresh)GetFragment(tabLayout.SelectedTabPosition);
				if (r != null) {
					var s = r.OnBackPressedFragment();
                    if (s == null || s == "/") {
						exitCount++;
					} else {
						exitCount = 0;
					}
				}
			} catch {
				exitCount++;
			}

			if (exitCount >= 2)
				base.OnBackPressed();
		}
	}
}
