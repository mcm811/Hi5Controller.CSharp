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
	using Com.Changmin.HI5Controller;

	/// <summary>
	///   A ListFragment that will show the files and subdirectories of a given directory.
	/// </summary>
	/// <remarks>
	///   <para> This was placed into a ListFragment to make this easier to share this functionality with with tablets. </para>
	///   <para> Note that this is a incomplete example. It lacks things such as the ability to go back up the directory tree, or any special handling of a file when it is selected. </para>
	/// </remarks>
	public class FileListFragment : ListFragment
	{
		private FileListAdapter _adapter;
		private DirectoryInfo _directory;

		public string DirPath
		{
			get { return _directory.FullName; }
			set { _directory = new DirectoryInfo(value); }
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			_adapter = new FileListAdapter(Activity, new FileSystemInfo[0]);
			ListAdapter = _adapter;
			//DirPath = Arguments.GetString("dir_path");
			//DirPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).ToString();
			DirPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
		}

		public override void OnListItemClick(ListView l, View v, int position, long id)
		{
			var fileSystemInfo = _adapter.GetItem(position);
			if (position == 0) {
				RefreshFilesList(Path.GetDirectoryName(fileSystemInfo.FullName));
			} else if (fileSystemInfo.IsDirectory()) {
				RefreshFilesList(fileSystemInfo.FullName);
			}
			base.OnListItemClick(l, v, position, id);
		}

		public override void OnResume()
		{
			base.OnResume();

			RefreshFilesList(DirPath);
		}

		public void RefreshFilesList(string directory)
		{
			IList<FileSystemInfo> visibleThings = new List<FileSystemInfo>();
			try {
				//visibleThings.Add(new DirectoryInfo(directory == "/" ? "/" : Path.GetDirectoryName(directory)));
				var dir = new DirectoryInfo(directory);
				visibleThings.Add(dir);
				foreach (var item in dir.GetFileSystemInfos().Where(item => item.IsVisible())) {
					visibleThings.Add(item);
				}
				_directory = dir;
			} catch (Exception ex) {
				Log.Error("FileListFragment", "Couldn't access the directory " + _directory.FullName + "; " + ex);
				//Toast.MakeText(Activity, "Problem retrieving contents of " + directory, ToastLength.Long).Show();
				return;
			}
			_adapter.AddDirectoryContents(visibleThings);

			// If we don't do this, then the ListView will not update itself when then data set 
			// in the adapter changes. It will appear to the user that nothing has happened.
			ListView.RefreshDrawableState();

			Log.Verbose("FileListFragment", "Displaying the contents of directory {0}.", directory);
		}
	}
}
