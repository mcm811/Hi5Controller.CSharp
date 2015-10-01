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

namespace Com.Changyoung.HI5Controller
{
	public class WcdListRowViewHolder : Java.Lang.Object
	{
		public TextView tvOutputData { get; private set; }
		public TextView tvOutputType { get; private set; }
		public TextView tvSqueezeForce { get; private set; }
		public TextView tvMoveTipClearance { get; private set; }
		public TextView tvFixedTipClearance { get; private set; }
		public TextView tvPannelThickness { get; private set; }
		public TextView tvCommandOffset { get; private set; }

		public WcdListRowViewHolder(View row)
		{
			tvOutputData = row.FindViewById<TextView>(Resource.Id.tvOutputData);
			tvOutputType = row.FindViewById<TextView>(Resource.Id.tvOutputType);
			tvSqueezeForce = row.FindViewById<TextView>(Resource.Id.tvSqueezeForce);
			tvMoveTipClearance = row.FindViewById<TextView>(Resource.Id.tvMoveTipClearance);
			tvFixedTipClearance = row.FindViewById<TextView>(Resource.Id.tvFixedTipClearance);
			tvPannelThickness = row.FindViewById<TextView>(Resource.Id.tvPannelThickness);
			tvCommandOffset = row.FindViewById<TextView>(Resource.Id.tvCommandOffset);
		}

		public void Update(WeldConditionData item)
		{
			tvOutputData.Text = item.OutputData;
			tvOutputType.Text = item.OutputType;
			tvSqueezeForce.Text = item.SqueezeForce;
			tvMoveTipClearance.Text = item.MoveTipClearance;
			tvFixedTipClearance.Text = item.FixedTipClearance;
			tvPannelThickness.Text = item.PannelThickness;
			tvCommandOffset.Text = item.CommandOffset;
		}
	}
}