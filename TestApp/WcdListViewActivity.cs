using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Support.V4.Widget;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using DialogFragment = Android.Support.V4.App.DialogFragment;
using FloatingActionButton = Android.Support.Design.Widget.FloatingActionButton;
using com.xamarin.recipes.filepicker;

using System.IO;
using Android.Util;
using Java.Lang;
using System.Threading.Tasks;
using Android.Graphics;
using System;

namespace HI5Controller
{
	[Activity(Label = "용접 조건 데이터", MainLauncher = false, Icon = "@drawable/robot_industrial", Theme = "@style/MyTheme")]
	public class WcdListViewActivity : AppCompatActivity
	{
		private Toolbar toolbar;
		private DrawerLayout drawerLayout;
		private NavigationView navigationView;
		private FloatingActionButton fabWcd;

		private string dirPath = string.Empty;

		private List<WeldConditionData> mItems;
		private ListView mListView;
		private WcdListViewAdapter adapter;

		private readonly Color defaultBackgroundColor = Color.Transparent;
		private readonly Color selectedBackGroundColor = Color.LightGray;

		async private Task<List<WeldConditionData>> ReadFileAsync(string fileName, List<WeldConditionData> items)
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
					//Toast.MakeText(this, "불러 오기: " + fileName + "", ToastLength.Short).Show();
				}
			} catch {
				Toast.MakeText(this, "파일이 없습니다: " + fileName + "", ToastLength.Short).Show();
				Finish();
			}
			return items;
		}

		async private Task<string> UpdateFileAsync(string fileName, List<WeldConditionData> items)
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

		private List<WeldConditionData> ReadFile(string fileName, List<WeldConditionData> items)
		{
			//StreamReader sr = new StreamReader(Assets.Open(fileName));
			try {
				using (StreamReader sr = new StreamReader(fileName)) {
					string swdLine = string.Empty;
					bool addText = false;
					while ((swdLine = sr.ReadLine()) != null) {
						if (swdLine.StartsWith("#006"))
							break;
						if (addText && swdLine.Trim().Length > 0)
							items.Add(new WeldConditionData(swdLine));
						if (swdLine.StartsWith("#005"))
							addText = true;
					}
					sr.Close();
					//Toast.MakeText(this, "불러 오기: " + fileName + "", ToastLength.Short).Show();
				}
			} catch {
				Toast.MakeText(this, "파일이 없습니다: " + fileName + "", ToastLength.Short).Show();
				Finish();
			}
			return items;
		}

		private string UpdateFile(string fileName, List<WeldConditionData> items)
		{
			//StreamReader sr = new StreamReader(Assets.Open(fileName));
			StringBuilder sb = new StringBuilder();
			try {
				using (StreamReader sr = new StreamReader(fileName)) {
					string swdLine = string.Empty;
					bool addText = true;
					bool wcdText = true;
					while ((swdLine = sr.ReadLine()) != null) {
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
					sw.Write(sb.ToString());
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
			SetContentView(Resource.Layout.WcdListView);

			// 액션바
			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			SetSupportActionBar(toolbar);
			SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu_white);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			SupportActionBar.Title = Resources.GetString(Resource.String.WcdListViewName);
			SupportActionBar.Hide();

			// 서랍 메뉴
			dirPath = Intent.GetStringExtra("dir_path") ?? "";
			drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
			navigationView.NavigationItemSelected += (sender, e) =>
			{
				e.MenuItem.SetChecked(true);
				Intent intent;
				switch (e.MenuItem.ItemId) {
					case Resource.Id.nav_workpathconfig:
					intent = new Intent(this, typeof(FilePickerActivity));
					intent.PutExtra("dir_path", dirPath);
					StartActivityForResult(intent, 1);
					break;
					case Resource.Id.nav_wcd:
					intent = new Intent(this, typeof(WcdListViewActivity));
					intent.PutExtra("dir_path", dirPath);
					StartActivity(intent);
					break;
					case Resource.Id.nav_robot:
					intent = new Intent(this, typeof(WcdTextViewActivity));
					intent.PutExtra("dir_path", dirPath);
					StartActivity(intent);
					break;
				}
				drawerLayout.CloseDrawers();
			};

			// 떠 있는 액션버튼
			fabWcd = FindViewById<FloatingActionButton>(Resource.Id.fab_wcd);
			fabWcd.Click += (object sender, EventArgs e) =>
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
					Toast.MakeText(this, "선택: " + sb.ToString(), ToastLength.Short).Show();
				}
				Finish();
			};

			string robotPath = System.IO.Path.Combine(dirPath, "ROBOT.SWD");
			mItems = new List<WeldConditionData>();
			adapter = new WcdListViewAdapter(this, await ReadFileAsync(robotPath, mItems));
			//adapter = new WcdListViewAdapter(this, ReadFile(robotPath, mItems));

			mListView = FindViewById<ListView>(Resource.Id.myListView);
			mListView.FastScrollEnabled = true;
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
					e.View.SetBackgroundColor(defaultBackgroundColor);  // 기본 백그라운드 색깔
				}
				//Toast.MakeText(this, e.Position.ToString() + " (" + mListView.CheckedItemCount.ToString() + ")", ToastLength.Short).Show();
			};
			//mListView.ItemLongClick += async (object sender, AdapterView.ItemLongClickEventArgs e) =>
			//mListView.ItemLongClick += (object sender, AdapterView.ItemLongClickEventArgs e) =>
			//{
			//	SparseBooleanArray checkedList = mListView.CheckedItemPositions;
			//	List<int> positions = new List<int>();
			//	for (int i = 0; i < checkedList.Size(); i++) {
			//		if (checkedList.ValueAt(i)) {
			//			positions.Add(checkedList.KeyAt(i));
			//		}
			//	}

			//	if (positions.Count > 0) {
			//		StringBuilder sb = new StringBuilder();
			//		foreach (int pos in positions) {
			//			sb.Append(pos);
			//			sb.Append(" ");
			//		}
			//		Toast.MakeText(this, sb.ToString(), ToastLength.Short).Show();
			//	}

			//	UpdateFile(dirPath, mItems);
			//};
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
