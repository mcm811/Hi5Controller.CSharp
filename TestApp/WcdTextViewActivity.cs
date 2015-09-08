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

namespace ListViewApp
{
	[Activity(Label = "ROBOT.SWD", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/MyCustomTheme")]

	public class WcdTextViewActivity : Activity
	{
		async private Task<string> ReadFileToString(string fileName)
		{
			StreamReader sr = new StreamReader(Assets.Open(fileName));
			string st = await sr.ReadToEndAsync();
			sr.Close();

			return st;
		}

		async protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.WcdTextView);

			TextView tv = FindViewById<TextView>(Resource.Id.wcdTextView);
			tv.Text = await ReadFileToString("ROBOT.SWD");

			//tv.Click += (object sender, EventArgs e) =>
			//{ };
		}
	}
}