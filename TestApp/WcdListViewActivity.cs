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
using System.Threading.Tasks;

namespace ListViewApp
{
	[Activity(Label = "용접 조건 데이터", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/MyCustomTheme")]
	public class WcdListViewActivity : AppCompatActivity
	{
		private List<WeldConditionData> mItems;
		private ListView mListView;

		async private Task<List<WeldConditionData>> ReadFile(string fileName, List<WeldConditionData> items)
		{
			StreamReader sr = new StreamReader(Assets.Open(fileName));
			string swdLine = string.Empty;
			bool addText = false;
            while ((swdLine = await sr.ReadLineAsync()) != null) {
				if (swdLine.StartsWith("#006"))
					break;
				if (addText && swdLine.Trim().Length > 0)
					items.Add(new WeldConditionData(swdLine));
				if (swdLine.StartsWith("#005"))
					addText = true;
			}
			sr.Close();

			return items;
		}

		async private Task<string> UpdateFile(string fileName, List<WeldConditionData> items)
		{
			StringBuilder sb = new StringBuilder();
			StreamReader sr = new StreamReader(Assets.Open(fileName));
			string swdLine = string.Empty;
			bool addText = true;
			while ((swdLine = await sr.ReadLineAsync()) != null) {
			//foreach (string swdLine in (await ReadAssets(fileName)).Split('\n')) {
				if (addText == false) {
					foreach (WeldConditionData wcd in items) {
						sb.Append(wcd.WcdString);
						sb.Append("\n");
					}
				}
				if (swdLine.StartsWith("#006"))
					addText = true;
				if (addText && swdLine.Length > 0) {
					sb.Append(swdLine);
					sb.Append("\n");
				}
				if (swdLine.StartsWith("#005"))
					addText = false;
			}
			sr.Close();

			return sb.ToString();
		}

		async protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.WcdListView);

			mListView = FindViewById<ListView>(Resource.Id.myListView);

			mItems = new List<WeldConditionData>();
			WcdListViewAdapter adapter = new WcdListViewAdapter(this, await ReadFile("ROBOT.SWD", mItems));

			mListView.Adapter = adapter;
			mListView.ChoiceMode = ChoiceMode.Multiple;
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