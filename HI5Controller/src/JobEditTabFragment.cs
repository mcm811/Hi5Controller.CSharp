using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.App;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using FragmentStatePagerAdapter = Android.Support.V4.App.FragmentStatePagerAdapter;
using Android.Util;
using System.Text;

namespace Com.Changyoung.HI5Controller
{
	public class JobEditTabFragment : Fragment
	{
		List<JobFile> mJobFileList;

		private void LogDebug(string msg)
		{
			Log.Debug(Context.PackageName, "JobEditTabFragment: " + msg);
		}

		private void ToastShow(string str)
		{
			Toast.MakeText(Context, str, ToastLength.Short).Show();
			LogDebug(str);
		}

		public string PrefPath
		{
			get
			{
				try {
					using (var prefs = Application.Context.GetSharedPreferences(Context.PackageName, FileCreationMode.Private)) {
						return prefs.GetString("dirpath_file", Android.OS.Environment.ExternalStorageDirectory.AbsolutePath);
					}
				} catch {
					return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
				}
			}
			set
			{
				try {
					if (PrefPath != value) {
						using (var prefs = Application.Context.GetSharedPreferences(Context.PackageName, FileCreationMode.Private)) {
							var prefEditor = prefs.Edit();
							prefEditor.PutString("dirpath_file", value);
							prefEditor.Commit();
							ToastShow("경로 저장: " + value);
						}
					}
				} catch {

				}
			}
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.JobEditTabFragment, container, false);
			var textView = view.FindViewById<TextView>(Resource.Id.textView);
			mJobFileList = new List<JobFile>();

			StringBuilder sbJobEdit = new StringBuilder();

			try {
				var dir = new DirectoryInfo(PrefPath);
				foreach (var item in dir.GetFileSystemInfos()) {
					if (item.FullName.EndsWith(".JOB")) {
						var file = new JobFile(item.FullName);
						var cn = file.GetCN();
						if (cn.Length > 0) {
							sbJobEdit.AppendLine(item.Name);
							sbJobEdit.AppendLine(cn);
							mJobFileList.Add(file);
						}
					}
				}
			} catch {
			}

			textView.Text = sbJobEdit.ToString();

			return view;
		}
	}
}