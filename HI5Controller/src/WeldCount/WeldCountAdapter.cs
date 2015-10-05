using Android.Content;
using Android.Views;
using Android.Widget;

namespace Com.Changyoung.HI5Controller
{
	class WeldCountAdapter : ArrayAdapter<JobFile>
	{
		public WeldCountAdapter(Context context, int textViewResourceId) : base(context, textViewResourceId)
		{ }

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