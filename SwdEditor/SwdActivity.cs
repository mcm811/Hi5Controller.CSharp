using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using System.Threading.Tasks;

namespace SwdEditor
{
	[Activity(Label = "용접 조건 데이터 (Weld condition data)", MainLauncher = false, Icon = "@drawable/icon")]
	public class SwdActivity : Activity
	{
		string[] stringTitle = new string[] {
			"출력 데이터,출력 타입,가압력,이동극 제거율,고정극 제거율,패널 두께,명령 옵셋",
			//"Output data,Output type,Squeeze force,Move tip clearance,Fixed tip clearence,Pannel thickness,Command offset",
		};

		private const int columnWidth = 100;

		private bool IsVertical() {
			return false;
			//return (WindowManager.DefaultDisplay.Rotation == SurfaceOrientation.Rotation0 || WindowManager.DefaultDisplay.Rotation == SurfaceOrientation.Rotation180);
		}

		private TableLayout titleTable;

		private TableLayout CreateTitleTable(bool StretchAllColumns = true, bool ShrinkAllColumns = true) {
			titleTable = new TableLayout(this);
			titleTable.StretchAllColumns = StretchAllColumns;
			titleTable.ShrinkAllColumns = ShrinkAllColumns;

			//String udata = "Underlined Text";
			//SpannableString content = new SpannableString(udata);
			//content.SetSpan(new UnderlineSpan(), 0, udata.Length, 0);
			//mTextView.setText(content);

			int n = 0;
			foreach (string item in stringTitle) {
				TableRow rowItem = new TableRow(this);

				CheckBox checkBox = new CheckBox(this);
				checkBox.Text = n == 0 ? "No" : "Index";
				checkBox.PaintFlags = PaintFlags.UnderlineText;
                checkBox.SetTextSize(Android.Util.ComplexUnitType.Pt, 6);
				checkBox.SetWidth(columnWidth);
				//checkBox.SetTextColor(Color.Rgb(255, 255, 255));
				//checkBox.SetBackgroundColor(Color.Rgb(200, 200, 200));

				rowItem.AddView(checkBox);

				int nValue = 0;
				string[] data = item.Trim().Split(new char[] { ',', '-' });
				foreach (string value in data) {
					if (value.Length != 0) {
						TextView valueText = new TextView(this);
						valueText.SetText(value.Trim(), TextView.BufferType.Editable);
						valueText.Gravity = GravityFlags.Center;
						valueText.SetSingleLine(true);
						valueText.SetWidth(columnWidth);
						valueText.PaintFlags = PaintFlags.UnderlineText;
						valueText.SetTextSize(Android.Util.ComplexUnitType.Pt, 6);
						//valueText.SetTextColor(Color.Rgb(255, 255, 255));
						//valueText.SetBackgroundColor(Color.Rgb(200, 200, 200));
						if (IsVertical() && nValue < 2) valueText.Visibility = ViewStates.Gone;
						rowItem.AddView(valueText);
						nValue++;
					}
				}
				titleTable.AddView(rowItem);
				n++;
			}

			return titleTable;
		}

		private TableLayout mainTable;

