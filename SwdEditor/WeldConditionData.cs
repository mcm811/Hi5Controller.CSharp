using System;

namespace SwdEditor
{
	public class WeldConditionData
	{
		public int outputData;              // 출력 데이터
		public int outputType;              // 출력 타입
		public int squeezeForce;            // 가압력
		public decimal moveTipClearance;    // 이동극 제거율
		public decimal fixedTipClearence;   // 고정극 제거율
		public decimal pannelThickness;     // 패널 두께
		public decimal commandOffset;       // 명령 옵셋

		public int Count {
			get { return 7; }
		}

		public string OutputData {
			get { return outputData.ToString(); }
			set { outputData = Convert.ToInt32(value); }
		}

		public string OutputType {
			get { return outputType.ToString(); }
			set { outputType = Convert.ToInt32(value); }
		}

		public string SqueezeForce {
			get { return squeezeForce.ToString(); }
			set { squeezeForce = Convert.ToInt32(value); }
		}

		public string MoveTipClearance {
			get { return string.Format("{0:F1}", moveTipClearance); }
			set { moveTipClearance = Convert.ToDecimal(value); }
		}

		public string FixedTipClearence {
			get { return string.Format("{0:F1}", fixedTipClearence); }
			set { fixedTipClearence = Convert.ToDecimal(value); }
		}

		public string PannelThickness {
			get { return string.Format("{0:F1}", pannelThickness); }
			set { pannelThickness = Convert.ToDecimal(value); }
		}

		public string CommandOffset {
			get { return string.Format("{0:F1}", commandOffset); }
			set { commandOffset = Convert.ToDecimal(value); }
		}

		public string WcdString {
			get {
				string format = outputData < 10 ? "\t- {0:D}={0:D},{1:D},{2:D},{3:F1},{4:F1},{5:F1},{6:F1]" : "\t-{0:D}={0:D},{1:D},{2:D},{3:F1},{4:F1},{5:F1},{6:F1]";
				return string.Format(format, outputData, outputType, squeezeForce, moveTipClearance, fixedTipClearence, pannelThickness, commandOffset);
			}
			set {
				string[] ds = value.Trim().Split(new char[] { '=' });
				if (ds.Length == 2) {
					string[] data = ds[1].Trim().Split(new char[] { ',', '-' });
					if (data.Length == 7) {
						OutputData = data[0].Trim();
						OutputType = data[1].Trim();
						SqueezeForce = data[2].Trim();
						MoveTipClearance = data[3].Trim();
						FixedTipClearence = data[4].Trim();
						PannelThickness = data[5].Trim();
						CommandOffset = data[6].Trim();
					}
				}
			}
		}

		// C# 인덱서 배열처럼 쓸수 있게됨
		public string this[int index] {
			get {
				switch (index) {
					case 0:
					return OutputData;
					case 1:
					return OutputType;
					case 2:
					return SqueezeForce;
					case 3:
					return MoveTipClearance;
					case 4:
					return FixedTipClearence;
					case 5:
					return PannelThickness;
					case 6:
					return CommandOffset;
				}

				return string.Empty;
			}

			set {
				switch (index) {
					case 0:
					OutputData = value;
					break;
					case 1:
					OutputType = value;
					break;
					case 2:
					SqueezeForce = value;
					break;
					case 3:
					MoveTipClearance = value;
					break;
					case 4:
					FixedTipClearence = value;
					break;
					case 5:
					PannelThickness = value;
					break;
					case 6:
					CommandOffset = value;
					break;
				}
			}
		}

		public WeldConditionData(string s) {
			WcdString = s;
		}

		public WeldConditionData() { }
	}
}