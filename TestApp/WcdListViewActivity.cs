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
using com.xamarin.recipes.filepicker;

using System.IO;
using Android.Util;
using Java.Lang;
using System.Threading.Tasks;
using Android.Graphics;
using System;
using Android.Text;

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
		private string robotPath = string.Empty;

		private List<WeldConditionData> mItems;
		private ListView mListView;
		private WcdListViewAdapter adapter;

		private int lastPosition = 0;

		private const float alphaOff = 0.2f;
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
			View header = navigationView.InflateHeaderView(Resource.Layout.drawer_header_layout);
			RelativeLayout drawerHeader = header.FindViewById<RelativeLayout>(Resource.Id.drawerHeader);
			drawerHeader.Click += (sender, e) =>
			{
				var intent = new Intent(this, typeof(WcdActivity));
				StartActivity(intent);
			};

			robotPath = System.IO.Path.Combine(dirPath, "ROBOT.SWD");
			mItems = new List<WeldConditionData>();
			adapter = new WcdListViewAdapter(this, await ReadFileAsync(robotPath, mItems));

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

				//내용 수정 방법
				//adapter[e.Position].PannelThickness = (Convert.ToDecimal(adapter[e.Position].PannelThickness) + 1).ToString();
				//adapter.NotifyDataSetChanged();
				//Console.WriteLine(e.Position.ToString() + " (" + adapter[e.Position].PannelThickness.ToString() + ")");
				//Toast.MakeText(this, e.Position.ToString() + " (" + mListView.CheckedItemCount.ToString() + ")", ToastLength.Short).Show();
			};
			mListView.ItemLongClick += (object sender, AdapterView.ItemLongClickEventArgs e) =>
			{
				lastPosition = e.Position;
				FabWcd_Click(sender, e);
			};

			// 떠 있는 액션버튼
			fabWcd = FindViewById<FloatingActionButton>(Resource.Id.fab_wcd);
			fabWcd.Click += FabWcd_Click;
			//fabWcd.Click += (object sender, EventArgs e) => { };
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

			// 에디트텍스트
			IList<EditText> editTextList = new List<EditText>();
			editTextList.Add(editFieldView.FindViewById<EditText>(Resource.Id.etOutputData));
			editTextList.Add(editFieldView.FindViewById<EditText>(Resource.Id.etOutputType));
			editTextList.Add(editFieldView.FindViewById<EditText>(Resource.Id.etSqueezeForce));
			editTextList.Add(editFieldView.FindViewById<EditText>(Resource.Id.etMoveTipClearance));
			editTextList.Add(editFieldView.FindViewById<EditText>(Resource.Id.etFixedTipClearance));
			editTextList.Add(editFieldView.FindViewById<EditText>(Resource.Id.etPannelThickness));
			editTextList.Add(editFieldView.FindViewById<EditText>(Resource.Id.etCommandOffset));
			editTextList.Add(editFieldView.FindViewById<EditText>(Resource.Id.etBegin));
			editTextList.Add(editFieldView.FindViewById<EditText>(Resource.Id.etEnd));
			int[] etMax = { 1000, 100, 350, 500, 500, 500, 500, 1000, 1000 };

			IList<TextInputLayout> textInputLayoutList = new List<TextInputLayout>();
			textInputLayoutList.Add(editFieldView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout1));
			textInputLayoutList.Add(editFieldView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout2));
			textInputLayoutList.Add(editFieldView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout3));
			textInputLayoutList.Add(editFieldView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout4));
			textInputLayoutList.Add(editFieldView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout5));
			textInputLayoutList.Add(editFieldView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout6));
			textInputLayoutList.Add(editFieldView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout7));
			textInputLayoutList.Add(editFieldView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout8));
			textInputLayoutList.Add(editFieldView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout9));

			//adapter[e.Position].PannelThickness = (Convert.ToDecimal(adapter[e.Position].PannelThickness) + 1).ToString();
			//adapter.NotifyDataSetChanged();
			//Console.WriteLine(e.Position.ToString() + " (" + adapter[e.Position].PannelThickness.ToString() + ")");
			//Toast.MakeText(this, e.Position.ToString() + " (" + mListView.CheckedItemCount.ToString() + ")", ToastLength.Short).Show();

			// 기본으로 에디트텍스트를 안보이게 하고 텍스트뷰를 선택시 에디트텍스트가 보이게 이벤트 처리
			for (int i = 0; i < editTextList.Count; i++) {
				EditText et = editTextList[i];
				// 기본선택된 자료값 가져오기
				if (i < adapter[lastPosition].Count) {
					et.Text = adapter[lastPosition][i];
					et.SetTextColor(Color.RosyBrown);
					int maxValue = etMax[i];
					et.TextChanged += (object sender1, TextChangedEventArgs e1) =>
					{
						// 임계치 설정
						int n = Convert.ToInt32(e1.Text.ToString());
						if (n > maxValue) {
							n = maxValue;
							et.Text = n.ToString();
						}
					};
				}
				et.SetTextSize(ComplexUnitType.Sp, 10);
				//textInputLayoutList[i].ScaleX = 0.9f;
				//textInputLayoutList[i].ScaleY = 0.9f;
				if (i == 0)
					continue;
				if (i != 2)
					et.Alpha = alphaOff;
				et.Click += (sender1, e1) =>
				{
					et.Alpha = et.Alpha == alphaOff ? 1 : alphaOff;
				};
				textInputLayoutList[i].Click += (sender1, e1) =>
				{
					et.Alpha = et.Alpha == alphaOff ? 1 : alphaOff;
				};
			}

			var statusText = editFieldView.FindViewById<TextView>(Resource.Id.statusText);
			statusText.Text = adapter[lastPosition][0];
			if (positions.Count > 0) {
				StringBuilder sb = new StringBuilder();
				foreach (int pos in positions) {
					sb.Append(pos);
					sb.Append(" ");
				}
				statusText.Text = sb.ToString();
			}

			var sampleSeekBar = editFieldView.FindViewById<SeekBar>(Resource.Id.sampleSeekBar);
			sampleSeekBar.Max = adapter.Count - 1;
			sampleSeekBar.Progress = Convert.ToInt32(adapter[lastPosition][0]) - 1;
			sampleSeekBar.ProgressChanged += (object sender1, SeekBar.ProgressChangedEventArgs e1) =>
			{
				//statusText.Text = (e1.Progress + 1).ToString();
				for (int i = 0; i < adapter[sampleSeekBar.Progress].Count; i++) {
					editTextList[i].Text = adapter[sampleSeekBar.Progress][i];
					editTextList[i].SetTextColor(Color.RosyBrown);
				}
			};

			var beginSeekBar = editFieldView.FindViewById<SeekBar>(Resource.Id.sbBegin);
			var endSeekBar = editFieldView.FindViewById<SeekBar>(Resource.Id.sbEnd);

			beginSeekBar.Max = adapter.Count;
			beginSeekBar.Progress = beginSeekBar.Max / 2;

			endSeekBar.Max = adapter.Count;
			endSeekBar.Progress = endSeekBar.Max / 2;

			// 선택 시작
			editTextList[7].Text = string.Format("{0}", beginSeekBar.Progress + 1);
			editTextList[7].SetTextColor(Color.RosyBrown);
			// 선택 끝
			editTextList[8].Text = string.Format("{0}", endSeekBar.Progress + 1);
			editTextList[8].SetTextColor(Color.RosyBrown);

			editTextList[7].TextChanged += (object sender1, TextChangedEventArgs e1) =>
			{
				int n = Convert.ToInt32(e1.Text.ToString());
				if (n > beginSeekBar.Max) {
					n = beginSeekBar.Max;
					editTextList[7].Text = n.ToString();
				}
				beginSeekBar.Progress = n;
			};
			editTextList[8].TextChanged += (object sender1, TextChangedEventArgs e1) =>
			{
				int n = Convert.ToInt32(e1.Text.ToString());
				if (n > endSeekBar.Max) {
					n = endSeekBar.Max;
					editTextList[8].Text = n.ToString();
				}
				endSeekBar.Progress = endSeekBar.Max - n - 1;
			};

			beginSeekBar.ProgressChanged += (object sender1, SeekBar.ProgressChangedEventArgs e1) =>
			{
				if (e1.FromUser) {
					int sb1Progress = beginSeekBar.Progress;
					int sb2Progress = endSeekBar.Max - endSeekBar.Progress;
					editTextList[7].Text = string.Format("{0}", sb1Progress + 1);
					if (sb1Progress > sb2Progress) {
						endSeekBar.Progress = endSeekBar.Max - beginSeekBar.Progress;
						editTextList[8].Text = string.Format("{0}", sb1Progress + 1);
					}
					if (sb1Progress == 1 && sb2Progress == 1 || sb1Progress == beginSeekBar.Max && sb2Progress == endSeekBar.Max) {
						editTextList[7].Alpha = alphaOff;
						editTextList[8].Alpha = alphaOff;
					} else {
						editTextList[7].Alpha = 1;
						editTextList[8].Alpha = 1;
					}
				}
			};

			endSeekBar.ProgressChanged += (object sender2, SeekBar.ProgressChangedEventArgs e2) =>
			{
				if (e2.FromUser) {
					int sb1Progress = beginSeekBar.Progress;
					int sb2Progress = endSeekBar.Max - endSeekBar.Progress;
					editTextList[8].Text = string.Format("{0}", sb2Progress + 1);
					if (sb1Progress > sb2Progress) {
						beginSeekBar.Progress = sb2Progress;
						editTextList[7].Text = string.Format("{0}", sb2Progress + 1);
					}
					if (sb1Progress <= 1 && sb2Progress <= 1 || sb1Progress >= beginSeekBar.Max && sb2Progress >= endSeekBar.Max) {
						editTextList[7].Alpha = alphaOff;
						editTextList[8].Alpha = alphaOff;
					} else {
						editTextList[7].Alpha = 1;
						editTextList[8].Alpha = 1;
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
				}
			});

			dialog.SetPositiveButton("확인", async delegate
			{
				var seekBegin = editTextList[7].Alpha == 1 ? Convert.ToInt32(editTextList[7].Text) : 0;
				var seekEnd = editTextList[8].Alpha == 1 ? Convert.ToInt32(editTextList[8].Text) : 0;

				if (seekBegin > 0 && seekEnd > seekBegin) {
					for (int rowNum = seekBegin - 1; rowNum < seekEnd; rowNum++) {
						for (int colNum = 1; colNum < adapter[rowNum].Count; colNum++) {
							if (editTextList[colNum].Text != "" && editTextList[colNum].Alpha == 1) {
								adapter[rowNum][colNum] = editTextList[colNum].Text;
							}
						}
						mListView.SetItemChecked(rowNum, false);
						adapter[rowNum].ItemChecked = false;
					}
					adapter.NotifyDataSetChanged();
					await UpdateFileAsync(robotPath, mItems);
				} else if (positions.Count > 0) {
					foreach (int rowNum in positions) {
						for (int colNum = 1; colNum < adapter[rowNum].Count; colNum++) {
							if (editTextList[colNum].Text != "" && editTextList[colNum].Alpha == 1) {
								adapter[rowNum][colNum] = editTextList[colNum].Text;
							}
						}
						mListView.SetItemChecked(rowNum, false);
						adapter[rowNum].ItemChecked = false;
					}
					adapter.NotifyDataSetChanged();
					await UpdateFileAsync(robotPath, mItems);
				} else {
					int rowNum = lastPosition;
					if (editTextList[0].Alpha == 1) rowNum = Convert.ToInt32(editTextList[0].Text) - 1;
					for (int colNum = 1; colNum < adapter[rowNum].Count; colNum++) {
						if (editTextList[colNum].Text != "" && editTextList[colNum].Alpha == 1) {
							adapter[rowNum][colNum] = editTextList[colNum].Text;
						}
					}
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
