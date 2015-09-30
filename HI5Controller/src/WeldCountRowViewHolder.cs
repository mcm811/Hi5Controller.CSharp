using Android.Views;
using Android.Widget;

namespace Com.Changyoung.HI5Controller
{
	public class WeldCountRowViewHolder : Java.Lang.Object
	{
		public TextView tvWeldCount { get; private set; }

		//public TextView tvFileName { get; private set; }
		//public TextView tvPreview { get; private set; }
		//public TextView tvCount { get; private set; }
		//public TextView tvTime { get; private set; }
		//public TextView tvSize { get; private set; }

		public WeldCountRowViewHolder(View row)
		{
			tvWeldCount = row.FindViewById<TextView>(Resource.Id.tvWeldCount);
		}

		public void Update(JobFile jobFile)
		{
			tvWeldCount.Text = jobFile.GetCount();
			//tvFileName.Text = jobFile.JobCount.fileName;
		}
	}
}