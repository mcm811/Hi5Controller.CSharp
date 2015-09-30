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
using Android.Util;

namespace Com.Changyoung.HI5Controller
{
	class WcdListAdapter : BaseAdapter<WeldConditionData>
	{
		private List<WeldConditionData> mItems;
		private Context mContent;

		private readonly Color defaultBackgroundColor = Color.Transparent;
		private Color selectedBackGroundColor = Color.LightGray;

		public WcdListAdapter(Context context, List<WeldConditionData> items)
		{
			mItems = items;
			mContent = context;
			selectedBackGroundColor = Color.ParseColor("#F48FB1");
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
				row = LayoutInflater.From(mContent).Inflate(Resource.Layout.WcdListRow, null, false);
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
