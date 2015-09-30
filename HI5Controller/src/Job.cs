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
using System.IO;
using Android.Util;

namespace Com.Changyoung.HI5Controller
{
	public class Job
	{
		public enum RowTypes
		{
			Header,
			Comment,
			Spot,
			Move,
			Wait,
			Do,
			Call,
			End,
			Etc
		}

		public class JobValue
		{
			private string mType;
			private string mValue;

			public JobValue(string str)
			{
				Update = str;
			}

			public string Update
			{
				get
				{
					return Type + "=" + Value;
				}
				set
				{
					if (value != null) {
						var s = value.Trim().Split(new char[] { '=' });
						if (s.Length == 2) {
							Type = s[0];
							Value = s[1];
						}
					}
				}
			}

			public string Type
			{
				get
				{
					return mType;
				}

				set
				{
					mType = value;
				}
			}

			public string Value
			{
				get
				{
					return mValue;
				}

				set
				{
					mValue = value;
				}
			}
		}

		RowJob row;

		private static void LogDebug(string msg)
		{
			Log.Debug("Job", "[" + msg + "]");
		}

		private RowTypes GetRowType(string rowString)
		{
			RowTypes rowType = RowTypes.Etc;
			string[] s = rowString.Trim().Split(new char[] { ' ' });
			if (s.Length > 0) {
				if (s[0] == "Program")
					rowType = RowTypes.Header;
				else if (s[0].StartsWith("'"))
					rowType = RowTypes.Comment;
				else if (s[0].StartsWith("SPOT"))
					rowType = RowTypes.Spot;
				else if (s[0].StartsWith("S"))
					rowType = RowTypes.Move;
				else if (s[0].StartsWith("WAIT"))
					rowType = RowTypes.Wait;
				else if (s[0].StartsWith("DO"))
					rowType = RowTypes.Do;
				else if (s[0].StartsWith("END"))
					rowType = RowTypes.End;
			}
			return rowType;
		}

		public Job(int rowNumber, string rowString)
		{
			RowTypes rowType = GetRowType(rowString);

			//LogDebug(rowType.ToString() + "::::" + rowString);

			switch (rowType) {
				case RowTypes.Header:
				row = new HeaderJob(rowNumber, rowString);
				break;
				case RowTypes.Comment:
				row = new CommentJob(rowNumber, rowString);
				break;
				case RowTypes.Spot:
				row = new SpotJob(rowNumber, rowString);
				break;
				case RowTypes.Move:
				row = new MoveJob(rowNumber, rowString);
				break;
				case RowTypes.Wait:
				row = new WaitJob(rowNumber, rowString);
				break;
				case RowTypes.Do:
				row = new DoJob(rowNumber, rowString);
				break;
				case RowTypes.Call:
				row = new CallJob(rowNumber, rowString);
				break;
				case RowTypes.End:
				row = new EndJob(rowNumber, rowString);
				break;
				case RowTypes.Etc:
				row = new EtcJob(rowNumber, rowString);
				break;
			}
		}

		public string CN
		{
			get
			{
				if (RowType == RowTypes.Spot)
					return ((SpotJob)row).CN;
				else
					return null;
			}
			set
			{
				if (RowType == RowTypes.Spot)
					((SpotJob)row).CN = value;
			}
		}

		public string GN
		{
			get
			{
				if (RowType == RowTypes.Spot)
					return ((SpotJob)row).GN;
				else
					return null;
			}
		}

		public string G
		{
			get
			{
				if (RowType == RowTypes.Spot)
					return ((SpotJob)row).G;
				else
					return null;
			}
		}

		public RowTypes RowType
		{
			get
			{
				return row.RowType;
			}
		}

		public int RowNumber
		{
			get
			{
				return row.mRowNumber;
			}
		}

		public string RowString
		{
			get
			{
				return row.mRowString;
			}
		}

		public class RowJob
		{
			public RowTypes mRowType;
			public int mRowNumber;
			public string mRowString;

			public RowTypes RowType
			{
				get
				{
					return mRowType;
				}

				set
				{
					mRowType = value;
				}
			}

