namespace com.xamarin.recipes.filepicker
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	using Android.OS;
	using Android.Support.V4.App;
	using Android.Util;
	using Android.Views;
	using Android.Widget;
	using Com.Changyoung.HI5Controller;
	using Android.App;
	using Android.Content;

	/// <summary>
	///   A ListFragment that will show the files and subdirectories of a given directory.
	/// </summary>
	/// <remarks>
	///   <para> This was placed into a ListFragment to make this easier to share this functionality with with tablets. </para>
	///   <para> Note that this is a incomplete example. It lacks things such as the ability to go back up the directory tree, or any special handling of a file when it is selected. </para>
	/// </remarks>
	public class FileListFragment : Android.Support.V4.App.ListFragment
	{
		private FileListAdapter mFileListAdapter;
		private DirectoryInfo mDirectoryInfo;

		private void LogDebug(string msg)
		{
			Log.Debug(Context.PackageName, "FileListFragment: " + msg);
		}

		private void ToastShow(string str)
		{
			Toast.MakeText(Context, str, ToastLength.Short).Show();
			LogDebug(str);
		}

		public string DirPath
		{
			get { return mDirectoryInfo.FullName; }
			set { mDirectoryInfo = new DirectoryInfo(value); }
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			mFileListAdapter = new FileListAdapter(Activity, new FileSystemInfo[0]);
			ListAdapter = mFileListAdapter;
			DirPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			return base.OnCreateView(inflater, container, savedInstanceState);
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
			var fileSystemInfo = mFileListAdapter.GetItem(position);
			if (position == 0) {
				RefreshFilesList(Path.GetDirectoryName(fileSystemInfo.FullName));
			} else if (fileSystemInfo.IsDirectory()) {
				RefreshFilesList(fileSystemInfo.FullName);
			}
			base.OnListItemClick(l, v, position, id);
		}

		public void RefreshFilesList(string directory)
		{
			IList<FileSystemInfo> visibleThings = new List<FileSystemInfo>();
			try {
				var dir = new DirectoryInfo(directory);
				visibleThings.Add(dir);
				foreach (var item in dir.GetFileSystemInfos().Where(item => item.IsVisible())) {
					visibleThings.Add(item);
				}
				mDirectoryInfo = dir;
			} catch (Exception ex) {
				Log.Error("FileListFragment", "Couldn't access the directory " + mDirectoryInfo.FullName + "; " + ex);
				return;
			}
			mFileListAdapter.AddDirectoryContents(visibleThings);

			// If we don't do this, then the ListView will not update itself when then data set 
			// in the adapter changes. It will appear to the user that nothing has happened.
			ListView.RefreshDrawableState();

			Log.Verbose("FileListFragment", "Displaying the contents of directory {0}.", directory);
		}
	}
}