		private TableLayout CreateMainTable(bool StretchAllColumns = true, bool ShrinkAllColumns = true) {
			mainTable = new TableLayout(this);
			mainTable.StretchAllColumns = StretchAllColumns;
			mainTable.ShrinkAllColumns = ShrinkAllColumns;

			int mainTableIndex = 1;
			var weldConditionData = Intent.Extras.GetStringArrayList("weld_condition_data") ?? new string[0];
			foreach (string item in weldConditionData) {
				TableRow rowItem = new TableRow(this);

				CheckBox checkBox = new CheckBox(this);
				checkBox.Text = mainTableIndex++.ToString();
				checkBox.SetTextSize(Android.Util.ComplexUnitType.Pt, 6);
				checkBox.SetWidth(columnWidth);

				rowItem.AddView(checkBox);

				int nValue = 0;
				string[] data = item.Trim().Split(new char[] { ',', '-' });
				foreach (string value in data) {
					if (value.Length != 0) {
						TextView valueText = new TextView(this);
						valueText.SetText(value.Trim(), TextView.BufferType.Editable);
						valueText.Gravity = GravityFlags.Center;
						valueText.SetSingleLine(true);
						valueText.SetWidth(columnWidth);
						if (IsVertical() && nValue < 2) valueText.Visibility = ViewStates.Gone;
						rowItem.AddView(valueText);
						nValue++;
					}
				}

				rowItem.Click += (sender, e) => {
					CheckBox cb = (CheckBox)rowItem.GetChildAt(0);
					cb.Checked = !cb.Checked;
				};

				rowItem.LongClick += (sender, e) => {
					var dialog = new AlertDialog.Builder(this);
					LayoutInflater layoutInflater = LayoutInflater.From(this);
					View editFieldView = layoutInflater.Inflate(Resource.Layout.EditField, null);
					dialog.SetView(editFieldView);

					List<TextView> textView = new List<TextView>();
					textView.Add(editFieldView.FindViewById<TextView>(Resource.Id.textView1));
					textView.Add(editFieldView.FindViewById<TextView>(Resource.Id.textView2));
					textView.Add(editFieldView.FindViewById<TextView>(Resource.Id.textView3));
					textView.Add(editFieldView.FindViewById<TextView>(Resource.Id.textView4));
					textView.Add(editFieldView.FindViewById<TextView>(Resource.Id.textView5));
					textView.Add(editFieldView.FindViewById<TextView>(Resource.Id.textView6));
					textView.Add(editFieldView.FindViewById<TextView>(Resource.Id.textView7));

					List<EditText> editText = new List<EditText>();
					editText.Add(editFieldView.FindViewById<EditText>(Resource.Id.editText1));
					editText.Add(editFieldView.FindViewById<EditText>(Resource.Id.editText2));
					editText.Add(editFieldView.FindViewById<EditText>(Resource.Id.editText3));
					editText.Add(editFieldView.FindViewById<EditText>(Resource.Id.editText4));
					editText.Add(editFieldView.FindViewById<EditText>(Resource.Id.editText5));
					editText.Add(editFieldView.FindViewById<EditText>(Resource.Id.editText6));
					editText.Add(editFieldView.FindViewById<EditText>(Resource.Id.editText7));
					editText.Add(editFieldView.FindViewById<EditText>(Resource.Id.editText8));
					editText.Add(editFieldView.FindViewById<EditText>(Resource.Id.editText9));

					for (int i = 1; i < rowItem.ChildCount; i++) {
						TextView tv = (TextView)rowItem.GetChildAt(i);
						editText[i - 1].Text = tv.Text;
					}

					for (int i = 0; i < textView.Count; i++) {
						EditText et = editText[i];
						et.Visibility = ViewStates.Invisible;
						textView[i].Click += (sender0, e0) => {
							et.Visibility = et.Visibility == ViewStates.Invisible ? ViewStates.Visible : ViewStates.Invisible;
						};
					}

					var _seekBar1 = editFieldView.FindViewById<SeekBar>(Resource.Id.seekBar1);
					_seekBar1.Max = mainTable.ChildCount - 1;
					_seekBar1.Progress = 0;
					editText[7].Text = string.Format("{0}", _seekBar1.Progress + 1);

					var _seekBar2 = editFieldView.FindViewById<SeekBar>(Resource.Id.seekBar2);
					_seekBar2.Max = mainTable.ChildCount - 1;
					_seekBar2.Progress = _seekBar2.Max;

					_seekBar1.ProgressChanged += (object sender1, SeekBar.ProgressChangedEventArgs e1) => {
						if (e1.FromUser) {
							editText[7].Text = string.Format("{0}", e1.Progress + 1);
							editText[7].Visibility = _seekBar1.Progress == _seekBar1.Max ? ViewStates.Invisible : ViewStates.Visible;
							editText[8].Visibility = _seekBar2.Progress == 0 ? ViewStates.Invisible : ViewStates.Visible;
							editText[7].Visibility = _seekBar1.Progress > _seekBar2.Progress ? ViewStates.Invisible : ViewStates.Visible;
							editText[8].Visibility = _seekBar2.Progress < _seekBar1.Progress ? ViewStates.Invisible : ViewStates.Visible;
						}
					};

					editText[8].Text = string.Format("{0}", _seekBar2.Progress + 1);
					_seekBar2.ProgressChanged += (object sender2, SeekBar.ProgressChangedEventArgs e2) => {
						if (e2.FromUser) {
							editText[8].Text = string.Format("{0}", e2.Progress + 1);
							editText[7].Visibility = _seekBar1.Progress == _seekBar1.Max ? ViewStates.Invisible : ViewStates.Visible;
							editText[8].Visibility = _seekBar2.Progress == 0 ? ViewStates.Invisible : ViewStates.Visible;
							editText[7].Visibility = _seekBar1.Progress > _seekBar2.Progress ? ViewStates.Invisible : ViewStates.Visible;
							editText[8].Visibility = _seekBar2.Progress < _seekBar1.Progress ? ViewStates.Invisible : ViewStates.Visible;
						}
					};

					int checkCount = 0;
					// 체크가 된 항목 갯수
					for (int i = 0; i < mainTable.ChildCount; i++) {
						TableRow ri = (TableRow)mainTable.GetChildAt(i);
						CheckBox cb = (CheckBox)ri.GetChildAt(0);
						if (cb.Checked) checkCount++;
					}

					dialog.SetNegativeButton("취소", delegate {
						if (checkCount > 0) {
							for (int i = 0; i < mainTable.ChildCount; i++) {
								TableRow ri = (TableRow)mainTable.GetChildAt(i);
								CheckBox cb = (CheckBox)ri.GetChildAt(0);
								cb.Checked = false;
							}
						}
					});

					dialog.SetPositiveButton("확인", delegate {
						var seekBegin = editText[7].Visibility == ViewStates.Visible ? Convert.ToInt32(editText[7].Text) : 0;
						var seekEnd = editText[8].Visibility == ViewStates.Visible ? Convert.ToInt32(editText[8].Text) : 0;

						if (seekBegin > 0 && seekEnd > seekBegin) {
							for (int j = seekBegin - 1; j < seekEnd; j++) {
								TableRow ri = (TableRow)mainTable.GetChildAt(j);
								CheckBox cb = (CheckBox)ri.GetChildAt(0);
								for (int i = 2; i < ri.ChildCount; i++) {
									if (editText[i - 1].Text != "" && editText[i - 1].Visibility == ViewStates.Visible) {
										TextView tv = (TextView)ri.GetChildAt(i);
										if (i > 3)
											tv.Text = string.Format("{0:F1}", Double.Parse(editText[i - 1].Text));
										else
											tv.Text = editText[i - 1].Text;
									}
								}
								cb.Checked = false;
							}
						} else if (checkCount > 1) {
							for (int j = 0; j < mainTable.ChildCount; j++) {
								TableRow ri = (TableRow)mainTable.GetChildAt(j);
								CheckBox cb = (CheckBox)ri.GetChildAt(0);
								if (cb.Checked) {
									for (int i = 2; i < ri.ChildCount; i++) {
										if (editText[i - 1].Text != "" && editText[i - 1].Visibility == ViewStates.Visible) {
											TextView tv = (TextView)ri.GetChildAt(i);
											if (i > 3)
												tv.Text = string.Format("{0:F1}", Double.Parse(editText[i - 1].Text));
											else
												tv.Text = editText[i - 1].Text;
										}
									}
									cb.Checked = false;
								}
							}
						} else {
							for (int i = 2; i < rowItem.ChildCount; i++) {
								if (editText[i - 1].Text != "" && editText[i - 1].Visibility == ViewStates.Visible) {
									TextView tv = (TextView)rowItem.GetChildAt(i);
									if (i > 3)
										tv.Text = string.Format("{0:F1}", Double.Parse(editText[i - 1].Text));
									else
										tv.Text = editText[i - 1].Text;
								}
							}
						}
					});
					dialog.Show();
				};
				mainTable.AddView(rowItem);
			}
			return mainTable;
		}

		Task AsyncMainTable() {
			return Task.Run(() => {
				CreateMainTable();
			});
		}

		protected override async void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);
			//ProgressBar pb = FindViewById<ProgressBar>(Resource.Id.progressBar1);
			ProgressBar pb = new ProgressBar(this);
			pb.Indeterminate = true;
			pb.Visibility = ViewStates.Visible;
			pb.ScaleX = 1.5F;
			pb.ScaleY = 1.5F;
			pb.SetPadding(0, 300, 0, 0);

			LinearLayout linearLayout = new LinearLayout(this);
			linearLayout.Orientation = Orientation.Vertical;
			//linearLayout.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);
			SetContentView(linearLayout);

			linearLayout.AddView(CreateTitleTable());
			linearLayout.AddView(pb);
			await AsyncMainTable();
			pb.Visibility = ViewStates.Gone;

			ScrollView scv = new ScrollView(this);
			scv.AddView(mainTable);
			linearLayout.AddView(scv);
		}
	}
}
