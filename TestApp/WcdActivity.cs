﻿using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using DialogFragment = Android.Support.V4.App.DialogFragment;
using FloatingActionButton = Android.Support.Design.Widget.FloatingActionButton;
using com.xamarin.recipes.filepicker;

namespace HI5Controller
{
	[Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/robot_industrial", Theme = "@style/MyTheme")]
	public class WcdActivity : AppCompatActivity
	{
		private Toolbar toolbar;
		private DrawerLayout drawerLayout;
		private NavigationView navigationView;
		private FloatingActionButton fab;

		private EditText dirPath;
		private Button folderPickerButton;
		private Button wcdListViewButton;
		private Button wcdTextButton;

		private string DirPath
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
						ToastShow("경로 저장: " + value);
					}
				} catch {

				}
			}
		}

		private void ToastShow(string str)
		{
			Toast
				.MakeText(this, str, ToastLength.Short)
				.Show();
			//Snackbar
			//	.Make(parentLayout, "Message sent", Snackbar.LengthLong)
			//	.SetAction("Undo", (view) => { /*Undo message sending here.*/ })
			//	.Show(); // Don’t forget to show!
		}

		protected override void OnCreate(Bundle bundle)
		{
			//Window.RequestFeature(WindowFeatures.NoTitle);
			//Window.AddFlags(WindowManagerFlags.Fullscreen);
			//Window.ClearFlags(WindowManagerFlags.Fullscreen);

			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Wcd);

			// 액션바
			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			SetSupportActionBar(toolbar);
			SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu_white);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			SupportActionBar.Title = Resources.GetString(Resource.String.ApplicationName);

			// 서랍 메뉴
			drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
			navigationView.NavigationItemSelected += (sender, e) =>
			{
				e.MenuItem.SetChecked(true);
				Intent intent;
				switch (e.MenuItem.ItemId) {
					case Resource.Id.nav_workpathconfig:
					intent = new Intent(this, typeof(FilePickerActivity));
					intent.PutExtra("dir_path", dirPath.Text);
					StartActivityForResult(intent, 1);
					break;
					case Resource.Id.nav_wcd:
					intent = new Intent(this, typeof(WcdListViewActivity));
					intent.PutExtra("dir_path", dirPath.Text);
					StartActivity(intent);
					break;
					case Resource.Id.nav_robot:
					intent = new Intent(this, typeof(WcdTextViewActivity));
					intent.PutExtra("dir_path", dirPath.Text);
					StartActivity(intent);
					break;
					case Resource.Id.nav_settings:
					break;
				}
				drawerLayout.CloseDrawers();
			};

			// 기본 화면 구성
			dirPath = FindViewById<EditText>(Resource.Id.dirPathTextView);
			dirPath.Text = DirPath;

			fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
			fab.Click += (sender, e) =>
			{
				var intent = new Intent(this, typeof(FilePickerActivity));
				intent.PutExtra("dir_path", dirPath.Text);
				StartActivityForResult(intent, 1);
			};

			folderPickerButton = FindViewById<Button>(Resource.Id.folderPickerButton);
			folderPickerButton.Click += (sender, e) =>
			{
				var intent = new Intent(this, typeof(FilePickerActivity));
				intent.PutExtra("dir_path", dirPath.Text);
				StartActivityForResult(intent, 1);
			};
			
			//FilePickerDialog filePickerDialog = new FilePickerDialog();
			//filePickerDialog.Show(FragmentManager, "dialog fragment");

			wcdListViewButton = FindViewById<Button>(Resource.Id.button1);
			wcdListViewButton.Click += (sender, e) =>
			{
				var intent = new Intent(this, typeof(WcdListViewActivity));
				intent.PutExtra("dir_path", dirPath.Text);
				StartActivity(intent);
			};

			wcdTextButton = FindViewById<Button>(Resource.Id.button2);
			wcdTextButton.Click += (sender, e) =>
			{
				var intent = new Intent(this, typeof(WcdTextViewActivity));
				intent.PutExtra("dir_path", dirPath.Text);
				StartActivity(intent);
			};
		}

		protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if (resultCode == Result.Ok && requestCode == 1) {
				dirPath.Text = data.GetStringExtra("dir_path") ?? Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
				DirPath = dirPath.Text;
			}
		}

		protected override void OnStop()
		{
			if (DirPath != dirPath.Text)
				DirPath = dirPath.Text;
			base.OnStop();
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
