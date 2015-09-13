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
using Android.Graphics;

namespace HI5Controller
{
	[Activity(Label = "용접 조건 데이터", MainLauncher = false, Icon = "@drawable/robot_industrial", Theme = "@style/MyTheme")]
	public class WcdListViewActivity : AppCompatActivity
	{
		private List<WeldConditionData> mItems;
		private ListView mListView;
		private WcdListViewAdapter adapter;

		private readonly Color defaultBackgroundColor = Color.Transparent;
		private readonly Color selectedBackGroundColor = Color.LightGray;

		async private Task<List<WeldConditionData>> ReadFile(string fileName, List<WeldConditionData> items)
		{
			//StreamReader sr = new StreamReader(Assets.Open(fileName));
			try {
				using (StreamReader sr = new StreamReader(fileName)) {
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
					Toast.MakeText(this, "불러 오기: " + fileName + "", ToastLength.Short).Show();
				}
			} catch {
				Toast.MakeText(this, "파일이 없습니다: " + fileName + "", ToastLength.Short).Show();
				Finish();
			}
			return items;
		}

		async private Task<string> UpdateFile(string fileName, List<WeldConditionData> items)
		{
			//StreamReader sr = new StreamReader(Assets.Open(fileName));
			StringBuilder sb = new StringBuilder();
			try {
				using (StreamReader sr = new StreamReader(fileName)) {
					string swdLine = string.Empty;
					bool addText = true;
					bool wcdText = true;
					while ((swdLine = await sr.ReadLineAsync()) != null) {
						if (addText == false && wcdText) {
							foreach (WeldConditionData wcd in items) {
								sb.Append(wcd.WcdString);
								sb.Append("\n");
							}
							sb.Append("\n");
							wcdText = false;
						}
						if (swdLine.StartsWith("#006"))
							addText = true;
						if (addText /*&& swdLine.Length > 0*/) {
							sb.Append(swdLine);
							sb.Append("\n");
						}
						if (swdLine.StartsWith("#005"))
							addText = false;
					}
					sr.Close();
				}
			} catch {
				Toast.MakeText(this, "읽기 실패: " + fileName + "", ToastLength.Short).Show();
			}

			try {
				using (var sw = new StreamWriter(fileName)) {
					await sw.WriteAsync(sb.ToString());
					sw.Close();
					Toast.MakeText(this, "저장 완료: " + fileName + "", ToastLength.Short).Show();
				}
			} catch {
				Toast.MakeText(this, "쓰기 실패: " + fileName + "", ToastLength.Short).Show();
			}
			//Log.Error("===============", sb.ToString());

			return sb.ToString();
		}

		private Color GetArgb(int argb)
		{
			return Color.Argb(Color.GetAlphaComponent(argb), Color.GetRedComponent(argb), Color.GetGreenComponent(argb), Color.GetBlueComponent(argb));
		}

		async protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.WcdListViewCard);

			string dirPath = System.IO.Path.Combine(Intent.GetStringExtra("dir_path") ?? "", "ROBOT.SWD");

			mItems = new List<WeldConditionData>();
			adapter = new WcdListViewAdapter(this, await ReadFile(dirPath, mItems));

			mListView = FindViewById<ListView>(Resource.Id.myListView);
			mListView.Adapter = adapter;
			mListView.ChoiceMode = ChoiceMode.Multiple;
			mListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
			{
				//adapter[e.Position].PannelThickness = (Convert.ToDecimal(adapter[e.Position].PannelThickness) + 1).ToString();
				//adapter.NotifyDataSetChanged();
				//Console.WriteLine(e.Position.ToString() + " (" + adapter[e.Position].PannelThickness.ToString() + ")");

				if (mListView.IsItemChecked(e.Position)) {
					e.View.SetBackgroundColor(selectedBackGroundColor);
				} else {
					e.View.SetBackgroundColor(defaultBackgroundColor);	// 기본 백그라운드 색깔
				}
				//Toast.MakeText(this, e.Position.ToString() + " (" + mListView.CheckedItemCount.ToString() + ")", ToastLength.Short).Show();
			};
			mListView.ItemLongClick += async (object sender, AdapterView.ItemLongClickEventArgs e) =>
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

				await UpdateFile(dirPath, mItems);
			};

			Button button = FindViewById<Button>(Resource.Id.btnWcdEdit);
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