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

namespace HI5Controller
{
	class WcdListViewAdapter : BaseAdapter<WeldConditionData>
	{
		private List<WeldConditionData> mItems;
		private Context mContent;

		public WcdListViewAdapter(Context context, List<WeldConditionData> items)
		{
			mItems = items;
			mContent = context;
		}

		public override WeldConditionData this[int position]
		{
			get
			{
				return mItems[position];
			}
		}

		public override int Count
		{
			get
			{
				return mItems.Count;
			}
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View row = convertView;

			if (row == null) {
				row = LayoutInflater.From(mContent).Inflate(Resource.Layout.WcdListViewRow, null, false);
			}

			TextView tvOutputData = row.FindViewById<TextView>(Resource.Id.tvOutputData);
			tvOutputData.Text = mItems[position].OutputData;

			TextView tvOutputType = row.FindViewById<TextView>(Resource.Id.tvOutputType);
			tvOutputType.Text = mItems[position].OutputType;

			TextView tvSqueezeForce = row.FindViewById<TextView>(Resource.Id.tvSqueezeForce);
			tvSqueezeForce.Text = mItems[position].SqueezeForce;

			TextView tvMoveTipClearance = row.FindViewById<TextView>(Resource.Id.tvMoveTipClearance);
			tvMoveTipClearance.Text = mItems[position].MoveTipClearance;

			TextView tvFixedTipClearance = row.FindViewById<TextView>(Resource.Id.tvFixedTipClearance);
			tvFixedTipClearance.Text = mItems[position].FixedTipClearance;

			TextView tvPannelThickness = row.FindViewById<TextView>(Resource.Id.tvPannelThickness);
			tvPannelThickness.Text = mItems[position].PannelThickness;

			TextView tvCommandOffset = row.FindViewById<TextView>(Resource.Id.tvCommandOffset);
			tvCommandOffset.Text = mItems[position].CommandOffset;

			return row;
		}
	}
}