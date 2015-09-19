using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using System.IO;
using Android.Support.V4.Widget;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using DialogFragment = Android.Support.V4.App.DialogFragment;
using FloatingActionButton = Android.Support.Design.Widget.FloatingActionButton;
using com.xamarin.recipes.filepicker;


namespace Com.Changmin.HI5Controller.src
{
	[Activity(Label = "ROBOT.SWD", MainLauncher = false, Icon = "@drawable/robot_industrial", Theme = "@style/MyTheme")]
	public class WcdTextActivity : AppCompatActivity
	{
		private Toolbar toolbar;
		private DrawerLayout drawerLayout;
		private NavigationView navigationView;
		private FloatingActionButton fabDone;

		private TextView pathTv;
		private TextView wcdTv;

		private string dirPath = string.Empty;

		async private Task<string> ReadFileToString(string fileName)
		{
			string st = "";
			try {
				using (StreamReader sr = new StreamReader(fileName)) {
					st = await sr.ReadToEndAsync();
					sr.Close();
				}
			} catch {
				Toast.MakeText(this, "파일이 없습니다: " + fileName + "", ToastLength.Short).Show();
				Finish();
			}

			return st;
		}

		async protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.WcdText);
			Window.AddFlags(WindowManagerFlags.Fullscreen);

			// 액션바
			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			SetSupportActionBar(toolbar);
			SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu_white);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			SupportActionBar.Title = Resources.GetString(Resource.String.WcdTextViewName);
			SupportActionBar.Hide();

			// 서랍 메뉴
			dirPath = Intent.GetStringExtra("dir_path") ?? "";
			drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
			navigationView.NavigationItemSelected += (sender, e) =>
			{
				e.MenuItem.SetChecked(true);
				Intent intent;
				switch (e.MenuItem.ItemId) {
					case Resource.Id.nav_workpathconfig:
					intent = new Intent(this, typeof(FilePickerActivity));
					intent.PutExtra("dir_path", dirPath);
					StartActivityForResult(intent, 1);
					break;
					case Resource.Id.nav_wcd:
					intent = new Intent(this, typeof(WcdListActivity));
					intent.PutExtra("dir_path", dirPath);
					StartActivity(intent);
					break;
					case Resource.Id.nav_robot:
					intent = new Intent(this, typeof(WcdTextActivity));
					intent.PutExtra("dir_path", dirPath);
					StartActivity(intent);
					break;
				}
				drawerLayout.CloseDrawers();
			};
			View header = navigationView.InflateHeaderView(Resource.Layout.drawer_header_layout);
			RelativeLayout drawerHeader = header.FindViewById<RelativeLayout>(Resource.Id.drawerHeader);
			drawerHeader.Click += (sender, e) =>
			{
				var intent = new Intent(this, typeof(WcdActivity));
				StartActivity(intent);
			};
			
			// 떠 있는 액션버튼
			fabDone = FindViewById<FloatingActionButton>(Resource.Id.fab_done);
			fabDone.Click += (sender, e) =>
			{
				Finish();
			};

			string robotPath = System.IO.Path.Combine(dirPath, "ROBOT.SWD");
            pathTv = FindViewById<TextView>(Resource.Id.pathTextView);
			pathTv.Text = robotPath;

			wcdTv = FindViewById<TextView>(Resource.Id.wcdTextView);
			wcdTv.Text = await ReadFileToString(robotPath);
		}

		// 액션바 우측 옵션
		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.home, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		// 액션바 옵션 선택시 처리
		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId) {
				case Android.Resource.Id.Home:
				drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
				return true;
			}
			return base.OnOptionsItemSelected(item);
		}
	}
}