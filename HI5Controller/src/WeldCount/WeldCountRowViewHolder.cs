using Android.Views;
using Android.Widget;

namespace Com.Changyoung.HI5Controller
{
	public class WeldCountRowViewHolder : Java.Lang.Object
	{
		public TextView tvFileName { get; private set; }
		public TextView tvTime { get; private set; }
		public TextView tvSize { get; private set; }
		public TextView tvCount { get; private set; }
		public TextView tvPreview { get; private set; }
		public TextView tvCN { get; private set; }

		public WeldCountRowViewHolder(View row)
		{
			tvFileName = row.FindViewById<TextView>(Resource.Id.tvFileName);
			tvTime = row.FindViewById<TextView>(Resource.Id.tvTime);
			tvSize = row.FindViewById<TextView>(Resource.Id.tvSize);
			tvCount = row.FindViewById<TextView>(Resource.Id.tvCount);
			tvPreview = row.FindViewById<TextView>(Resource.Id.tvPreview);
			tvCN = row.FindViewById<TextView>(Resource.Id.tvCN);
		}

		public void Update(JobFile jobFile)
		{
			tvFileName.Text = jobFile.JobCount.fi.Name;
			tvTime.Text = jobFile.JobCount.fi.LastWriteTime.ToString();
			tvSize.Text = jobFile.JobCount.fi.Length.ToString() + "B";
			tvCount.Text = jobFile.JobCount.GetString();
			tvPreview.Text = jobFile.JobCount.Preview;
			tvCN.Text = jobFile.GetCNList();
		}
	}
}