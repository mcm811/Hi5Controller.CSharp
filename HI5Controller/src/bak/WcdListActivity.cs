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
using AlertDialog = Android.Support.V7.App.AlertDialog;
using EditText = Android.Support.V7.Widget.AppCompatEditText;
using com.xamarin.recipes.filepicker;

using System.IO;
using Android.Util;
using Java.Lang;
using System.Threading.Tasks;
using Android.Graphics;
using System;
using Android.Text;

namespace Com.Changmin.HI5Controller.src
{
	[Activity(Label = "용접 조건 데이터", MainLauncher = false, Icon = "@drawable/robot_industrial", Theme = "@style/MyTheme")]
	public class WcdListActivity : AppCompatActivity
	{
		private Toolbar toolbar;
		private DrawerLayout drawerLayout;
		private NavigationView navigationView;
		private FloatingActionButton fabWcd;

		private string dirPath = string.Empty;
		private string robotPath = string.Empty;

		private List<WeldConditionData> mItems;
		private ListView mListView;
		private WcdListAdapter adapter;

		private int lastPosition = 0;

		private const float alphaOff = 0.2f;
		private readonly Color defaultBackgroundColor = Color.Transparent;
		private readonly Color selectedBackGroundColor = Color.LightGray;

		private void LogDebug(string msg)
		{
			Log.Debug(Application.PackageName, msg);
		}

		async private Task<List<WeldConditionData>> ReadFileAsync(string fileName, List<WeldConditionData> items)
		{
			try {
				//using (StreamReader sr = new StreamReader(Assets.Open(fileName))) {
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
					LogDebug("불러 오기: " + fileName);
				}
			} catch {
				ToastShow("읽기 실패: " + fileName);
			}
			return items;
		}

		async private Task<string> UpdateFileAsync(string fileName, List<WeldConditionData> items)
		{
			StringBuilder sb = new StringBuilder();
			try {
				//using (StreamReader sr = new StreamReader(Assets.Open(fileName))) {
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
				ToastShow("읽기 실패: " + fileName);
			}

			try {
				using (var sw = new StreamWriter(fileName)) {
					await sw.WriteAsync(sb.ToString());
					sw.Close();
					ToastShow("저장 완료: " + fileName);
				}
			} catch {
				ToastShow("저장 실패: " + fileName);
			}

			return sb.ToString();
		}

