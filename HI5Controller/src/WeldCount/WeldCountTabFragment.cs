using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.IO;
using Fragment = Android.Support.V4.App.Fragment;
using EditText = Android.Support.V7.Widget.AppCompatEditText;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using System.Collections.Generic;
using System;
using Android.Views.InputMethods;

namespace Com.Changyoung.HI5Controller
{
	public class WeldCountTabFragment : Fragment, IRefresh
	{
		private View view;
		private ListView listView;
		private WeldCountAdapter weldCountAdapter;

		private string dirPath;

		private void LogDebug(string msg)
		{
			Log.Debug(Context.PackageName, "WeldCountTabFragment: " + msg);
		}

		private void ToastShow(string str)
		{
			//Toast.MakeText(Context, str, ToastLength.Short).Show();
			Snackbar.Make(view, str, Snackbar.LengthLong).Show();
			LogDebug(str);
		}

		private void SnackbarShow(View viewParent, string str)
		{
			Snackbar.Make(viewParent, str, Snackbar.LengthLong)
					.SetAction("Undo", (view) => { /*Undo message sending here.*/ })
					.Show(); // Don’t forget to show!
			LogDebug(str);
		}

		public void Refresh(bool forced = false)
		{
			if (forced || dirPath != Pref.Path || weldCountAdapter.Count == 0) {
				//LogDebug("Refresh: " + dirPath + " : " + Pref.Path + " : " + weldCountAdapter.Count.ToString());

				dirPath = Pref.Path;
				if (weldCountAdapter == null)
					weldCountAdapter = new WeldCountAdapter(Context, Resource.Layout.weld_count_row);
				else
					weldCountAdapter.Clear();

				try {
					var dir = new DirectoryInfo(dirPath);
					foreach (var item in dir.GetFileSystemInfos()) {
						if (item.FullName.EndsWith(".JOB"))
							weldCountAdapter.Add(new JobFile(item.FullName));
					}
					weldCountAdapter.NotifyDataSetChanged();
				} catch {
				}
			}
		}

		public override void OnCreate(Bundle bundle)
		{
			LogDebug("OnCreate");
			base.OnCreate(bundle);

			dirPath = Pref.Path;
			Refresh(forced: true);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate(Resource.Layout.weld_count_tab_fragment, container, false);

			var refresher = view.FindViewById<SwipeRefreshLayout>(Resource.Id.srl);
			if (refresher != null) {
				refresher.Refresh += delegate
				{
					Refresh(forced: true);
					refresher.Refreshing = false;
				};
			}

			listView = view.FindViewById<ListView>(Resource.Id.weldCountListView);
			listView.Adapter = weldCountAdapter;
			listView.ItemClick += ListView_Click;
			listView.ItemLongClick += (object sender, AdapterView.ItemLongClickEventArgs e) =>
			{
				if (weldCountAdapter.Count == 0) {
					Refresh();
					if (weldCountAdapter.Count == 0)
						ToastShow("항목이 없습니다");
					return;
				}

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

				var jobFile = weldCountAdapter.GetItem(e.Position);
				textView.Text = jobFile.RowText();

				dialog.SetPositiveButton("닫기", delegate
				{ });

				dialog.Show();
			};

			return view;
		}

		private void ListView_Click(object sender, AdapterView.ItemClickEventArgs e)
		{
			if (weldCountAdapter.Count == 0) {
				Refresh();
				if (weldCountAdapter.Count == 0)
					ToastShow("항목이 없습니다");
				return;
			}

			InputMethodManager imm = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);
			View dialogView = LayoutInflater.From(Context).Inflate(Resource.Layout.weld_count_editor, null);
			AlertDialog.Builder dialog = new AlertDialog.Builder(Context);
			dialog.SetView(dialogView);

			var jobFile = weldCountAdapter.GetItem(e.Position);
			var statusText = dialogView.FindViewById<TextView>(Resource.Id.statusText);
			statusText.Text = "계열 수정 (CN: " + jobFile.JobCount.Total.ToString() + "개)";

