using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.IO;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Fragment = Android.Support.V4.App.Fragment;
using EditText = Android.Support.V7.Widget.AppCompatEditText;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using System.Collections.Generic;
using System;
using Android.Views.InputMethods;

namespace Com.Changyoung.HI5Controller
{
	public class WeldCountFragment : Fragment, IRefresh
	{
		private View view;
		private ListView listView;
		private WeldCountAdapter adapter;

		private string dirPath;

		private void LogDebug(string msg)
		{
			try {
				Log.Debug(Context.PackageName, "WeldCountTabFragment: " + msg);
			} catch { }
		}

		public void Show(string str)
		{
			//Snackbar.Make(viewParent, str, Snackbar.LengthLong)
			//		.SetAction("Undo", (view) => { /*Undo message sending here.*/ })
			//		.SetAction("Redo", (view) => { /*Undo message sending here.*/ })
			//		.Show();
			try {
				Snackbar.Make(view, str, Snackbar.LengthShort).Show();
			} catch { }
			LogDebug(str);
		}

		public void Refresh(bool forced = false)
		{
			if (forced || dirPath != Pref.WorkPath || adapter.Count == 0) {
				dirPath = Pref.WorkPath;
				if (adapter == null)
					adapter = new WeldCountAdapter(Context, Resource.Layout.weld_count_row);
				else
					adapter.Clear();

				try {
					var dir = new DirectoryInfo(dirPath);
					foreach (var item in dir.GetFileSystemInfos()) {
						if (item.Name.ToUpper().EndsWith(".JOB") || item.Name.ToUpper().StartsWith("HX"))
							adapter.Add(new JobFile(item.FullName));
					}
					adapter.NotifyDataSetChanged();
					listView.RefreshDrawableState();
				} catch {
				}
			}
		}

		public bool Refresh(string path)
		{
			return true;
		}

		public string OnBackPressedFragment()
		{
			return null;
		}

		public override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			dirPath = Pref.WorkPath;
			Refresh(forced: true);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate(Resource.Layout.weld_count_fragment, container, false);

			var refresher = view.FindViewById<SwipeRefreshLayout>(Resource.Id.srl);
			if (refresher != null) {
				refresher.Refresh += delegate
				{
					Refresh(forced: true);
					refresher.Refreshing = false;
				};
			}

			listView = view.FindViewById<ListView>(Resource.Id.listView);
			listView.Adapter = adapter;
			listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
			{
				var builder = new AlertDialog.Builder(Context);
				builder.SetItems(Resource.Array.select_dialog_items, (object sender1, DialogClickEventArgs e1) =>
				{
					var items = Resources.GetStringArray(Resource.Array.select_dialog_items);
					var item = items[(int)e1.Which];
					if (e1.Which == 0) {
						ListView_Click(e.Position);
					} else if (e1.Which == 1) {
						Pref.TextViewDialog(Context, null, adapter.GetItem(e.Position).RowText());
					}
				});
				builder.Create().Show();
			};
			listView.ItemLongClick += (object sender, AdapterView.ItemLongClickEventArgs e) =>
			{
				ListView_Click(e.Position);
			};

			return view;
		}

		private void ListView_Click(int position)
		{
			if (adapter.Count == 0) {
				Refresh();
				if (adapter.Count == 0)
					Show("항목이 없습니다");
				return;
			}

			var jobFile = adapter.GetItem(position);
			if (jobFile.JobCount.Total == 0) {
				Show("CN 항목이 없습니다");
				return;
			}

			InputMethodManager imm = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);
			View dialogView = LayoutInflater.From(Context).Inflate(Resource.Layout.weld_count_editor, null);
			AlertDialog.Builder dialog = new AlertDialog.Builder(Context);
			dialog.SetView(dialogView);

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
					etCN.SetSingleLine();
					etCN.SetTextSize(ComplexUnitType.Dip, 12);
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
				if (int.TryParse(etBeginNumber.Text, out beginNumber)) {
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
					adapter.NotifyDataSetChanged();
					listView.RefreshDrawableState();
					this.Show((string)("저장 완료: " + jobFile.JobCount.fi.Name));
				}
			});

			dialog.Show();
		}
	}
}