			public RowJob(RowTypes rowType, int rowNumber, string rowString)
			{
				RowType = rowType;
				mRowNumber = rowNumber;
				mRowString = rowString;
			}
		}

		public class HeaderJob : RowJob
		{
			string version;
			string mechType;
			string totalAxis;
			string auxAxis;

			public HeaderJob(int rowNumber, string rowString) : base(RowTypes.Header, rowNumber, rowString)
			{ }
		}

		public class CommentJob : RowJob
		{
			public CommentJob(int rowNumber, string rowString) : base(RowTypes.Comment, rowNumber, rowString)
			{ }
		}

		public class SpotJob : RowJob
		{
			List<JobValue> mJobValueList;

			public SpotJob(int rowNumber, string rowString) : base(RowTypes.Spot, rowNumber, rowString)
			{
				mJobValueList = new List<JobValue>();
				var s = rowString.Trim().Split(new char[] { ' ' });
				LogDebug("s.Length: " + s.Length.ToString());
				if (s.Length == 2) {
					var f = s[1].Split(new char[] { ',' });
					LogDebug("f.Lenght: " + f.Length.ToString());
					for (int i = 0; i < f.Length; i++) {
						mJobValueList.Add(new JobValue(f[i]));
						LogDebug("f" + i.ToString() + ": " + f[i]);
					}
					//if (f.Length == 3) {
					//	mJobValueList.Add(new JobValue(f[0]));
					//	mJobValueList.Add(new JobValue(f[1]));
					//	mJobValueList.Add(new JobValue(f[2]));
					//	//LogDebug("f0: " + f[0]);
					//	//LogDebug("f1: " + f[1]);
					//	//LogDebug("f2: " + f[2]);
					//}
				}
			}

			public void Update()
			{
				mRowString = "     SPOT " + mJobValueList[0].Update + "," + mJobValueList[1].Update + "," + mJobValueList[2].Update;
			}

			public string CN
			{
				get
				{
					foreach (var s in mJobValueList) {
						if (s.Type == "CN")
							return s.Value;
					}
					return null;
				}

				set
				{
					foreach (var s in mJobValueList) {
						if (s.Type == "CN") {
							s.Value = value;
							Update();
						}
					}
				}
			}

			public string GN
			{
				get
				{
					foreach (var s in mJobValueList) {
						if (s.Type == "GN")
							return s.Value;
					}
					return null;
				}
			}

			public string G
			{
				get
				{
					foreach (var s in mJobValueList) {
						if (s.Type == "G")
							return s.Value;
					}
					return null;
				}
			}
		}

		public class MoveJob : RowJob
		{
			int step;
			public MoveJob(int rowNumber, string rowString) : base(RowTypes.Move, rowNumber, rowString)
			{ }
		}

		public class WaitJob : RowJob
		{
			public WaitJob(int rowNumber, string rowString) : base(RowTypes.Wait, rowNumber, rowString)
			{ }
		}

		public class DoJob : RowJob
		{
			public DoJob(int rowNumber, string rowString) : base(RowTypes.Do, rowNumber, rowString)
			{ }
		}

		public class CallJob : RowJob
		{
			public CallJob(int rowNumber, string rowString) : base(RowTypes.Call, rowNumber, rowString)
			{ }
		}

		public class EndJob : RowJob
		{
			public EndJob(int rowNumber, string rowString) : base(RowTypes.End, rowNumber, rowString)
			{ }
		}

		public class EtcJob : RowJob
		{
			public EtcJob(int rowNumber, string rowString) : base(RowTypes.Etc, rowNumber, rowString)
			{ }
		}
	}

	public class JobCount
	{
		private int total;           // SPOT 의 총 카운트
		private List<int> gn;        // SPOT 단어 다음에 나오는 첫번째 단어 분석 해서 종류 결정(GN1, GN2, GN3, G1, G2)
		private List<int> g;
		private int step;            // S1 S2 S3 붙은 것들 젤 마지막 S번호 값
		private string preview;

		private FileInfo fi;
		private string fileName;

