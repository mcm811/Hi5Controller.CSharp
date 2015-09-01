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
using Android.Support.V7.Widget;

namespace SwdEditor
{
	[Activity(Label = "SWD 편집기", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/MyCustomTheme")]
	public class MainActivity : Activity
	{
        protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.Main);

			//RequestWindowFeature(WindowFeatures.NoTitle);
			//this.Window.AddFlags(WindowManagerFlags.Fullscreen);
			//this.Window.ClearFlags(WindowManagerFlags.Fullscreen);

			CardView cv = FindViewById<CardView>(Resource.Id.cardView);
			ImageButton tp = FindViewById <ImageButton>(Resource.Id.imageButton1);
			Button bt = FindViewById<Button>(Resource.Id.openButton);
			TextView stv = FindViewById<TextView>(Resource.Id.scrollTextView);

			tp.Click += (sender, e) => {
				var intent = new Intent(this, typeof(SwdActivity));
				List<string> weldConditionData = SwdFile.GetList(stv.Text);
				intent.PutStringArrayListExtra("weld_condition_data", weldConditionData);
				StartActivity(intent);
			};

			bt.Click += async (sender, e) => {
				try {
					StreamReader sr = new StreamReader(Assets.Open("ROBOT.SWD"));
					stv.Text = await sr.ReadToEndAsync();
				} catch {
					Log.Error("swd", "StreamReader");
				}
			};

			//var toast = Toast.MakeText(this, "시계를 터치하세요", ToastLength.Long);
			//toast.Show();
		}
	}
}
