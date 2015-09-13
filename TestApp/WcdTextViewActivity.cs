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

namespace HI5Controller
{
	[Activity(Label = "ROBOT.SWD", MainLauncher = false, Icon = "@drawable/robot_industrial", Theme = "@style/MyTheme")]

	public class WcdTextViewActivity : Activity
	{
		private TextView pathTv;
		private TextView wcdTv;

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

			string dirPath = Path.Combine(Intent.GetStringExtra("dir_path") ?? "", "ROBOT.SWD");

			pathTv = FindViewById<TextView>(Resource.Id.pathTextView);
			pathTv.Text = dirPath;

			wcdTv = FindViewById<TextView>(Resource.Id.wcdTextView);
			wcdTv.Text = await ReadFileToString(dirPath);

			//tv.Click += (object sender, EventArgs e) =>
			//{ };

			Button button = FindViewById<Button>(Resource.Id.btnWcdOk);
			button.Click += (object sender, System.EventArgs e) =>
			{
				//Intent intent = new Intent(this, typeof(FilePickerActivity));
				//intent.PutExtra("dir_path", WcdActivity.path);
				//SetResult(Result.Ok, intent);
				Finish();
			};
		}
	}
}