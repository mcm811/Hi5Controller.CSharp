using System;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using FloatingActionButton = Android.Support.Design.Widget.FloatingActionButton;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using EditText = Android.Support.V7.Widget.AppCompatEditText;
using Fragment = Android.Support.V4.App.Fragment;

using System.IO;
using Android.Util;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Views.InputMethods;
using System.Text;

namespace Com.Changyoung.HI5Controller
{
	public class WcdListFragment : Fragment, IRefresh
	{
		private View view;
		private FloatingActionButton fabWcd;

		private string dirPath;
		private string robotPath;

		private ListView listView;
		private WcdListAdapter wcdListAdapter;

		private int lastPosition = 0;

		private const float alphaOff = 0.2f;
		private readonly Color defaultBackgroundColor = Color.Transparent;
		private Color selectedBackGroundColor = Color.LightGray;

		private void LogDebug(string msg)
		{
			Log.Debug(Context.PackageName, "WcdListFragment: " + msg);
		}

		private void ToastShow(string str)
		{
			//Toast.MakeText(Context, str, ToastLength.Short).Show();
			Snackbar.Make(view, str, Snackbar.LengthLong).Show();
			LogDebug(str);
		}

		private void SnackbarShow(View viewParent, string str)
		{
			var sb = Snackbar.Make(viewParent, str, Snackbar.LengthLong);
			//sb.SetAction("Undo", (view) => { /*Undo message sending here.*/ });
			sb.Show(); // Don’t forget to show!
			LogDebug(str);
		}

