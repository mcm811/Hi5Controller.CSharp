using Android.Util;
using System;

namespace HI5Controller
{
	public class WeldConditionData
	{
		private int outputData;              // ��� ������
		private int outputType;              // ��� Ÿ��
		private int squeezeForce;            // ���з�
		private decimal moveTipClearance;    // �̵��� ������
		private decimal fixedTipClearance;   // ������ ������
		private decimal pannelThickness;     // �г� �β�
		private decimal commandOffset;       // ��� �ɼ�

		private bool itemChecked;

        public bool ItemChecked
		{
			get { return itemChecked; }
			set { itemChecked = value; }
		}

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

		public string FixedTipClearance {
			get { return string.Format("{0:F1}", fixedTipClearance); }
			set { fixedTipClearance = Convert.ToDecimal(value); }
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
				return (outputData < 10 ? "\t- " : "\t-") + OutputData + "=" + OutputData + "," + OutputType + "," + SqueezeForce + "," + MoveTipClearance + "," + FixedTipClearance + "," + PannelThickness + "," + CommandOffset;
			}
			set {
				string[] ds = value.Trim().Split(new char[] { '=' });
				if (ds.Length == 2) {
					string[] data = ds[1].Trim().Split(new char[] { ',', '-' });
					if (data.Length == 7) {
						//OutputData = data[0];
						//OutputType = data[1];
						//SqueezeForce = data[2];
						//MoveTipClearance = data[3];
						//FixedTipClearance = data[4];
						//PannelThickness = data[5];
						//CommandOffset = data[6];

						OutputData = data[0].Trim();
						OutputType = data[1].Trim();
						SqueezeForce = data[2].Trim();
						MoveTipClearance = data[3].Trim();
						FixedTipClearance = data[4].Trim();
						PannelThickness = data[5].Trim();
						CommandOffset = data[6].Trim();
					}
				}
			}
		}

		// C# �ε��� �迭ó�� ���� �ְԵ�
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
					return FixedTipClearance;
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
					FixedTipClearance = value;
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