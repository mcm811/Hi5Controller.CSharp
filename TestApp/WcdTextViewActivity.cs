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
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using com.xamarin.recipes.filepicker;

namespace HI5Controller
{
	[Activity(Label = "ROBOT.SWD", MainLauncher = false, Icon = "@drawable/robot_industrial", Theme = "@style/MyTheme")]
	public class WcdTextViewActivity : AppCompatActivity
	{
		private Toolbar toolbar;
		private DrawerLayout drawerLayout;
		private NavigationView navigationView;
		private string dirPath = string.Empty;

		private TextView pathTv;
		private TextView wcdTv;
		private Button btnOk;


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
			SetContentView(Resource.Layout.WcdTextView);

			// 액션바
			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			SetSupportActionBar(toolbar);
			SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu_white);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			SupportActionBar.Title = Resources.GetString(Resource.String.WcdTextViewName);

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
					intent = new Intent(this, typeof(WcdListViewActivity));
					intent.PutExtra("dir_path", dirPath);
					StartActivity(intent);
					break;
					case Resource.Id.nav_robot:
					intent = new Intent(this, typeof(WcdTextViewActivity));
					intent.PutExtra("dir_path", dirPath);
					StartActivity(intent);
					break;
					case Resource.Id.nav_settings:
					break;
				}
				drawerLayout.CloseDrawers();
			};

			string robotPath = System.IO.Path.Combine(dirPath, "ROBOT.SWD");
            pathTv = FindViewById<TextView>(Resource.Id.pathTextView);
			pathTv.Text = robotPath;

			wcdTv = FindViewById<TextView>(Resource.Id.wcdTextView);
			wcdTv.Text = await ReadFileToString(robotPath);

			btnOk = FindViewById<Button>(Resource.Id.btnWcdOk);
			btnOk.Click += (object sender, System.EventArgs e) =>
			{
				Finish();
			};
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