using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using System;

using Toolbar = Android.Support.V7.Widget.Toolbar;
using DialogFragment = Android.Support.V4.App.DialogFragment;
using FloatingActionButton = Android.Support.Design.Widget.FloatingActionButton;
using ViewPager = Android.Support.V4.View.ViewPager;
using PagerAdapter = Android.Support.V4.View.PagerAdapter;
using TabLayout = Android.Support.Design.Widget.TabLayout;
using ActionBar = Android.Support.V7.App.ActionBar;
using com.xamarin.recipes.filepicker;

namespace com.changmin.HI5Controller
{
	[Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/robot_industrial", Theme = "@style/MyTheme")]
	public class WcdActivity : AppCompatActivity
	{
		private Toolbar toolbar;
		private ActionBar actionBar;
		private DrawerLayout drawerLayout;
		private NavigationView navigationView;

		private TabLayout tabLayout;
		private ViewPager viewPager;
		private PagerAdapter pagerAdapter;

		private AdView adView;

		//private EditText etDirPath;
		//private Button wcdListViewButton;
		//private Button wcdTextButton;
		//private FloatingActionButton fab;

		public string PrefPath
		{
			get
			{
				try {
					using (var prefs = Application.Context.GetSharedPreferences(Application.PackageName, FileCreationMode.Private)) {
						return prefs.GetString("dirpath_file", Android.OS.Environment.ExternalStorageDirectory.AbsolutePath);
					}
				} catch {
					return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
				}
			}
			set
			{
				try {
					using (var prefs = Application.Context.GetSharedPreferences(Application.PackageName, FileCreationMode.Private)) {
						var prefEditor = prefs.Edit();
						prefEditor.PutString("dirpath_file", value);
						prefEditor.Commit();
						ToastShow("경로 저장1: " + value);
					}
				} catch {

				}
			}
		}

		private void ToastShow(string str)
		{
			Toast.MakeText(this, str, ToastLength.Short).Show();
		}

		private void SnackbarShow(View viewParent, string str)
		{
			Snackbar.Make(viewParent, str, Snackbar.LengthLong)
					.SetAction("Undo", (view) => { /*Undo message sending here.*/ })
					.Show(); // Don’t forget to show!
		}

		private void SetFullscreen()
		{
			//Window.RequestFeature(WindowFeatures.NoTitle);
			//Window.ClearFlags(WindowManagerFlags.Fullscreen);
			Window.AddFlags(WindowManagerFlags.Fullscreen);
		}

		private void NaviViewHeader()
		{
			View header = navigationView.InflateHeaderView(Resource.Layout.drawer_header_layout);
			RelativeLayout drawerHeader = header.FindViewById<RelativeLayout>(Resource.Id.drawerHeader);
			drawerHeader.Click += (sender, e) =>
			{
				var intent = new Intent(this, typeof(WcdActivity));
				StartActivity(intent);
			};
		}

		private void NaviView()
		{
			// 서랍 메뉴
			drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
			navigationView.NavigationItemSelected += (sender, e) =>
			{
				//ToastShow(e.MenuItem.ItemId.ToString());
				Intent intent;
				e.MenuItem.SetChecked(true);
				switch (e.MenuItem.ItemId) {
					case Resource.Id.nav_wcd:
					intent = new Intent(this, typeof(WcdListViewActivity));
					intent.PutExtra("dir_path", PrefPath);
					StartActivity(intent);
					break;
					case Resource.Id.nav_robot:
					intent = new Intent(this, typeof(WcdTextViewActivity));
					intent.PutExtra("dir_path", PrefPath);
					StartActivity(intent);
					break;
					case Resource.Id.nav_workpathconfig:
					intent = new Intent(this, typeof(FilePickerActivity));
					intent.PutExtra("dir_path", PrefPath);
					StartActivityForResult(intent, 1);
					break;
				}
				drawerLayout.CloseDrawers();
			};
			//NaviViewHeader();
		}

		private void BaseView()
		{
			//// 기본 화면 구성
			//etDirPath = FindViewById<EditText>(Resource.Id.etDirPath);
			//etDirPath.Text = PrefPath;

			//// 떠 있는 액션버튼
			//fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
			//fab.Elevation = 6;
			//fab.Click += (sender, e) =>
			//{
			//	var intent = new Intent(this, typeof(FilePickerActivity));
			//	intent.PutExtra("dir_path", etDirPath.Text);
			//	StartActivityForResult(intent, 1);
			//};

			//wcdListViewButton = FindViewById<Button>(Resource.Id.button1);
			//wcdListViewButton.Click += (sender, e) =>
			//{
			//	var intent = new Intent(this, typeof(WcdListViewActivity));
			//	intent.PutExtra("dir_path", etDirPath.Text);
			//	StartActivity(intent);
			//};

			//wcdTextButton = FindViewById<Button>(Resource.Id.button2);
			//wcdTextButton.Click += (sender, e) =>
			//{
			//	var intent = new Intent(this, typeof(WcdTextViewActivity));
			//	intent.PutExtra("dir_path", etDirPath.Text);
			//	StartActivity(intent);
			//};

			//ToastShow(Application.PackageName);
			//Log.Error("+++++++++++++++++++++++++:::::::::::::::::::::::::::::::", Application.PackageName);
		}

		private void ActionBarTab()
		{
			//actionBar.NavigationMode = (int) ActionBarNavigationMode.Tabs;
			//ActionBar.Tab tab = actionBar.NewTab();
			//tab.SetText(Resources.GetString(Resource.String.tab1_text));
			//tab.SetIcon(Resource.Drawable.ic_android);
			////tab.SetTabListener(new TabListener<HomeFragment>(this, "home"));
			//actionBar.AddTab(tab);
		}

		// TabListener that replaces a Fragment when a tab is clicked.
		private class TabLayoutOnTabSelectedListener : Java.Lang.Object, TabLayout.IOnTabSelectedListener
		{
			ViewPager viewPager;

			public TabLayoutOnTabSelectedListener(ViewPager vp)
			{
				viewPager = vp;
			}

			public void OnTabReselected(TabLayout.Tab tab)
			{
				//throw new NotImplementedException();
			}

			public void OnTabSelected(TabLayout.Tab tab)
			{
				viewPager.SetCurrentItem(tab.Position, true);
			}

			public void OnTabUnselected(TabLayout.Tab tab)
			{
				//throw new NotImplementedException();
			}
		}

		private void TabLayoutViewPager()
		{
			tabLayout = FindViewById<TabLayout>(Resource.Id.tab_layout);
			tabLayout.AddTab(tabLayout.NewTab().SetText(Resources.GetString(Resource.String.tab1_text)));
			tabLayout.AddTab(tabLayout.NewTab().SetText(Resources.GetString(Resource.String.tab2_text)));
			tabLayout.AddTab(tabLayout.NewTab().SetText(Resources.GetString(Resource.String.tab3_text)));
			tabLayout.TabGravity = TabLayout.GravityFill;

			viewPager = FindViewById<ViewPager>(Resource.Id.pager);
			pagerAdapter = new PagerAdapter(SupportFragmentManager, tabLayout.TabCount);
			viewPager.Adapter = pagerAdapter;
			viewPager.AddOnPageChangeListener(new TabLayout.TabLayoutOnPageChangeListener(tabLayout));
			tabLayout.SetOnTabSelectedListener(new TabLayoutOnTabSelectedListener(viewPager));
		}

		private void AdMob()
		{
			// Create an ad.
			adView = FindViewById<AdView>(Resource.Id.adView);

			// Create an ad request.
			AdRequest adRequest = new AdRequest();
			adRequest.SetTesting(true);

			adRequest.AddTestDevice(AdRequest.TestEmulator);
			// If you're trying to show ads on device, use this.
			// The device ID to test will be shown on adb log.
			// adRequest.AddTestDevice (some_device_id);

			// Start loading the ad in the background.
			adView.LoadAd(adRequest);
		}

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Wcd);
			SetFullscreen();

			// 액션바
			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			SetSupportActionBar(toolbar);
			actionBar = SupportActionBar;
			actionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu_white);
			actionBar.SetDisplayHomeAsUpEnabled(true);
			actionBar.Title = Resources.GetString(Resource.String.ApplicationName);
			actionBar.Elevation = 6;
			actionBar.Show();

			TabLayoutViewPager();
			NaviView();
			BaseView();
			AdMob();
		}

		protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
		{
			if (resultCode == Result.Ok && requestCode == 1) {
				PrefPath = data.GetStringExtra("dir_path") ?? Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
			}
			//ToastShow(resultCode.ToString() + " ::: " + requestCode.ToString() + ":::::" + PrefPath);
			base.OnActivityResult(requestCode, resultCode, data);
		}

		protected override void OnStop()
		{
			//if (PrefPath != etDirPath.Text)
			//	PrefPath = etDirPath.Text;
			base.OnStop();
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			// 액션바 우측 옵션
			MenuInflater.Inflate(Resource.Menu.home, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			// 액션바 옵션 선택시 처리
			//ToastShow(item.ItemId.ToString());
			//Log.Debug("NavigatoinView", item.ItemId.ToString());

			switch (item.ItemId) {
				case Android.Resource.Id.Home:
				drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
				return true;
				case Resource.Id.menu_settings:
				var intent = new Intent(this, typeof(FilePickerActivity));
				intent.PutExtra("dir_path", PrefPath);
				StartActivityForResult(intent, 1);
				return true;
			}
			return base.OnOptionsItemSelected(item);
		}
	}
}
