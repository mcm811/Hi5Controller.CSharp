using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using com.xamarin.recipes.filepicker;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Fragment = Android.Support.V4.App.Fragment;

namespace Com.Changyoung.HI5Controller
{
	public class FileListFragment : Fragment
	{
		private View view;
		private ListView listView;
		private FileListAdapter adapter;
		private DirectoryInfo directoryInfo;

		public string PrefKey { get; set; }

		public View SnackbarView { get; set; }

		private void LogDebug(string msg)
		{
			try {
				Log.Debug(Context.PackageName, "FileListFragment: " + msg);
			} catch { }
		}

		private void Show(string str)
		{
			try {
				if (SnackbarView != null)
					Snackbar.Make(SnackbarView, str, Snackbar.LengthShort).Show();
			} catch { }
			LogDebug(str);
		}

		public string DirPath
		{
			get { return directoryInfo.FullName; }
			set { directoryInfo = new DirectoryInfo(value); }
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			adapter = new FileListAdapter(Activity, new FileSystemInfo[0]);
			DirPath = Environment.ExternalStorageDirectory.AbsolutePath;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate(Resource.Layout.file_list_fragment, container, false);

			var refresher = view.FindViewById<SwipeRefreshLayout>(Resource.Id.srl);
			if (refresher != null) {
				refresher.Refresh += delegate
				{
					RefreshFilesList(DirPath);
					refresher.Refreshing = false;
				};
			}

			listView = view.FindViewById<ListView>(Resource.Id.listView);
			listView.Adapter = adapter;
			listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
			{
				var fileSystemInfo = adapter.GetItem(e.Position);
				if (e.Position == 0) {
					RefreshFilesList(Path.GetDirectoryName(fileSystemInfo.FullName));
				} else if (fileSystemInfo.IsDirectory()) {
					RefreshFilesList(fileSystemInfo.FullName);
				} else {
					Pref.TextViewDialog(Context, fileSystemInfo.FullName);
				}
			};
			listView.ItemLongClick += (object sender, AdapterView.ItemLongClickEventArgs e) =>
			{
				if (e.Position == 0)
					return;

				var fileSystemInfo = adapter.GetItem(e.Position);
				var actionName = fileSystemInfo.IsDirectory() ? "폴더 삭제" : "파일 삭제";
				var fileType = fileSystemInfo.IsDirectory() ? "이 폴더를 " : "이 파일을 ";
				var msg = fileType + "완전히 삭제 하시겠습니까?\n\n" + fileSystemInfo.Name + "\n\n수정한 날짜: " + fileSystemInfo.LastWriteTime.ToString();

				var builder = new AlertDialog.Builder(Context);
				builder.SetTitle(actionName)
					   .SetMessage(msg)
					   .SetNegativeButton("취소", delegate { RefreshFilesList(DirPath); })
					   .SetPositiveButton("삭제", delegate
						{
							Directory.Delete(fileSystemInfo.FullName, true);
							RefreshFilesList(DirPath);
						});
				builder.Create().Show();
			};

			return view;
		}

		public override void OnResume()
		{
			base.OnResume();
			RefreshFilesList(DirPath);
		}

		public string RefreshFilesList(string directory = null)
		{
			IList<FileSystemInfo> visibleThings = new List<FileSystemInfo>();
			try {
				if (directory == null)
					directory = DirPath;
				var dir = new DirectoryInfo(directory);
				visibleThings.Add(dir);
				foreach (var item in dir.GetFileSystemInfos().Where(item => item.IsVisible())) {
					visibleThings.Add(item);
				}
				directoryInfo = dir;
				Pref.SetPath(PrefKey, dir.FullName);
				adapter.AddDirectoryContents(visibleThings);
				listView.RefreshDrawableState();
			} catch { }

			return DirPath;
		}
	}
}