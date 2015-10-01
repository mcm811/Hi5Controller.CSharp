using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace Com.Changyoung.HI5Controller
{
	class WcdListAdapter : BaseAdapter<WeldConditionData>
	{
		private List<WeldConditionData> mItems;
		private Context mContext;

		private readonly Color defaultBackgroundColor = Color.Transparent;
		private Color selectedBackGroundColor = Color.LightGray;

		public WcdListAdapter(Context context, List<WeldConditionData> items = null)
		{
			mItems = items == null ? new List<WeldConditionData>() : items;
			mContext = context;
			selectedBackGroundColor = Color.ParseColor("#F48FB1");
		}

		public void Add(WeldConditionData item)
		{
			mItems.Add(item);
		}

		public void Clear()
		{
			mItems.Clear();
		}

		public override WeldConditionData this[int position]
		{
			get { return mItems[position]; }
		}

		public override int Count
		{
			get { return mItems.Count; }
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View row = convertView;
			WcdListRowViewHolder viewHolder;

			if (row == null) {
				row = LayoutInflater.From(mContext).Inflate(Resource.Layout.WcdListRow, null, false);
				viewHolder = new WcdListRowViewHolder(row);
				row.Tag = viewHolder;
			} else {
				viewHolder = (WcdListRowViewHolder)row.Tag;
			}

			row.SetBackgroundColor(this[position].ItemChecked ? selectedBackGroundColor : defaultBackgroundColor);
			viewHolder.Update(this[position]);

			return row;
		}
	}
}
