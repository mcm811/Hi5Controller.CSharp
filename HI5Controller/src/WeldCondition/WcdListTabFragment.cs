using System;
using System.Collections.Generic;
using Android.App;
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
using Android.Text;
using Android.Views.InputMethods;
using System.Text;

namespace Com.Changyoung.HI5Controller
{
	public class WcdListTabFragment : Fragment, IRefresh
	{
		private View view;
		private FloatingActionButton fabWcd;

		private string dirPath;
		private string robotPath;

		private ListView mListView;
		private WcdListAdapter mWcdListAdapter;

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
			Toast.MakeText(Context, str, ToastLength.Short).Show();
			LogDebug(str);
		}

		private void SnackbarShow(View viewParent, string str)
		{
			Snackbar.Make(viewParent, str, Snackbar.LengthLong)
					.SetAction("Undo", (view) => { /*Undo message sending here.*/ })
					.Show(); // Don’t forget to show!
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

		public void Refresh(bool forced = false)
		{
			if (mWcdListAdapter == null) {
				fabWcd.SetImageResource(Resource.Drawable.ic_refresh_white);
				LogDebug("mWcdListAdapter == null");
				return;
			}

			if (forced || dirPath != Pref.Path || mWcdListAdapter.Count == 0) {
				LogDebug("Refresh: " + dirPath + " : " + Pref.Path + " : " + mWcdListAdapter.Count.ToString());
				dirPath = Pref.Path;
				robotPath = System.IO.Path.Combine(dirPath, "ROBOT.SWD");
				mWcdListAdapter.Clear();
				ReadFile(robotPath, mWcdListAdapter);
				mWcdListAdapter.NotifyDataSetChanged();
			}
			fabWcd.SetImageResource(mWcdListAdapter.Count == 0 ? Resource.Drawable.ic_refresh_white : Resource.Drawable.ic_edit_white);
		}

		public override void OnCreate(Bundle bundle)
		{
			LogDebug("OnCreate");
			base.OnCreate(bundle);

			dirPath = Pref.Path;
			robotPath = System.IO.Path.Combine(dirPath, "ROBOT.SWD");
			mWcdListAdapter = new WcdListAdapter(Context);
			ReadFile(robotPath, mWcdListAdapter);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			LogDebug("OnCreateView");
			view = inflater.Inflate(Resource.Layout.wcd_list_tab_fragment, container, false);

			selectedBackGroundColor = Context.Resources.GetColor(Resource.Color.tab2_textview_background);

			mListView = view.FindViewById<ListView>(Resource.Id.wcdListView);
			mListView.Adapter = mWcdListAdapter;
			mListView.FastScrollEnabled = false;
			mListView.ChoiceMode = ChoiceMode.Multiple;
			mListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
			{
				mWcdListAdapter[e.Position].ItemChecked = mListView.IsItemChecked(e.Position);
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
			fabWcd.SetImageResource(mWcdListAdapter.Count == 0 ? Resource.Drawable.ic_refresh_white : Resource.Drawable.ic_edit_white);
			fabWcd.Click += FabWcd_Click;

			return view;
		}

		private void FabWcd_Click(object sender, EventArgs e)
		{
			if (mWcdListAdapter.Count == 0) {
				Refresh();
				if (mWcdListAdapter.Count == 0)
					ToastShow("항목이 없습니다");
				return;
			}

			List<int> positions = new List<int>();
			try {
				SparseBooleanArray checkedList = mListView.CheckedItemPositions;
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

			//IList<TextInputLayout> tilList = new List<TextInputLayout>();
			//tilList.Add(dialogView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout1));
			//tilList.Add(dialogView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout2));
			//tilList.Add(dialogView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout3));
			//tilList.Add(dialogView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout4));
			//tilList.Add(dialogView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout5));
			//tilList.Add(dialogView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout6));
			//tilList.Add(dialogView.FindViewById<TextInputLayout>(Resource.Id.textInputLayout7));

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
				//et.SetTextSize(ComplexUnitType.Sp, 12);
				//et.ScaleX = 0.8f;
				//et.ScaleY = 0.8f;
				//tilList[i].ScaleX = 0.8f;
				//tilList[i].ScaleY = 0.8f;

				if (i == 0)                                                 // outputData
					et.Text = mWcdListAdapter[lastPosition][i];             // 기본선택된 자료값 가져오기

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
					if (e1.KeyCode == Keycode.Enter) {
						imm.HideSoftInputFromWindow(et.WindowToken, 0);
						et.ClearFocus();
						e1.Handled = true;
					}
				};
			}

			var statusText = dialogView.FindViewById<TextView>(Resource.Id.statusText);
			statusText.Text = mWcdListAdapter[lastPosition][0];
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
			sampleSeekBar.Max = mWcdListAdapter.Count - 1;
			sampleSeekBar.Progress = Convert.ToInt32(mWcdListAdapter[lastPosition][0]) - 1;
			sampleSeekBar.ProgressChanged += (object sender1, SeekBar.ProgressChangedEventArgs e1) =>
			{
				for (int i = 0; i < mWcdListAdapter[sampleSeekBar.Progress].Count; i++) {
					if (etList[i].Text != "")
						etList[i].Text = mWcdListAdapter[sampleSeekBar.Progress][i];
				}
				if (positions.Count == 0) {
					lastPosition = sampleSeekBar.Progress;
					statusText.Text = "수정 항목: " + (lastPosition + 1).ToString();
				}
			};

			// 선택 시작
			var beginSeekBar = dialogView.FindViewById<SeekBar>(Resource.Id.sbBegin);
			beginSeekBar.Max = mWcdListAdapter.Count - 1;
			beginSeekBar.Progress = 0;

			// 선택 끝
			var endSeekBar = dialogView.FindViewById<SeekBar>(Resource.Id.sbEnd);
			endSeekBar.Max = mWcdListAdapter.Count - 1;
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
				//if (positions.Count > 0) {
				//	foreach (int pos in positions) {
				//		mListView.SetItemChecked(pos, false);
				//		mWcdListAdapter[pos].ItemChecked = false;
				//	}
				//	mWcdListAdapter.NotifyDataSetChanged();
				//}
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
					for (int colNum = 1; colNum < mWcdListAdapter[rowNum].Count; colNum++) {
						if (etList[colNum].Text != "") {
							mWcdListAdapter[rowNum][colNum] = etList[colNum].Text;
							isUpdate = true;
						}
					}
					mListView.SetItemChecked(rowNum, false);
					mWcdListAdapter[rowNum].ItemChecked = false;
				}
				if (isUpdate) {
					mWcdListAdapter.NotifyDataSetChanged();
					await UpdateFileAsync(robotPath, mWcdListAdapter);
				}
			});

			dialog.Show();
		}
	}
}