namespace com.xamarin.recipes.filepicker
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	using Android.OS;
	using Android.Util;
	using Android.Views;
	using Android.Widget;
	using Android.Content;
	using Android.Support.Design.Widget;

	/// <summary>
	///   A ListFragment that will show the files and subdirectories of a given directory.
	/// </summary>
	/// <remarks>
	///   <para> This was placed into a ListFragment to make this easier to share this functionality with with tablets. </para>
	///   <para> Note that this is a incomplete example. It lacks things such as the ability to go back up the directory tree, or any special handling of a file when it is selected. </para>
	/// </remarks>
	public class FileListFragment : Android.Support.V4.App.ListFragment
	{
		private View view;
		private FileListAdapter fileListAdapter;
		private DirectoryInfo directoryInfo;

		public string PrefKey { get; set; }

		public View SnackbarView { get; set; }

		private void LogDebug(string msg)
		{
			try {
				Log.Debug(Context.PackageName, "FileListFragment: " + msg);
			} catch { }
		}

		private void ToastShow(string str)
		{
			try {
				if (SnackbarView != null)
					Snackbar.Make(SnackbarView, str, Snackbar.LengthLong).Show();
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
			fileListAdapter = new FileListAdapter(Activity, new FileSystemInfo[0]);
			ListAdapter = fileListAdapter;
			DirPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = base.OnCreateView(inflater, container, savedInstanceState);
			return view;
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			ListView.LongClick += (object sender, View.LongClickEventArgs e) =>
			{
				ToastShow("길게 클릭");
			};
		}

		public override void OnResume()
		{
			base.OnResume();
			RefreshFilesList(DirPath);
		}

		public override void OnPause()
		{
			base.OnPause();
		}

		public override void OnListItemClick(ListView l, View v, int position, long id)
		{
			var fileSystemInfo = fileListAdapter.GetItem(position);
			if (position == 0) {
				RefreshFilesList(Path.GetDirectoryName(fileSystemInfo.FullName));
			} else if (fileSystemInfo.IsDirectory()) {
				RefreshFilesList(fileSystemInfo.FullName);
			} else {
				Com.Changyoung.HI5Controller.Pref.TextViewDialog(Context, fileSystemInfo.FullName);
			}

			base.OnListItemClick(l, v, position, id);
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
				if (PrefKey != null)
					Com.Changyoung.HI5Controller.Pref.SetPath(PrefKey, dir.FullName);
			} catch (Exception ex) {
				//Log.Error("FileListFragment", "Couldn't access the directory " + directoryInfo.FullName + "; " + ex);
				return DirPath;
			}
			fileListAdapter.AddDirectoryContents(visibleThings);

			// If we don't do this, then the ListView will not update itself when then data set 
			// in the adapter changes. It will appear to the user that nothing has happened.
			ListView.RefreshDrawableState();

			//Log.Verbose("FileListFragment", "Displaying the contents of directory {0}.", directory);

			return DirPath;
		}
	}
}