		private void ReadFile(string fileName, WcdListAdapter items, Context context = null)
		{
			try {
				using (var sr = context != null ? new StreamReader(context.Assets.Open(fileName), Encoding.GetEncoding("euc-kr")) : new StreamReader(fileName, Encoding.GetEncoding("euc-kr"))) {
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
				LogDebug("읽기 실패:" + fileName);
			}
		}

		async private Task<string> UpdateFileAsync(string fileName, WcdListAdapter items, Context context = null)
		{
			StringBuilder sb = new StringBuilder();
			try {
				using (var sr = context != null ? new StreamReader(context.Assets.Open(fileName), Encoding.GetEncoding("euc-kr")) : new StreamReader(fileName, Encoding.GetEncoding("euc-kr"))) {
					string swdLine = string.Empty;
					bool addText = true;
					bool wcdText = true;
					while ((swdLine = await sr.ReadLineAsync()) != null) {
						if (addText == false && wcdText) {
							for (int i = 0; i < items.Count; i++) {
								sb.AppendLine(items[i].WcdString);
							}
							sb.Append("\n");
							wcdText = false;
						}
						if (swdLine.StartsWith("#006"))
							addText = true;
						if (addText) {
							sb.AppendLine(swdLine);
						}
						if (swdLine.StartsWith("#005"))
							addText = false;
					}
					sr.Close();
				}
			} catch {
				LogDebug("읽기 실패: " + fileName);
			}

			try {
				using (var sw = context != null ? new StreamWriter(context.Assets.Open(fileName), Encoding.GetEncoding("euc-kr")) : new StreamWriter(fileName, false, Encoding.GetEncoding("euc-kr"))) {
					sw.Write(sb.ToString());
					sw.Close();
					//ToastShow("저장 완료: " + fileName.Substring(fileName.LastIndexOf('/')));
					ToastShow("저장 완료: " + fileName);
				}
			} catch {
				ToastShow("저장 실패: " + fileName);
			}

			return sb.ToString();
		}

		private Color GetArgb(int argb)
		{
			return Color.Argb(Color.GetAlphaComponent(argb), Color.GetRedComponent(argb), Color.GetGreenComponent(argb), Color.GetBlueComponent(argb));
		}

		public void Refresh(bool forced = false)
		{
			if (wcdListAdapter == null) {
				fabWcd.SetImageResource(Resource.Drawable.ic_refresh_white);
				LogDebug("mWcdListAdapter == null");
				return;
			}

			if (forced || dirPath != Pref.WorkPath || wcdListAdapter.Count == 0) {
				LogDebug("Refresh: " + dirPath + " : " + Pref.WorkPath + " : " + wcdListAdapter.Count.ToString());
				dirPath = Pref.WorkPath;
				robotPath = System.IO.Path.Combine(dirPath, "ROBOT.SWD");
				wcdListAdapter.Clear();
				ReadFile(robotPath, wcdListAdapter);
				wcdListAdapter.NotifyDataSetChanged();
			}

			try {
				SparseBooleanArray checkedList = listView.CheckedItemPositions;
				for (int i = 0; i < checkedList.Size(); i++) {
					if (checkedList.ValueAt(i)) {
						var pos = checkedList.KeyAt(i);
						listView.SetItemChecked(pos, true);
						wcdListAdapter[pos].ItemChecked = true;
					}
				}
				if (wcdListAdapter.Count == 0)
					fabWcd.SetImageResource(Resource.Drawable.ic_refresh_white);
				else if (listView.CheckedItemCount == 0)
					fabWcd.SetImageResource(Resource.Drawable.ic_subject_white);
				else
					fabWcd.SetImageResource(Resource.Drawable.ic_edit_white);
				wcdListAdapter.NotifyDataSetChanged();
			} catch { }
		}

		public override void OnCreate(Bundle bundle)
		{
			LogDebug("OnCreate");
			base.OnCreate(bundle);

			dirPath = Pref.WorkPath;
			robotPath = System.IO.Path.Combine(dirPath, "ROBOT.SWD");
			wcdListAdapter = new WcdListAdapter(Context);
			ReadFile(robotPath, wcdListAdapter);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			LogDebug("OnCreateView");
			view = inflater.Inflate(Resource.Layout.wcd_list_fragment, container, false);

			selectedBackGroundColor = Context.Resources.GetColor(Resource.Color.tab3_textview_background);

			listView = view.FindViewById<ListView>(Resource.Id.wcdListView);
			listView.Adapter = wcdListAdapter;
			listView.FastScrollEnabled = false;
			listView.ChoiceMode = ChoiceMode.Multiple;
			listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
			{
				wcdListAdapter[e.Position].ItemChecked = listView.IsItemChecked(e.Position);
				if (listView.IsItemChecked(e.Position)) {
					lastPosition = e.Position;
					e.View.SetBackgroundColor(selectedBackGroundColor);
				} else {
					e.View.SetBackgroundColor(defaultBackgroundColor);  // 기본 백그라운드 색깔
				}

				try {
					if (wcdListAdapter.Count == 0)
						fabWcd.SetImageResource(Resource.Drawable.ic_refresh_white);
					else if (listView.CheckedItemCount == 0)
						fabWcd.SetImageResource(Resource.Drawable.ic_subject_white);
					else
						fabWcd.SetImageResource(Resource.Drawable.ic_edit_white);
				} catch { }
			};
			listView.ItemLongClick += (object sender, AdapterView.ItemLongClickEventArgs e) =>
			{
				try {
					SparseBooleanArray checkedList = listView.CheckedItemPositions;
					for (int i = 0; i < checkedList.Size(); i++) {
						if (checkedList.ValueAt(i)) {
							var pos = checkedList.KeyAt(i);
							listView.SetItemChecked(pos, false);
							wcdListAdapter[pos].ItemChecked = false;
						}
					}
					if (wcdListAdapter.Count == 0)
						fabWcd.SetImageResource(Resource.Drawable.ic_refresh_white);
					else if (listView.CheckedItemCount == 0)
						fabWcd.SetImageResource(Resource.Drawable.ic_subject_white);
					else
						fabWcd.SetImageResource(Resource.Drawable.ic_edit_white);
					wcdListAdapter.NotifyDataSetChanged();
				} catch { }
			};

			var refresher = view.FindViewById<SwipeRefreshLayout>(Resource.Id.srl);
			if (refresher != null) {
				refresher.Refresh += delegate
				{
					Refresh(forced: true);
					refresher.Refreshing = false;
				};
			}

			// 떠 있는 액션버튼
			fabWcd = view.FindViewById<FloatingActionButton>(Resource.Id.fab_wcd);
			fabWcd.Click += FabWcd_Click;

			try {
				if (wcdListAdapter.Count == 0)
					fabWcd.SetImageResource(Resource.Drawable.ic_refresh_white);
				else if (listView.CheckedItemCount == 0)
					fabWcd.SetImageResource(Resource.Drawable.ic_subject_white);
				else
					fabWcd.SetImageResource(Resource.Drawable.ic_edit_white);
			} catch { }

			return view;
		}

		private void FabWcd_TextView(object sender, EventArgs e)
		{
			//View dialogView = LayoutInflater.From(Context).Inflate(Resource.Layout.weld_count_text_view, null);
			//var textView = dialogView.FindViewById<TextView>(Resource.Id.textView);
			//AlertDialog.Builder dialog = new AlertDialog.Builder(Context);
			//dialog.SetView(dialogView);

			var textView = new TextView(Context);
			textView.SetPadding(10, 10, 10, 10);
			textView.SetTextSize(ComplexUnitType.Sp, 10f);
			var scrollView = new ScrollView(Context);
			scrollView.AddView(textView);
			AlertDialog.Builder dialog = new AlertDialog.Builder(Context);
			dialog.SetView(scrollView);

			try {
				using (StreamReader sr = new StreamReader(robotPath, Encoding.GetEncoding("euc-kr"))) {
					textView.Text = sr.ReadToEnd();
					sr.Close();
				}
			} catch {
				LogDebug("파일이 없습니다: " + robotPath);
			}

			dialog.SetPositiveButton("닫기", delegate
			{ });

			dialog.Show();
		}

		private void FabWcd_Click(object sender, EventArgs e)
		{
			if (wcdListAdapter.Count == 0) {
				Refresh();
				if (wcdListAdapter.Count == 0)
					ToastShow("항목이 없습니다");
				return;
			} else if (listView.CheckedItemCount == 0) {
				FabWcd_TextView(sender, e);
				return;
			}

			List<int> positions = new List<int>();
			try {
				SparseBooleanArray checkedList = listView.CheckedItemPositions;
				for (int i = 0; i < checkedList.Size(); i++) {
					if (checkedList.ValueAt(i)) {
						positions.Add(checkedList.KeyAt(i));
					}
				}
				if (positions.Count == 0)
					lastPosition = 0;
			} catch {
				return;
			}

			View dialogView = LayoutInflater.From(Context).Inflate(Resource.Layout.wcd_editor, null);
			AlertDialog.Builder dialog = new AlertDialog.Builder(Context);
			dialog.SetView(dialogView);

			// 에디트텍스트
			IList<EditText> etList = new List<EditText>();
			etList.Add(dialogView.FindViewById<EditText>(Resource.Id.etOutputData));
			etList.Add(dialogView.FindViewById<EditText>(Resource.Id.etOutputType));
			etList.Add(dialogView.FindViewById<EditText>(Resource.Id.etSqueezeForce));
			etList.Add(dialogView.FindViewById<EditText>(Resource.Id.etMoveTipClearance));
			etList.Add(dialogView.FindViewById<EditText>(Resource.Id.etFixedTipClearance));
			etList.Add(dialogView.FindViewById<EditText>(Resource.Id.etPannelThickness));
			etList.Add(dialogView.FindViewById<EditText>(Resource.Id.etCommandOffset));

			int[] etMax = { 2000, 100, 350, 500, 500, 500, 500, 1000, 1000 };   // 임계치

			InputMethodManager imm = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);

			for (int i = 0; i < etList.Count; i++) {
				EditText et = etList[i];

				if (i == 0)                                                 // outputData
					et.Text = wcdListAdapter[lastPosition][i];             // 기본선택된 자료값 가져오기

				int maxValue = etMax[i];                                    // 임계치 설정
				et.FocusChange += (object sender1, View.FocusChangeEventArgs e1) =>
				{
					int n;
					if (Int32.TryParse(et.Text, out n)) {
						if (n > maxValue) {
							n = maxValue;
							et.Text = n.ToString();
						}
					}
				};
				et.KeyPress += (object sender1, View.KeyEventArgs e1) =>
				{
					// KeyEventArgs.Handled
					// 라우트된 이벤트를 처리된 것으로 표시하려면 true이고,
					// 라우트된 이벤트를 처리되지 않은 것으로 두어 이벤트가 추가로 라우트되도록 허용하려면 false입니다.
					// 기본값은 false입니다. 
					e1.Handled = false;
					if (e1.KeyCode == Keycode.Enter || e1.KeyCode == Keycode.Back || e1.KeyCode == Keycode.Escape) {
						imm.HideSoftInputFromWindow(et.WindowToken, 0);
						et.ClearFocus();
						e1.Handled = true;
					}
				};
			}

			var statusText = dialogView.FindViewById<TextView>(Resource.Id.statusText);
			statusText.Text = wcdListAdapter[lastPosition][0];
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

			var sampleSeekBar = dialogView.FindViewById<SeekBar>(Resource.Id.sampleSeekBar);
			sampleSeekBar.Max = wcdListAdapter.Count - 1;
			sampleSeekBar.Progress = Convert.ToInt32(wcdListAdapter[lastPosition][0]) - 1;
			sampleSeekBar.ProgressChanged += (object sender1, SeekBar.ProgressChangedEventArgs e1) =>
			{
				for (int i = 0; i < wcdListAdapter[sampleSeekBar.Progress].Count; i++) {
					if (etList[i].Text != "")
						etList[i].Text = wcdListAdapter[sampleSeekBar.Progress][i];
				}
				if (positions.Count == 0) {
					lastPosition = sampleSeekBar.Progress;
					statusText.Text = "수정 항목: " + (lastPosition + 1).ToString();
				}
			};

			// 선택 시작
			var beginSeekBar = dialogView.FindViewById<SeekBar>(Resource.Id.sbBegin);
			beginSeekBar.Max = wcdListAdapter.Count - 1;
			beginSeekBar.Progress = 0;

			// 선택 끝
			var endSeekBar = dialogView.FindViewById<SeekBar>(Resource.Id.sbEnd);
			endSeekBar.Max = wcdListAdapter.Count - 1;
			endSeekBar.Progress = endSeekBar.Max;

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
				try {
					SparseBooleanArray checkedList = listView.CheckedItemPositions;
					for (int i = 0; i < checkedList.Size(); i++) {
						if (checkedList.ValueAt(i)) {
							var pos = checkedList.KeyAt(i);
							listView.SetItemChecked(pos, true);
							wcdListAdapter[pos].ItemChecked = true;
						}
					}
					if (wcdListAdapter.Count == 0)
						fabWcd.SetImageResource(Resource.Drawable.ic_refresh_white);
					else if (listView.CheckedItemCount == 0)
						fabWcd.SetImageResource(Resource.Drawable.ic_subject_white);
					else
						fabWcd.SetImageResource(Resource.Drawable.ic_edit_white);
					wcdListAdapter.NotifyDataSetChanged();
				} catch { }
			});

			dialog.SetPositiveButton("저장", async delegate
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
					for (int colNum = 1; colNum < wcdListAdapter[rowNum].Count; colNum++) {
						if (etList[colNum].Text != "") {
							wcdListAdapter[rowNum][colNum] = etList[colNum].Text;
							isUpdate = true;
						}
					}
					listView.SetItemChecked(rowNum, false);
					wcdListAdapter[rowNum].ItemChecked = false;
				}
				if (isUpdate) {
					wcdListAdapter.NotifyDataSetChanged();
					await UpdateFileAsync(robotPath, wcdListAdapter);
				}
				try {
					if (wcdListAdapter.Count == 0)
						fabWcd.SetImageResource(Resource.Drawable.ic_refresh_white);
					else if (listView.CheckedItemCount == 0)
						fabWcd.SetImageResource(Resource.Drawable.ic_subject_white);
					else
						fabWcd.SetImageResource(Resource.Drawable.ic_edit_white);
				} catch { }
			});

			dialog.Show();
		}
	}
}