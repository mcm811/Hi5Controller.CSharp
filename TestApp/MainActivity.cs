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
			//Window.RequestFeature(WindowFeatures.NoTitle);
			//Window.AddFlags(WindowManagerFlags.Fullscreen);
			//Window.ClearFlags(WindowManagerFlags.Fullscreen);

			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main);

			Button bt1 = FindViewById<Button>(Resource.Id.button1);
			bt1.Click += (sender, e) => {
				var intent = new Intent(this, typeof(WcdListViewActivity));
				//intent.PutStringArrayListExtra("weld_condition_data", SwdFile.GetStringArrayList(stv.Text));
				StartActivity(intent);
			};

			Button bt2 = FindViewById<Button>(Resource.Id.button2);
			bt2.Click += (sender, e) => {
				var intent = new Intent(this, typeof(WcdTextViewActivity));
				//intent.PutStringArrayListExtra("weld_condition_data", SwdFile.GetStringArrayList(stv.Text));
				StartActivity(intent);
			};

			Button bt3 = FindViewById<Button>(Resource.Id.button3);
			bt3.Click += (sender, e) => {
				//var intent = new Intent(this, typeof(WcdTextViewActivity));
				//intent.PutStringArrayListExtra("weld_condition_data", SwdFile.GetStringArrayList(stv.Text));
				//StartActivity(intent);
			};
		}
	}
}
