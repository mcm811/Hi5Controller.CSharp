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
	public class JobEditTabFragment : Fragment
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
		}

		public void Refresh(bool forced = false)
		{
			if (forced || dirPath != PrefPath || jobFileList.Count == 0) {
				dirPath = PrefPath;
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
			dirPath = PrefPath;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate(Resource.Layout.JobEditTabFragment, container, false);
			textView = view.FindViewById<TextView>(Resource.Id.textView);

			Refresh(forced: true);

			return view;
		}
	}
}