		public JobCount(string fileName)
		{
			this.fileName = fileName;
			try {
				fi = new FileInfo(fileName);
			} catch {
				fi = null;
			}
			gn = new List<int>();
			g = new List<int>();
		}

		public void IncreaseGN(string strIndex)
		{
			int index;
			//Log.Debug("JobCount", "GN:" + strIndex);
			if (Int32.TryParse(strIndex, out index)) {
				if (gn.Count < index) {
					for (int i = gn.Count; i < index; i++) {
						gn.Add(0);
					}
				}
				gn[index - 1]++;
			}
		}

		public void IncreaseG(string strIndex)
		{
			int index;
			//Log.Debug("JobCount", "G:" + strIndex);
			if (Int32.TryParse(strIndex, out index)) {
				if (g.Count < index) {
					for (int i = g.Count; i < index; i++) {
						g.Add(0);
					}
				}
				g[index - 1]++;
			}
		}

		public int Total
		{
			get { return total; }
			set { total = value; }
		}

		public int Step
		{
			get { return step; }
			set { step = value; }
		}

		public string Preview
		{
			get { return preview; }
			set { preview = value; }
		}

		public string GetString()
		{
			StringBuilder sb = new StringBuilder();
			int n;
			if (fi != null) {
				sb.Append(fi.Name);
				if (preview != null) sb.Append("    " + preview);
				sb.Append("\n");

				sb.Append("Total: " + total.ToString());
				n = 1;
				foreach (var item in gn) {
					sb.Append(",  GN" + n++.ToString() + ": " + item.ToString());
				}
				n = 1;
				foreach (var item in g) {
					sb.Append(",  G" + n++.ToString() + ": " + item.ToString());
				}
				sb.AppendLine(",  Step: " + step.ToString());


				sb.Append("날짜: " + fi.LastWriteTime.ToString());
				sb.Append("    크기: " + fi.Length.ToString() + "B");

				Log.Debug("JobCount", sb.ToString());
			}

			return sb.ToString();
		}
	}

	public class JobFile
	{
		private string mFileName;
		private List<Job> mJobList;
		private JobCount mJc;

		public JobCount JobCount
		{
			get { return mJc; }
		}

		public JobFile(string fileName, Context context = null)
		{
			mFileName = fileName;
			mJobList = new List<Job>();
			mJc = new JobCount(fileName);
			ReadFile(mFileName, mJobList, context);
		}

		private List<Job> ReadFile(string fileName, List<Job> items, Context context = null)
		{
			try {
				using (var sr = context != null ? new StreamReader(context.Assets.Open(fileName), Encoding.GetEncoding("euc-kr")) : new StreamReader(fileName, Encoding.GetEncoding("euc-kr"))) {
					string rowString;
					int rowNumber = 0;
					while ((rowString = sr.ReadLine()) != null) {
						items.Add(new Job(rowNumber++, rowString));
					}
					sr.Close();
					//LogDebug("불러 오기:" + fileName);
				}
			} catch {
				//ToastShow("읽기 실패:" + fileName);
			}
			return items;
		}

		public string GetCount()
		{
			Count(mJc);
			return mJc.GetString();
		}

		public string GetCN()
		{
			StringBuilder sb = new StringBuilder();

			foreach (var job in mJobList) {
				var cn = job.CN;
				if (cn != null) {
					sb.Append(job.RowNumber.ToString() + ": CN=" + cn + "\n");
				}
			}

			return sb.ToString();
		}

		public void Count(JobCount jc)
		{
			foreach (var job in mJobList) {
				var cn = job.CN;
				if (cn != null) {
					jc.Total++;
				}
				var gn = job.GN;
				if (gn != null) {
					jc.IncreaseGN(gn);
				}
				var g = job.G;
				if (g != null) {
					jc.IncreaseG(g);
				}
				if (job.RowType == Job.RowTypes.Move) {
					jc.Step++;
				}
				if (job.RowType == Job.RowTypes.Comment) {
					if (jc.Preview == null)
						jc.Preview = job.RowString.Trim();
				}
			}
		}
	}
}