			var linearLayout = dialogView.FindViewById<LinearLayout>(Resource.Id.linearLayout);
			var etBeginNumber = dialogView.FindViewById<EditText>(Resource.Id.etBeginNumber);
			var sbBeginNumber = dialogView.FindViewById<SeekBar>(Resource.Id.sampleSeekBar);

			var etList = new List<EditText>();
			for (int i = 0; i < jobFile.Count; i++) {
				if (jobFile[i].RowType == Job.RowTypes.Spot) {
					var textInputLayout = new TextInputLayout(Context);
					var etCN = new EditText(Context);
					etCN.InputType = etBeginNumber.InputType;
					etCN.SetSelectAllOnFocus(true);
					etCN.Hint = "CN[" + jobFile[i].RowNumber + "]";
					etCN.Text = jobFile[i].CN;
					etCN.Gravity = GravityFlags.Center;
					etCN.Tag = jobFile[i];
					etCN.FocusChange += (object sender1, View.FocusChangeEventArgs e1) =>
					{
						int beginNumber;
						if (Int32.TryParse(etCN.Text, out beginNumber)) {
							if (beginNumber > 255) {
								beginNumber = 255;
								etCN.Text = beginNumber.ToString();
							}
						}
					};
					etCN.KeyPress += (object sender1, View.KeyEventArgs e1) =>
					{
						e1.Handled = false;
						if (e1.KeyCode == Keycode.Enter || e1.KeyCode == Keycode.Back || e1.KeyCode == Keycode.Escape) {
							imm.HideSoftInputFromWindow(etCN.WindowToken, 0);
							etCN.ClearFocus();
							e1.Handled = true;
						}
					};
					textInputLayout.AddView(etCN);
					linearLayout.AddView(textInputLayout);
					etList.Add(etCN);
				}
			}

			etBeginNumber.FocusChange += (object sender1, View.FocusChangeEventArgs e1) =>
			{
				int beginNumber;
				if (Int32.TryParse(etBeginNumber.Text, out beginNumber)) {
					if (beginNumber > 255) {
						beginNumber = 255;
						etBeginNumber.Text = beginNumber.ToString();
					}
					sbBeginNumber.Progress = beginNumber - 1;

					foreach (var et in etList) {
						et.Text = beginNumber++.ToString();
						if (beginNumber > 255)
							beginNumber = 255;
					}
				}
			};
			etBeginNumber.KeyPress += (object sender1, View.KeyEventArgs e1) =>
			{
				e1.Handled = false;
				if (e1.KeyCode == Keycode.Enter || e1.KeyCode == Keycode.Back || e1.KeyCode == Keycode.Escape) {
					imm.HideSoftInputFromWindow(etBeginNumber.WindowToken, 0);
					etBeginNumber.ClearFocus();
					e1.Handled = true;
				}
			};

			sbBeginNumber.Max = 254;
			sbBeginNumber.Progress = 0;
			sbBeginNumber.ProgressChanged += (object sender1, SeekBar.ProgressChangedEventArgs e1) =>
			{
				int beginNumber = sbBeginNumber.Progress + 1;
				etBeginNumber.Text = beginNumber.ToString();
			};
			sbBeginNumber.StopTrackingTouch += (object sender1, SeekBar.StopTrackingTouchEventArgs e1) =>
			{
				int beginNumber = sbBeginNumber.Progress + 1;
				etBeginNumber.Text = beginNumber.ToString();
				foreach (var et in etList) {
					et.Text = beginNumber++.ToString();
					if (beginNumber > 255)
						beginNumber = 255;
				}
			};

			dialog.SetNegativeButton("취소", delegate
			{ });

			dialog.SetPositiveButton("저장", delegate
			{
				foreach (var et in etList) {
					var job = (Job)et.Tag;
					job.CN = et.Text;
				}
				if (jobFile.JobCount.Total > 0) {
					jobFile.SaveFile();
					weldCountAdapter.NotifyDataSetChanged();
					ToastShow("저장 완료: " + jobFile.JobCount.fi.FullName);
				}
			});

			dialog.Show();
		}
	}
}
