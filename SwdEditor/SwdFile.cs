using System;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using Android.Util;

namespace SwdEditor
{
	public static class SwdFile
	{
		public static readonly IList<WeldConditionData> wcdList = new List<WeldConditionData>();

		public static IList<string> WcdIListString {
			set {
				foreach (string wcdString in value) {
					SwdFile.wcdList.Add(new WeldConditionData(wcdString));
				}
			}
		}

		public static IList<string> GetStringArrayList(string swdString) {
			IList<string> swdList = new List<string>();
			bool addText = false;
			foreach (string swdLine in swdString.Split('\n')) {
				if (swdLine.StartsWith("#006"))
					break;
				if (addText && swdLine.Trim().Length > 0)
					swdList.Add(swdLine);
				if (swdLine.StartsWith("#005"))
					addText = true;
			}
			return swdList;
		}

		public static void SetWCDList(List<string> swdList, List<WeldConditionData> wcdList) {

		}
	}
}
