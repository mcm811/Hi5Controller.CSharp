using System;
using System.Collections.Generic;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Com.Changyoung.HI5Controller
{
	class WeldCountAdapter : ArrayAdapter<JobFile>
	{
		public WeldCountAdapter(Context context, int textViewResourceId) : base(context, textViewResourceId)
		{
		}

		public WeldCountAdapter(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
		{
		}

		public WeldCountAdapter(Context context, int textViewResourceId, IList<JobFile> objects) : base(context, textViewResourceId, objects)
		{
		}

		public WeldCountAdapter(Context context, int textViewResourceId, JobFile[] objects) : base(context, textViewResourceId, objects)
		{
		}

		public WeldCountAdapter(Context context, int resource, int textViewResourceId) : base(context, resource, textViewResourceId)
		{
		}

		public WeldCountAdapter(Context context, int resource, int textViewResourceId, IList<JobFile> objects) : base(context, resource, textViewResourceId, objects)
		{
		}

		public WeldCountAdapter(Context context, int resource, int textViewResourceId, JobFile[] objects) : base(context, resource, textViewResourceId, objects)
		{
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View row;
			WeldCountRowViewHolder viewHolder;

			if (convertView == null) {
				row = LayoutInflater.From(Context).Inflate(Resource.Layout.weld_count_row, null, false);
				viewHolder = new WeldCountRowViewHolder(row);
				row.Tag = viewHolder;
			} else {
				row = convertView;
				viewHolder = (WeldCountRowViewHolder)row.Tag;
			}

			viewHolder.Update(GetItem(position));

			return row;
		}
	}
}