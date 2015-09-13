using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Android.Util;
using Java.Util;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using com.xamarin.recipes.filepicker;
using Android.Support.V4.App;
using System.Text;

namespace HI5Controller
{
	[Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/robot_industrial", Theme = "@style/MyTheme")]
	public class WcdActivity : AppCompatActivity
	{
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

			dirPath = FindViewById<EditText>(Resource.Id.dirPathTextView);
			dirPath.Text = DirPath;
			//dirPath.TextChanged += (sender, e) =>
			//{ };

			folderPickerButton = FindViewById<Button>(Resource.Id.folderPickerButton);
			folderPickerButton.Click += (sender, e) =>
			{
				var intent = new Intent(this, typeof(FilePickerActivity));
				intent.PutExtra("dir_path", dirPath.Text);
				StartActivityForResult(intent, 1);
			};

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
	}
}
