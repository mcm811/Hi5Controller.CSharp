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
	[Activity(Label = "용접 조건 데이터 (Weld condition data)", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/MyCustomTheme")]
	public class SwdActivity : Activity
	{
		static readonly List<WeldConditionData> weldConditonDataList = new List<WeldConditionData>();

		string[] stringTitle = new string[] {
			//"Output data,Output type,Squeeze force,Move tip clearance,Fixed tip clearence,Pannel thickness,Command offset",
			//"출력 데이터,출력 타입,가압력,이동극 제거율,고정극 제거율,패널 두께,명령 옵셋",
			"출력 타입,가압력,이동극 제거율,고정극 제거율,패널 두께,명령 옵셋",
		};

		private const int columnWidth = 100;

		private bool IsVertical() {
			var metrics = Resources.DisplayMetrics;
			//bool vt = (WindowManager.DefaultDisplay.Rotation == SurfaceOrientation.Rotation0 || WindowManager.DefaultDisplay.Rotation == SurfaceOrientation.Rotation180);
			//Console.WriteLine("넓이:{0}, 높이:{1}", metrics.WidthPixels, metrics.HeightPixels);
			return metrics.WidthPixels < metrics.HeightPixels;
		}

		private TableLayout CreateTitleTable(bool StretchAllColumns = true, bool ShrinkAllColumns = true) {
			TableLayout titleTable = new TableLayout(this);
			titleTable.StretchAllColumns = StretchAllColumns;
			titleTable.ShrinkAllColumns = ShrinkAllColumns;

			bool isVertical = IsVertical();
			int n = 0;
			foreach (string item in stringTitle) {
				TableRow rowItem = new TableRow(this);

				CheckBox checkBox = new CheckBox(this);
				checkBox.Text = n++ == 0 ? "No" : "Index";
				checkBox.PaintFlags = PaintFlags.UnderlineText;
				checkBox.SetTextSize(Android.Util.ComplexUnitType.Pt, 6);
				checkBox.SetWidth(columnWidth);

				rowItem.AddView(checkBox);

				int nv = 0;
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
						if (isVertical && nv++ < 1) valueText.Visibility = ViewStates.Gone;
						rowItem.AddView(valueText);
					}
				}
				titleTable.AddView(rowItem);
			}

			return titleTable;
		}

		private TableLayout mainTable;

		private TableLayout CreateMainTable(bool StretchAllColumns = true, bool ShrinkAllColumns = true) {
			mainTable = new TableLayout(this);
			mainTable.StretchAllColumns = StretchAllColumns;
			mainTable.ShrinkAllColumns = ShrinkAllColumns;

			//WeldConditionData wcd = new WeldConditionData();

            bool isVertical = IsVertical();
			var weldConditionData = Intent.Extras.GetStringArrayList("weld_condition_data") ?? new string[0];
			foreach (string item in weldConditionData) {
				string[] its = item.Trim().Split(new char[] { '=' });
				if (its.Length != 2) {
					Log.Error("SWD: its - ", its.Length.ToString() + ": [" + its[0] + "] [" + its[1] + "]");
					continue;
				}

				string[] data = its[1].Trim().Split(new char[] { ',', '-' });
				if (data.Length != 7) {
					Log.Error("SWD: data - ", data.Length.ToString() + ": [" + data[0] + "] [" + data[1] + "]");
					continue;
				}

				TableRow rowItem = new TableRow(this);
				int nv = 0;
				foreach (string value in data) {
					if (nv == 0) {
						CheckBox view = new CheckBox(this);
						view.Text = value;
						view.SetWidth(columnWidth);
						view.SetTextSize(Android.Util.ComplexUnitType.Pt, 6);
						rowItem.AddView(view);
					} else {
						TextView view = new TextView(this);
						view.Text = value;
						view.SetWidth(columnWidth);
						view.Gravity = GravityFlags.Center;
						if (isVertical && nv < 2) view.Visibility = ViewStates.Gone;
						rowItem.AddView(view);
					}
					nv++;
				}

				rowItem.Click += (sender, e) => {
					CheckBox cb = (CheckBox)rowItem.GetChildAt(0);
					cb.Checked = !cb.Checked;
				};

				rowItem.LongClick += (sender, e) => {
					var dialog = new AlertDialog.Builder(this);
					LayoutInflater layoutInflater = LayoutInflater.From(this);
					View editFieldView = layoutInflater.Inflate(Resource.Layout.Editor, null);
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
						editText[i - 1].Text = ((TextView)rowItem.GetChildAt(i)).Text;
					}

					for (int i = 0; i < textView.Count; i++) {
						EditText et = editText[i];
						et.Visibility = ViewStates.Invisible;
						textView[i].Click += (sender0, e0) => {
							et.Visibility = et.Visibility == ViewStates.Invisible ? ViewStates.Visible : ViewStates.Invisible;
						};
					}

					var statusText = editFieldView.FindViewById<TextView>(Resource.Id.statusText);
					var sampleSeekBar = editFieldView.FindViewById<SeekBar>(Resource.Id.sampleSeekBar);

					sampleSeekBar.Max = mainTable.ChildCount - 1;
					sampleSeekBar.Progress = Convert.ToInt32(((CheckBox)rowItem.GetChildAt(0)).Text) - 1;
					statusText.Text = sampleSeekBar.Progress.ToString();
					sampleSeekBar.ProgressChanged += (object sender1, SeekBar.ProgressChangedEventArgs e1) => {
						statusText.Text = (e1.Progress + 1).ToString();
						TableRow tri = (TableRow)mainTable.GetChildAt(sampleSeekBar.Progress);
						for (int i = 1; i < tri.ChildCount; i++) {
							editText[i - 1].Text = ((TextView)tri.GetChildAt(i)).Text;
							editText[i - 1].Visibility = ViewStates.Visible;
						}
					};

					var beginSeekBar = editFieldView.FindViewById<SeekBar>(Resource.Id.beginSeekBar);
					var endSeekBar = editFieldView.FindViewById<SeekBar>(Resource.Id.endSeekBar);

					beginSeekBar.Max = mainTable.ChildCount;
					beginSeekBar.Progress = beginSeekBar.Max / 2;

					endSeekBar.Max = mainTable.ChildCount;
					endSeekBar.Progress = endSeekBar.Max / 2;

					editText[7].Text = string.Format("{0}", beginSeekBar.Progress + 1);
					editText[8].Text = string.Format("{0}", endSeekBar.Progress + 1);

					editText[7].TextChanged += (object sender1, TextChangedEventArgs e1) => {
						//statusText.Text = e1.Text.ToString();
						int n = Convert.ToInt32(e1.Text.ToString());
						if (n > beginSeekBar.Max) {
							n = beginSeekBar.Max;
							editText[7].Text = n.ToString();
						}
						beginSeekBar.Progress = n;
					};
					editText[8].TextChanged += (object sender1, TextChangedEventArgs e1) => {
						//statusText.Text = e1.Text.ToString();
						int n = Convert.ToInt32(e1.Text.ToString());
						if (n > endSeekBar.Max) {
							n = endSeekBar.Max;
							editText[8].Text = n.ToString();
						}
						endSeekBar.Progress = endSeekBar.Max - n - 1;
					};

					beginSeekBar.ProgressChanged += (object sender1, SeekBar.ProgressChangedEventArgs e1) => {
						if (e1.FromUser) {
							int sb1Progress = beginSeekBar.Progress;
							int sb2Progress = endSeekBar.Max - endSeekBar.Progress;
							editText[7].Text = string.Format("{0}", sb1Progress + 1);
							if (sb1Progress > sb2Progress) {
								endSeekBar.Progress = endSeekBar.Max - beginSeekBar.Progress;
								editText[8].Text = string.Format("{0}", sb1Progress + 1);
							}
							if (sb1Progress == 1 && sb2Progress == 1 || sb1Progress == beginSeekBar.Max && sb2Progress == endSeekBar.Max) {
								editText[7].Visibility = ViewStates.Invisible;
								editText[8].Visibility = ViewStates.Invisible;
							} else {
								editText[7].Visibility = ViewStates.Visible;
								editText[8].Visibility = ViewStates.Visible;
							}
						}
					};

					endSeekBar.ProgressChanged += (object sender2, SeekBar.ProgressChangedEventArgs e2) => {
						if (e2.FromUser) {
							int sb1Progress = beginSeekBar.Progress;
							int sb2Progress = endSeekBar.Max - endSeekBar.Progress;
							editText[8].Text = string.Format("{0}", sb2Progress + 1);
							if (sb1Progress > sb2Progress) {
								beginSeekBar.Progress = sb2Progress;
								editText[7].Text = string.Format("{0}", sb2Progress + 1);
							}
							if (sb1Progress <= 1 && sb2Progress <= 1 || sb1Progress >= beginSeekBar.Max && sb2Progress >= endSeekBar.Max) {
								editText[7].Visibility = ViewStates.Invisible;
								editText[8].Visibility = ViewStates.Invisible;
							} else {
								editText[7].Visibility = ViewStates.Visible;
								editText[8].Visibility = ViewStates.Visible;
							}
						}
					};

					int checkCount = 0;
					// 체크가 된 항목 갯수
					for (int i = 0; i < mainTable.ChildCount; i++) {
						CheckBox cb = (CheckBox)((TableRow)mainTable.GetChildAt(i)).GetChildAt(0);
						if (cb.Checked) checkCount++;
					}

					dialog.SetNegativeButton("취소", delegate {
						if (checkCount > 0) {
							for (int i = 0; i < mainTable.ChildCount; i++) {
								CheckBox cb = (CheckBox)((TableRow)mainTable.GetChildAt(i)).GetChildAt(0);
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

						/*
						// 다시 메인 액티비티 호출
						var intent = new Intent(this, typeof(MainActivity));
						//intent.PutStringArrayListExtra("weld_condition_data", weldConditionData);
						StartActivity(intent);
						*/
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
			ProgressBar pb = new ProgressBar(this);
			pb.Indeterminate = true;
			pb.Visibility = ViewStates.Visible;
			pb.ScaleX = 1.5F;
			pb.ScaleY = 1.5F;
			pb.SetPadding(0, 300, 0, 0);

			LinearLayout linearLayout = new LinearLayout(this);
			linearLayout.Orientation = Orientation.Vertical;
			SetContentView(linearLayout);

			linearLayout.AddView(CreateTitleTable());
			linearLayout.AddView(pb);
			await AsyncMainTable();
			pb.Visibility = ViewStates.Gone;

			ScrollView scv = new ScrollView(this);
			scv.AddView(mainTable);
			linearLayout.AddView(scv);

			TextView tv = new TextView(this);
			tv.Text = mainTable.ChildCount.ToString();
			tv.Gravity = GravityFlags.Center;
			linearLayout.AddView(tv);

			var toast = Toast.MakeText(this, "길게를 터치하면 편집", ToastLength.Long);
			toast.Show();
		}
	}
}
