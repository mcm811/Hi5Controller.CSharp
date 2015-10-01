using System.IO;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Fragment = Android.Support.V4.App.Fragment;
using Android.Util;
using System.Text;

namespace Com.Changyoung.HI5Controller
{
	public class JobEditTabFragment : Fragment, IRefresh
	{
		View view;
		TextView textView;

		List<JobFile> jobFileList;

		private string dirPath;

		private void LogDebug(string msg)
		{
			Log.Debug(Context.PackageName, "JobEditTabFragment: " + msg);
		}

		private void ToastShow(string str)
		{
			Toast.MakeText(Context, str, ToastLength.Short).Show();
			LogDebug(str);
		}

		public void Refresh(bool forced = false)
		{
			if (forced || dirPath != Pref.Path || jobFileList.Count == 0) {
				dirPath = Pref.Path;
				try {
					var sb = new StringBuilder();
					var dir = new DirectoryInfo(dirPath);
					jobFileList.Clear();
					foreach (var item in dir.GetFileSystemInfos()) {
						if (item.FullName.EndsWith(".JOB")) {
							var file = new JobFile(item.FullName);
							var cn = file.GetCNTest();
							if (cn.Length > 0) {
								sb.AppendLine(item.Name);
								sb.AppendLine(cn);
								jobFileList.Add(file);
							}
						}
					}
					textView.Text = sb.ToString();
				} catch {
				}
			}
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			jobFileList = new List<JobFile>();
			dirPath = Pref.Path;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate(Resource.Layout.job_edit_tab_fragment, container, false);
			textView = view.FindViewById<TextView>(Resource.Id.textView);

			Refresh(forced: true);

			return view;
		}
	}
}