		private List<WeldConditionData> ReadFile(string fileName, out List<WeldConditionData> items)
		{
			items = new List<WeldConditionData>();
			try {
				//using (StreamReader sr = new StreamReader(Assets.Open(fileName))) {
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
					LogDebug("불러 오기:" + fileName);
				}
			} catch {
				ToastShow("읽기 실패:" + fileName);
			}
			return items;
		}

		private string UpdateFile(string fileName, List<WeldConditionData> items)
		{
			StringBuilder sb = new StringBuilder();
			try {
				//using (StreamReader sr = new StreamReader(Assets.Open(fileName))) {
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
				ToastShow("읽기 실패: " + fileName);
			}

			try {
				using (var sw = new StreamWriter(fileName)) {
					sw.Write(sb.ToString());
					sw.Close();
					ToastShow("저장 완료:" + fileName);
				}
			} catch {
				ToastShow("저장 실패:" + fileName);
			}

			return sb.ToString();
		}

		private Color GetArgb(int argb)
		{
			return Color.Argb(Color.GetAlphaComponent(argb), Color.GetRedComponent(argb), Color.GetGreenComponent(argb), Color.GetBlueComponent(argb));
		}

		private void ToastShow(string str)
		{
			Toast.MakeText(this, str, ToastLength.Short).Show();
			LogDebug(str);
		}

		private void SnackbarShow(View viewParent, string str)
		{
			Snackbar.Make(viewParent, str, Snackbar.LengthLong)
					.SetAction("Undo", (view) => { /*Undo message sending here.*/ })
					.Show(); // Don’t forget to show!
		}

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.WcdList);
			Window.AddFlags(WindowManagerFlags.Fullscreen);

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
					case Resource.Id.nav_wcdpath:
					intent = new Intent(this, typeof(FilePickerActivity));
					intent.PutExtra("dir_path", dirPath);
					StartActivityForResult(intent, 1);
					break;
					case Resource.Id.nav_wcdlist:
					intent = new Intent(this, typeof(WcdListActivity));
					intent.PutExtra("dir_path", dirPath);
					StartActivity(intent);
					break;
					case Resource.Id.nav_robotswd:
					intent = new Intent(this, typeof(WcdTextActivity));
					intent.PutExtra("dir_path", dirPath);
					StartActivity(intent);
					break;
				}
				drawerLayout.CloseDrawers();
			};
			View header = navigationView.InflateHeaderView(Resource.Layout.drawer_header_layout);
			RelativeLayout drawerHeader = header.FindViewById<RelativeLayout>(Resource.Id.drawerHeader);
			drawerHeader.Click += (sender, e) =>
			{
				var intent = new Intent(this, typeof(WcdActivity));
				StartActivity(intent);
			};

			robotPath = System.IO.Path.Combine(dirPath, "ROBOT.SWD");
			adapter = new WcdListAdapter(this, ReadFile(robotPath, out mItems));

			mListView = FindViewById<ListView>(Resource.Id.myListView);
			mListView.FastScrollEnabled = true;
			mListView.Adapter = adapter;
			mListView.ChoiceMode = ChoiceMode.Multiple;
			mListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
			{
				adapter[e.Position].ItemChecked = mListView.IsItemChecked(e.Position);
				if (mListView.IsItemChecked(e.Position)) {
					lastPosition = e.Position;
					e.View.SetBackgroundColor(selectedBackGroundColor);
				} else {
					e.View.SetBackgroundColor(defaultBackgroundColor);  // 기본 백그라운드 색깔
				}
			};
			mListView.ItemLongClick += (object sender, AdapterView.ItemLongClickEventArgs e) =>
			{
				lastPosition = e.Position;
				FabWcd_Click(sender, e);
			};
			// 떠 있는 액션버튼
			fabWcd = FindViewById<FloatingActionButton>(Resource.Id.fab_wcd);
			fabWcd.Click += FabWcd_Click;
		}

		private void FabWcd_Click(object sender, EventArgs e)
		{
			SparseBooleanArray checkedList = mListView.CheckedItemPositions;
			List<int> positions = new List<int>();
			for (int i = 0; i < checkedList.Size(); i++) {
				if (checkedList.ValueAt(i)) {
					positions.Add(checkedList.KeyAt(i));
				}
			}
			if (positions.Count == 0)
				lastPosition = 0;

			View editFieldView = LayoutInflater.From(this).Inflate(Resource.Layout.WcdEditor, null);
			AlertDialog.Builder dialog = new AlertDialog.Builder(this);
			dialog.SetView(editFieldView);

			//IList<TextInputLayout> tilList = new List<TextInputLayout>();
			//tilList.Add(editFieldView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout1));
			//tilList.Add(editFieldView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout2));
			//tilList.Add(editFieldView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout3));
			//tilList.Add(editFieldView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout4));
			//tilList.Add(editFieldView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout5));
			//tilList.Add(editFieldView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout6));
			//tilList.Add(editFieldView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout7));

			// 에디트텍스트
			IList<EditText> etList = new List<EditText>();
			etList.Add(editFieldView.FindViewById<EditText>(Resource.Id.etOutputData));
			etList.Add(editFieldView.FindViewById<EditText>(Resource.Id.etOutputType));
			etList.Add(editFieldView.FindViewById<EditText>(Resource.Id.etSqueezeForce));
			etList.Add(editFieldView.FindViewById<EditText>(Resource.Id.etMoveTipClearance));
			etList.Add(editFieldView.FindViewById<EditText>(Resource.Id.etFixedTipClearance));
			etList.Add(editFieldView.FindViewById<EditText>(Resource.Id.etPannelThickness));
			etList.Add(editFieldView.FindViewById<EditText>(Resource.Id.etCommandOffset));

			int[] etMax = { 1000, 100, 350, 500, 500, 500, 500, 1000, 1000 };   // 임계치
			for (int i = 0; i < etList.Count; i++) {
				EditText et = etList[i];
				//et.SetTextSize(ComplexUnitType.Sp, 12);
				//et.ScaleX = 0.8f;
				//et.ScaleY = 0.8f;
				//tilList[i].ScaleX = 0.8f;
				//tilList[i].ScaleY = 0.8f;

				if (i == 0)                                                 // outputData
					et.Text = adapter[lastPosition][i];                     // 기본선택된 자료값 가져오기

				int maxValue = etMax[i];                                    // 임계치 설정
				et.TextChanged += (object sender1, TextChangedEventArgs e1) =>
				{
					int n;
					try {
						n = Convert.ToInt32(e1.Text.ToString());
						if (n > maxValue) {
							n = maxValue;
							et.Text = n.ToString();
						}
					} catch {       // Convert는 다른 타입이 들어오면 예외가 발생 된다
						if (Int32.TryParse(e1.Text.ToString(), out n)) {
							if (n > maxValue) {
								n = maxValue;
								et.Text = n.ToString();
							}
						}
					}
				};
			}

			var statusText = editFieldView.FindViewById<TextView>(Resource.Id.statusText);
			statusText.Text = adapter[lastPosition][0];
			if (positions.Count > 0) {
				StringBuilder sb = new StringBuilder();
				foreach (int pos in positions) {
					sb.Append(pos + 1);
					sb.Append(" ");
				}
				statusText.Text = "수정 항목: " + sb.ToString().TrimEnd();
			} else {
				statusText.Text = "수정 항목: " + (lastPosition + 1).ToString();
			}

			var sampleSeekBar = editFieldView.FindViewById<SeekBar>(Resource.Id.sampleSeekBar);
			sampleSeekBar.Max = adapter.Count - 1;
			sampleSeekBar.Progress = Convert.ToInt32(adapter[lastPosition][0]) - 1;
			sampleSeekBar.ProgressChanged += (object sender1, SeekBar.ProgressChangedEventArgs e1) =>
			{
				for (int i = 0; i < adapter[sampleSeekBar.Progress].Count; i++) {
					if (etList[i].Text != "")
						etList[i].Text = adapter[sampleSeekBar.Progress][i];
				}
				if (positions.Count == 0) {
					lastPosition = sampleSeekBar.Progress;
					statusText.Text = "수정 항목: " + (lastPosition + 1).ToString();
				}
			};

			// 선택 시작
			var beginSeekBar = editFieldView.FindViewById<SeekBar>(Resource.Id.sbBegin);
			beginSeekBar.Max = adapter.Count - 1;
			beginSeekBar.Progress = 0;

			// 선택 끝
			var endSeekBar = editFieldView.FindViewById<SeekBar>(Resource.Id.sbEnd);
			endSeekBar.Max = adapter.Count - 1;
			endSeekBar.Progress = endSeekBar.Max;

			//etList[7].TextChanged += (object sender1, TextChangedEventArgs e1) =>
			//{
			//	int n;
			//	try {
			//		n = Convert.ToInt32(e1.Text.ToString());
			//		if (n > beginSeekBar.Max) {
			//			n = beginSeekBar.Max;
			//			etList[7].Text = n.ToString();
			//		}
			//		beginSeekBar.Progress = n;
			//	} catch {
			//		if (Int32.TryParse(e1.Text.ToString(), out n)) {
			//			if (n > beginSeekBar.Max) {
			//				n = beginSeekBar.Max;
			//				etList[7].Text = n.ToString();
			//			}
			//			beginSeekBar.Progress = n;
			//		}
			//	}
			//};
			//etList[8].TextChanged += (object sender1, TextChangedEventArgs e1) =>
			//{
			//	int n;
			//	try {
			//		n = Convert.ToInt32(e1.Text.ToString());
			//		if (n > endSeekBar.Max) {
			//			n = endSeekBar.Max;
			//			etList[8].Text = n.ToString();
			//		}
			//		endSeekBar.Progress = endSeekBar.Max - n - 1;
			//	} catch {
			//		if (Int32.TryParse(e1.Text.ToString(), out n)) {
			//			if (n > endSeekBar.Max) {
			//				n = endSeekBar.Max;
			//				etList[8].Text = n.ToString();
			//			}
			//			endSeekBar.Progress = endSeekBar.Max - n - 1;
			//		}
			//	}
			//};
			beginSeekBar.ProgressChanged += (object sender1, SeekBar.ProgressChangedEventArgs e1) =>
			{
				if (e1.FromUser) {
					int sb1Progress = beginSeekBar.Progress;
					int sb2Progress = endSeekBar.Max - endSeekBar.Progress;
					if (sb1Progress > sb2Progress) {
						sb2Progress = sb1Progress;
						endSeekBar.Progress = endSeekBar.Max - sb1Progress;
					}
					if (sb1Progress == 0 && sb2Progress == 0 || sb1Progress == beginSeekBar.Max && sb2Progress == endSeekBar.Max) {
						if (positions.Count > 0) {
							StringBuilder sb = new StringBuilder();
							foreach (int pos in positions) {
								sb.Append(pos + 1);
								sb.Append(" ");
							}
							statusText.Text = "수정 항목: " + sb.ToString().TrimEnd();
						} else {
							statusText.Text = "수정 항목: " + (lastPosition + 1).ToString();
						}
					} else {
						statusText.Text = "수정 범위: " + string.Format("{0}", sb1Progress + 1) + " ~ " + string.Format("{0}", sb2Progress + 1);
					}
				}
			};
			endSeekBar.ProgressChanged += (object sender2, SeekBar.ProgressChangedEventArgs e2) =>
			{
				if (e2.FromUser) {
					int sb1Progress = beginSeekBar.Progress;
					int sb2Progress = endSeekBar.Max - endSeekBar.Progress;
					if (sb2Progress < sb1Progress) {
						sb1Progress = sb2Progress;
						beginSeekBar.Progress = sb2Progress;
					}
					if (sb1Progress == 0 && sb2Progress == 0 || sb1Progress == beginSeekBar.Max && sb2Progress == endSeekBar.Max) {
						if (positions.Count > 0) {
							StringBuilder sb = new StringBuilder();
							foreach (int pos in positions) {
								sb.Append(pos + 1);
								sb.Append(" ");
							}
							statusText.Text = "수정 항목: " + sb.ToString().TrimEnd();
						} else {
							statusText.Text = "수정 항목: " + (lastPosition + 1).ToString();
						}
					} else {
						statusText.Text = "수정 범위: " + string.Format("{0}", sb1Progress + 1) + " ~ " + string.Format("{0}", sb2Progress + 1);
					}
				}
			};

			dialog.SetNegativeButton("취소", delegate
			{
				if (positions.Count > 0) {
					foreach (int pos in positions) {
						mListView.SetItemChecked(pos, false);
						adapter[pos].ItemChecked = false;
					}
					adapter.NotifyDataSetChanged();
				}
			});

			dialog.SetPositiveButton("확인", async delegate
			{
				int seekBegin = beginSeekBar.Progress + 1;
				int seekEnd = endSeekBar.Max - endSeekBar.Progress + 1;
				bool isSeek = !((seekBegin == 1 && seekEnd == 1) || (seekBegin == beginSeekBar.Max + 1 && seekEnd == endSeekBar.Max + 1));
				bool isUpdate = false;

				if (positions.Count == 0)
					positions.Add(lastPosition);
				if (isSeek) {
					positions.Clear();
					for (int rowNum = seekBegin - 1; rowNum < seekEnd; rowNum++) {
						positions.Add(rowNum);
					}
				}
				foreach (int rowNum in positions) {
					for (int colNum = 1; colNum < adapter[rowNum].Count; colNum++) {
						if (etList[colNum].Text != "") {
							adapter[rowNum][colNum] = etList[colNum].Text;
							isUpdate = true;
						}
					}
					mListView.SetItemChecked(rowNum, false);
					adapter[rowNum].ItemChecked = false;
				}
				if (isUpdate) {
					adapter.NotifyDataSetChanged();
					await UpdateFileAsync(robotPath, mItems);
				}
			});
			dialog.Show();
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
