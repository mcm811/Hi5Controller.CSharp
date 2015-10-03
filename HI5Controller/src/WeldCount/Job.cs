using System;
using System.Collections.Generic;
using System.Text;
using Android.Content;
using System.IO;
using Android.Util;

namespace Com.Changyoung.HI5Controller
{
	public class Job : Java.Lang.Object
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

		public static void LogDebug(string msg)
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
			//string version;
			//string mechType;
			//string totalAxis;
			//string auxAxis;

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
				//LogDebug("s.Length: " + s.Length.ToString());
				if (s.Length == 2) {
					var f = s[1].Split(new char[] { ',' });
					//LogDebug("f.Lenght: " + f.Length.ToString());
					for (int i = 0; i < f.Length; i++) {
						mJobValueList.Add(new JobValue(f[i]));
						//LogDebug("f" + i.ToString() + ": " + f[i]);
					}
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
			//int step;
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

		public FileInfo fi { get; private set; }

		public JobCount(string fileName)
		{
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

			if (total > 0)
				sb.Append("Total: " + total.ToString());

			int n = 1;
			foreach (var item in gn) {
				sb.Append(",  GN" + n++.ToString() + ": " + item.ToString());
			}
			n = 1;
			foreach (var item in g) {
				sb.Append(",  G" + n++.ToString() + ": " + item.ToString());
			}
			if (step > 0) {
				if (total > 0)
					sb.Append(",  ");
				sb.Append("Step: " + step.ToString());
			}

			//Log.Debug("JobCount", sb.ToString());

			return sb.ToString();
		}
	}

	public class JobFile
	{
		private List<Job> jobList;
		private JobCount jobCount;

		public Job this[int index]
		{
			get { return jobList[index]; }
			set { jobList[index] = value; }
		}

		public int Count
		{
			get { return jobList.Count; }
		}

		public JobCount JobCount
		{
			get { return jobCount; }
		}

		public JobFile(string fileName, Context context = null)
		{
			jobList = new List<Job>();
			jobCount = new JobCount(fileName);
			ReadFile(fileName, jobList, context);
			BuildJobCount(jobList, jobCount);
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
				}
			} catch {
			}
			return items;
		}

		private void SaveFile(string fileName, List<Job> items, Context context = null)
		{
			try {
				using (var sw = context != null ? new StreamWriter(context.Assets.Open(fileName), Encoding.GetEncoding("euc-kr")) : new StreamWriter(fileName, false, Encoding.GetEncoding("euc-kr"))) {
					foreach (var item in items) {
						sw.WriteLine(item.RowString);
					}
					sw.Close();
					Job.LogDebug("저장 완료:" + fileName);
				}
				jobCount = null;
				jobCount = new JobCount(fileName);
				BuildJobCount(jobList, jobCount);
			} catch {
				Job.LogDebug("저장 실패:" + fileName);
			}
		}

		public void SaveFile()
		{
			SaveFile(jobCount.fi.FullName, jobList);
		}

		private void BuildJobCount(List<Job> jobList, JobCount jobCount)
		{
			foreach (var job in jobList) {
				var cn = job.CN;
				if (cn != null) {
					jobCount.Total++;
				}
				var gn = job.GN;
				if (gn != null) {
					jobCount.IncreaseGN(gn);
				}
				var g = job.G;
				if (g != null) {
					jobCount.IncreaseG(g);
				}
				if (job.RowType == Job.RowTypes.Move) {
					jobCount.Step++;
				}
				if (job.RowType == Job.RowTypes.Comment) {
					if (jobCount.Preview == null)
						jobCount.Preview = job.RowString.Trim();
				}
			}
		}

		public string GetCNList()
		{
			StringBuilder sb = new StringBuilder();
			int n = 0;
			foreach (var job in jobList) {
				var cn = job.CN;
				if (cn != null) {
					if (++n > 200) {	// 200개 까지만 보여줌
						sb.Append("...");
						break;
					}
					sb.Append(cn + "  ");
				}
			}
			if (sb.Length > 0)
				sb.Insert(0, "CN: ");
			return sb.ToString();
		}

		public string GetCNTest()
		{
			StringBuilder sb = new StringBuilder();

			foreach (var job in jobList) {
				var cn = job.CN;
				if (cn != null)
					sb.Append(job.RowNumber.ToString() + ": CN=" + cn + "\n");
			}

			return sb.ToString();
		}

		public string RowText()
		{
			StringBuilder sb = new StringBuilder();
			foreach (var Job in jobList) {
				sb.AppendLine(Job.RowString);
			}
			return sb.ToString();
		}

		public void UpdateCN(int start)
		{
			foreach (var job in jobList) {
				if (job.RowType == Job.RowTypes.Spot) {
					job.CN = start++.ToString();
				}
			}
		}

		public void UpdateCN(int index, string value)
		{
			jobList[index].CN = value;
		}
	}
}
