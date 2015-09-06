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
using System.IO;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace SwdEditor
{
	[Activity(Label = "용접 조건 데이터 (Weld condition data)", MainLauncher = false, Icon = "@drawable/icon", Theme = "@style/MyCustomTheme")]
	public class SwdActivity : AppCompatActivity
	{
		private TableLayout titleTable;
		private TableLayout mainTable;

		private const int columnWidth = 50;
		private const int textSize = 30;
		private const float alphaOff = 0.2f;

		string[] stringTitle = new string[] {
			//"Output data,Output type,Squeeze force,Move tip clearance,Fixed tip clearance,Pannel thickness,Command offset",
			//"출력 데이터,출력 타입,가압력,이동극 제거율,고정극 제거율,패널 두께,명령 옵셋",
			"출력,종류,가압력,이동극 제거율,고정극 제거율,패널 두께,명령 옵셋",
		};

		private bool IsVertical() {
			var metrics = Resources.DisplayMetrics;
			//bool vt = (WindowManager.DefaultDisplay.Rotation == SurfaceOrientation.Rotation0 || WindowManager.DefaultDisplay.Rotation == SurfaceOrientation.Rotation180);
			//Console.WriteLine("넓이:{0}, 높이:{1}", metrics.WidthPixels, metrics.HeightPixels);
			return metrics.WidthPixels < metrics.HeightPixels;
		}

		private TableLayout CreateTitleTable() {
			bool isVertical = IsVertical();
			int colWidthPx = columnWidth * Convert.ToInt32(Resources.DisplayMetrics.DensityDpi) / 160;
			int textDpi = isVertical ? textSize / 2 : textSize;
			foreach (string item in stringTitle) {
				TableRow rowItem = new TableRow(this);
				rowItem.SetBackgroundColor(Color.DodgerBlue);
				titleTable.AddView(rowItem);
				int colNum = 0;
				foreach (string colString in item.Trim().Split(new char[] { ',', '-' })) {
					if (colString.Length != 0) {
						if (colNum == 0) {
							CheckBox view = new CheckBox(this);
							view.Text = isVertical ? "출력" : colString;
							view.Gravity = GravityFlags.Left | GravityFlags.CenterVertical;
							view.SetWidth(colWidthPx / 2);
							view.SetTextSize(ComplexUnitType.Dip, textDpi / 2);
							view.SetTextColor(Color.White);
							view.SetBackgroundColor(Color.DodgerBlue);
							//view.PaintFlags = PaintFlags.SubpixelText;

							view.Click += (sender, e) => {
								Log.Error("Swd: ============================== IsChecked", view.Checked.ToString());
								for (int i = 0; i < mainTable.ChildCount; i++) {
									((CheckBox)((TableRow)mainTable.GetChildAt(i)).GetChildAt(0)).Checked = view.Checked;
								}
							};
							rowItem.AddView(view);
						} else {
							TextView view = new TextView(this);
							view.Text = colString;
							view.Gravity = (colNum == 6 ? GravityFlags.Right : GravityFlags.Center) | GravityFlags.CenterVertical;
							view.SetWidth(colNum == 6 ? colWidthPx / 2 : colWidthPx);
							view.SetTextSize(ComplexUnitType.Dip, textDpi / 2);
							view.SetTextColor(Color.White);
							view.SetBackgroundColor(Color.DodgerBlue);
							//view.PaintFlags = PaintFlags.SubpixelText;
							if (isVertical && colNum == 1) view.Visibility = ViewStates.Gone;
							rowItem.AddView(view);
							//Log.Error("++++++++++++++++", colNum.ToString() + " " + view.Gravity.ToString());
						}
						colNum++;
					}
				}
			}
			return titleTable;
		}

		private void CreateMainTable(bool StretchAllColumns = true, bool ShrinkAllColumns = true) {
			bool isVertical = IsVertical();
			int colWidthPx = columnWidth * Convert.ToInt32(Resources.DisplayMetrics.DensityDpi) / 160;
			int textDpi = isVertical ? textSize / 2 : textSize;
			//Log.Error("============", colWidthPx.ToString());
			foreach (WeldConditionData wcd in SwdFile.wcdList) {
				TableRow rowItem = new TableRow(this);  // 한줄 생성
				mainTable.AddView(rowItem);
				for (int colNum = 0; colNum < wcd.Count; colNum++) {
					if (colNum == 0) {
						CheckBox view = new CheckBox(this); // 첫번째 칸은 체크 박스 추가
						view.Text = wcd[colNum];
						view.Gravity = GravityFlags.Left | GravityFlags.CenterVertical;
						view.SetWidth(colWidthPx / 2);
						view.SetTextSize(ComplexUnitType.Dip, textDpi);
						rowItem.AddView(view);
					} else {
						TextView view = new TextView(this); // 나머지 칸은 텍스트 뷰 추가
						view.Text = wcd[colNum];
						view.Gravity = (colNum == 6 ? GravityFlags.Right : GravityFlags.Center) | GravityFlags.CenterVertical;
						view.SetWidth(colNum == 6 ? colWidthPx / 2 : colWidthPx);
						view.SetTextSize(ComplexUnitType.Dip, textDpi);
						if (isVertical && colNum == 1) view.Visibility = ViewStates.Gone;
						rowItem.AddView(view);
						//Log.Error("============", colNum.ToString() + " " + view.Gravity.ToString());
					}
				}

				// 클릭시 체크 박스 선택/해제
				rowItem.Click += (sender, e) => {
					CheckBox cb = (CheckBox)rowItem.GetChildAt(0);
					cb.Checked = !cb.Checked;
				};

				// 길게 클릭시 수정 대화창 띄우기
				rowItem.LongClick += (sender, e) => {
					View editFieldView = LayoutInflater.From(this).Inflate(Resource.Layout.Editor, null);
					AlertDialog.Builder dialog = new AlertDialog.Builder(this);
					dialog.SetView(editFieldView);

					//// 텍스트뷰
					//IList<TextView> textView = new List<TextView>();
					//textView.Add(editFieldView.FindViewById<TextView>(Resource.Id.textView1));
					//textView.Add(editFieldView.FindViewById<TextView>(Resource.Id.textView2));
					//textView.Add(editFieldView.FindViewById<TextView>(Resource.Id.textView3));
					//textView.Add(editFieldView.FindViewById<TextView>(Resource.Id.textView4));
					//textView.Add(editFieldView.FindViewById<TextView>(Resource.Id.textView5));
					//textView.Add(editFieldView.FindViewById<TextView>(Resource.Id.textView6));
					//textView.Add(editFieldView.FindViewById<TextView>(Resource.Id.textView7));

					// 에디트텍스트
					IList<EditText> editTextList = new List<EditText>();
					editTextList.Add(editFieldView.FindViewById<EditText>(Resource.Id.editText1));
					editTextList.Add(editFieldView.FindViewById<EditText>(Resource.Id.editText2));
					editTextList.Add(editFieldView.FindViewById<EditText>(Resource.Id.editText3));
					editTextList.Add(editFieldView.FindViewById<EditText>(Resource.Id.editText4));
					editTextList.Add(editFieldView.FindViewById<EditText>(Resource.Id.editText5));
					editTextList.Add(editFieldView.FindViewById<EditText>(Resource.Id.editText6));
					editTextList.Add(editFieldView.FindViewById<EditText>(Resource.Id.editText7));
					editTextList.Add(editFieldView.FindViewById<EditText>(Resource.Id.editText8));
					editTextList.Add(editFieldView.FindViewById<EditText>(Resource.Id.editText9));

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

					// 기본선택된 자료값 가져오기
					for (int i = 0; i < rowItem.ChildCount; i++) {
						EditText et = editTextList[i];
						et.Text = ((TextView)rowItem.GetChildAt(i)).Text;
						et.SetTextColor(Color.RosyBrown);
					}

					// 기본으로 에디트텍스트를 안보이게 하고 텍스트뷰를 선택시 에디트텍스트가 보이게 이벤트 처리
					for (int i = 1; i < editTextList.Count; i++) {
						EditText et = editTextList[i];
						et.Alpha = alphaOff;
						et.Click += (sender1, e1) => {
							et.Alpha = et.Alpha == alphaOff ? 1 : alphaOff;
						};
						textInputLayoutList[i].Click += (sender1, e1) => {
							et.Alpha = et.Alpha == alphaOff ? 1 : alphaOff;
						};
					}

					var statusText = editFieldView.FindViewById<TextView>(Resource.Id.statusText);
					statusText.Text = ((CheckBox)rowItem.GetChildAt(0)).Text;

					var sampleSeekBar = editFieldView.FindViewById<SeekBar>(Resource.Id.sampleSeekBar);
					sampleSeekBar.Max = mainTable.ChildCount - 1;
					sampleSeekBar.Progress = Convert.ToInt32(statusText.Text) - 1;
					sampleSeekBar.ProgressChanged += (object sender1, SeekBar.ProgressChangedEventArgs e1) => {
						statusText.Text = (e1.Progress + 1).ToString();
						TableRow tri = (TableRow)mainTable.GetChildAt(sampleSeekBar.Progress);
						for (int i = 0; i < tri.ChildCount; i++) {
							editTextList[i].Text = ((TextView)tri.GetChildAt(i)).Text;
							editTextList[i].Alpha = 1;
						}
					};

					var beginSeekBar = editFieldView.FindViewById<SeekBar>(Resource.Id.beginSeekBar);
					var endSeekBar = editFieldView.FindViewById<SeekBar>(Resource.Id.endSeekBar);

					beginSeekBar.Max = mainTable.ChildCount;
					beginSeekBar.Progress = beginSeekBar.Max / 2;

					endSeekBar.Max = mainTable.ChildCount;
					endSeekBar.Progress = endSeekBar.Max / 2;

					editTextList[7].Text = string.Format("{0}", beginSeekBar.Progress + 1);
					editTextList[8].Text = string.Format("{0}", endSeekBar.Progress + 1);
					editTextList[7].SetTextColor(Color.RosyBrown);
					editTextList[8].SetTextColor(Color.RosyBrown);

					editTextList[7].TextChanged += (object sender1, TextChangedEventArgs e1) => {
						int n = Convert.ToInt32(e1.Text.ToString());
						if (n > beginSeekBar.Max) {
							n = beginSeekBar.Max;
							editTextList[7].Text = n.ToString();
						}
						beginSeekBar.Progress = n;
					};
					editTextList[8].TextChanged += (object sender1, TextChangedEventArgs e1) => {
						int n = Convert.ToInt32(e1.Text.ToString());
						if (n > endSeekBar.Max) {
							n = endSeekBar.Max;
							editTextList[8].Text = n.ToString();
						}
						endSeekBar.Progress = endSeekBar.Max - n - 1;
					};

					beginSeekBar.ProgressChanged += (object sender1, SeekBar.ProgressChangedEventArgs e1) => {
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

					endSeekBar.ProgressChanged += (object sender2, SeekBar.ProgressChangedEventArgs e2) => {
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
						var seekBegin = editTextList[7].Alpha == 1 ? Convert.ToInt32(editTextList[7].Text) : 0;
						var seekEnd = editTextList[8].Alpha == 1 ? Convert.ToInt32(editTextList[8].Text) : 0;

						if (seekBegin > 0 && seekEnd > seekBegin) {
							for (int rowNum = seekBegin - 1; rowNum < seekEnd; rowNum++) {
								TableRow ri = (TableRow)mainTable.GetChildAt(rowNum);
								CheckBox cb = (CheckBox)ri.GetChildAt(0);
								for (int colNum = 1; colNum < ri.ChildCount; colNum++) {
									if (editTextList[colNum].Text != "" && editTextList[colNum].Alpha == 1) {
										TextView ttv = (TextView)ri.GetChildAt(colNum);
										SwdFile.wcdList[rowNum][colNum] = editTextList[colNum].Text;
										ttv.Text = SwdFile.wcdList[rowNum][colNum];
									}
								}
								cb.Checked = false;
							}
						} else if (checkCount > 0) {
							for (int rowNum = 0; rowNum < mainTable.ChildCount; rowNum++) {
								TableRow ri = (TableRow)mainTable.GetChildAt(rowNum);
								CheckBox cb = (CheckBox)ri.GetChildAt(0);
								if (cb.Checked) {
									for (int colNum = 1; colNum < ri.ChildCount; colNum++) {
										if (editTextList[colNum].Text != "" && editTextList[colNum].Alpha == 1) {
											TextView ttv = (TextView)ri.GetChildAt(colNum);
											SwdFile.wcdList[rowNum][colNum] = editTextList[colNum].Text;
											ttv.Text = SwdFile.wcdList[rowNum][colNum];
										}
									}
									cb.Checked = false;
								}
							}
						} else {
							int rowNum = Convert.ToInt32(((TextView)rowItem.GetChildAt(0)).Text) - 1;
							if (editTextList[0].Alpha == 1) rowNum = Convert.ToInt32(editTextList[0].Text) - 1;
							for (int colNum = 1; colNum < rowItem.ChildCount; colNum++) {
								if (editTextList[colNum].Text != "" && editTextList[colNum].Alpha == 1) {
									TextView ttv = (TextView)rowItem.GetChildAt(colNum);
									SwdFile.wcdList[rowNum][colNum] = editTextList[colNum].Text;
									ttv.Text = SwdFile.wcdList[rowNum][colNum];
								}
							}
						}
					});
					dialog.Show();
				};
			}
		}

		Task AsyncMainTable() {
			return Task.Run(() => {
				if (SwdFile.wcdList.Count == 0)
					SwdFile.WcdIListString = Intent.Extras.GetStringArrayList("weld_condition_data") ?? new string[0];
				CreateMainTable();
			});
		}

		async void GetWCDList() {
			Log.Error("WCD", "GetWCDList In");
			TextView stv = new TextView(this);
			Log.Error("WCD", "Start1");
			StreamReader sr = new StreamReader(Assets.Open("ROBOT.SWD"));
			stv.Text = await sr.ReadToEndAsync();
			sr.Close();
			//Log.Error("WCD", stv.Text);

			StringBuilder sb = new StringBuilder();
			Log.Error("WCD", "Start111");
			bool addText = true;
			Log.Error("WCD", "Start1111");
			foreach (string swdLine in stv.Text.Split('\n')) {
				if (addText == false) {
					foreach (WeldConditionData wcd in SwdFile.wcdList) {
						Log.Error("WCD", "44444444444444444444444444444444444");
						//Log.Error("WCD", wcd.Text());
						//sb.Append(wcd.Text());
					}
				}
				//if (swdLine.StartsWith("#006"))
				//	addText = true;
				//if (addText && swdLine.Trim().Length > 0)
				//	sb.Append(swdLine);
				//if (swdLine.StartsWith("#005"))
				//	addText = false;
			}
			var intent = new Intent(this, typeof(MainActivity));
			StartActivity(intent);
		}

		protected override async void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);

			LinearLayout linearLayout = new LinearLayout(this);
			linearLayout.Orientation = Orientation.Vertical;
			SetContentView(linearLayout);

			mainTable = new TableLayout(this);
			mainTable.StretchAllColumns = true;
			mainTable.ShrinkAllColumns = false;

			titleTable = new TableLayout(this);
			titleTable.StretchAllColumns = true;
			titleTable.ShrinkAllColumns = false;
			linearLayout.AddView(CreateTitleTable());

			ProgressBar pb = new ProgressBar(this);
			pb.Indeterminate = true;
			pb.Visibility = ViewStates.Visible;
			pb.ScaleX = 1.5F;
			pb.ScaleY = 1.5F;
			pb.SetPadding(0, 300, 0, 0);
			linearLayout.AddView(pb);

			await AsyncMainTable();
			pb.Visibility = ViewStates.Gone;

			ScrollView scv = new ScrollView(this);
			scv.AddView(mainTable);
			linearLayout.AddView(scv);

			var toast = Toast.MakeText(this, "결과: " + SwdFile.wcdList.Count.ToString() + "개 항목",
				ToastLength.Long);
			toast.Show();
		}
	}
}
