using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Support.V7.App;
using System.IO;
using Android.Util;
using Java.Lang;

namespace ListViewApp
{
	[Activity(Label = "ListView App", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/MyCustomTheme")]
	public class WcdListViewActivity : AppCompatActivity
	{
		private List<WeldConditionData> mItems;
		private ListView mListView;

		private List<WeldConditionData> ReadFile(string filename, List<WeldConditionData> items)
		{
			StreamReader sr = new StreamReader(Assets.Open("ROBOT.SWD"));
			string swdText = sr.ReadToEnd();
			sr.Close();

			bool addText = false;
			foreach (string swdLine in swdText.Split('\n')) {
				if (swdLine.StartsWith("#006"))
					break;
				if (addText && swdLine.Trim().Length > 0)
					items.Add(new WeldConditionData(swdLine));
				if (swdLine.StartsWith("#005"))
					addText = true;
			}

			return items;
		}

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.WcdListView);
			mListView = FindViewById<ListView>(Resource.Id.myListView);

			mItems = new List<WeldConditionData>();
			WcdListViewAdapter adapter = new WcdListViewAdapter(this, ReadFile("ROBOT.SWD", mItems));

			mListView.Adapter = adapter;
			mListView.ChoiceMode = ChoiceMode.Multiple;
			//mListView.Selector

			mListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
			{
				//adapter[e.Position].PannelThickness = (Convert.ToDecimal(adapter[e.Position].PannelThickness) + 1).ToString();
				//adapter.NotifyDataSetChanged();
				//Console.WriteLine(e.Position.ToString() + " (" + adapter[e.Position].PannelThickness.ToString() + ")");
				Toast.MakeText(this, e.Position.ToString() + " (" + mListView.CheckedItemCount.ToString() + ")", ToastLength.Short).Show();
			};
			mListView.ItemLongClick += (object sender, AdapterView.ItemLongClickEventArgs e) =>
			{
				SparseBooleanArray checkedList = mListView.CheckedItemPositions;
				List<int> positions = new List<int>();
				for (int i = 0; i < checkedList.Size(); i++) {
					if (checkedList.ValueAt(i)) {
                        positions.Add(checkedList.KeyAt(i));
					}
				}

				if (positions.Count > 0) {
					StringBuilder sb = new StringBuilder();
					foreach (int pos in positions) {
						sb.Append(pos);
						sb.Append(" ");
					}
					Toast.MakeText(this, sb.ToString(), ToastLength.Short).Show();
				}
			};
		}
	}
}