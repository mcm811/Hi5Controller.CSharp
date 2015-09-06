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

namespace ListViewApp
{
	[Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/MyCustomTheme")]
	public class MainActivity : AppCompatActivity
	{
        protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.Main);

			//RequestWindowFeature(WindowFeatures.NoTitle);
			//this.Window.AddFlags(WindowManagerFlags.Fullscreen);
			//this.Window.ClearFlags(WindowManagerFlags.Fullscreen);

			ImageButton tp = FindViewById <ImageButton>(Resource.Id.imageButton1);
			//Button bt = FindViewById<Button>(Resource.Id.openButton);
			//TextView stv = FindViewById<TextView>(Resource.Id.scrollTextView);

			//if (stv.Text.Length == 0) {
			//	try {
			//		StreamReader sr = new StreamReader(Assets.Open("ROBOT.SWD"));
			//		stv.Text = sr.ReadToEnd();
			//		sr.Close();
			//	} catch {
			//		Log.Error("swd", "Init StreamReader");
			//	}
			//}

			tp.Click += (sender, e) => {
				var intent = new Intent(this, typeof(WcdListViewActivity));
				//intent.PutStringArrayListExtra("weld_condition_data", SwdFile.GetStringArrayList(stv.Text));
				StartActivity(intent);
			};

			//bt.Click += async (sender, e) => {
			//	try {
			//		StreamReader sr = new StreamReader(Assets.Open("ROBOT.SWD"));
			//		stv.Text = await sr.ReadToEndAsync();
			//		sr.Close();
			//	} catch {
			//		Log.Error("swd", "Event Async StreamReader");
			//	}
			//};

			//Toast.MakeText(this, "시계를 터치하세요", ToastLength.Long).Show();
		}
	}
}
