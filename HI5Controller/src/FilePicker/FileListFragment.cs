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
	using Android;
	using AlertDialog = Android.Support.V7.App.AlertDialog;
	using System.Text;

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

		public string Key { get; set; }

		private void LogDebug(string msg)
		{
			Log.Debug(Context.PackageName, "FileListFragment: " + msg);
		}

		private void ToastShow(string str)
		{
			Snackbar.Make(view, str, Snackbar.LengthLong).Show();
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
				var textView = new TextView(Context);
				textView.SetPadding(10, 10, 10, 10);
				textView.SetTextSize(ComplexUnitType.Sp, 10f);
				var scrollView = new ScrollView(Context);
				scrollView.AddView(textView);
				AlertDialog.Builder dialog = new AlertDialog.Builder(Context);
				dialog.SetView(scrollView);

				try {
					using (StreamReader sr = new StreamReader(fileSystemInfo.FullName, Encoding.GetEncoding("euc-kr"))) {
						textView.Text = sr.ReadToEnd();
						sr.Close();
					}
				} catch { }

				dialog.SetPositiveButton("닫기", delegate
				{ });

				dialog.Show();
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
				directoryInfo = dir;
				if (Key != null)
					Com.Changyoung.HI5Controller.Pref.SetPath(Key, dir.FullName);
			} catch (Exception ex) {
				Log.Error("FileListFragment", "Couldn't access the directory " + directoryInfo.FullName + "; " + ex);
				return;
			}
			fileListAdapter.AddDirectoryContents(visibleThings);

			// If we don't do this, then the ListView will not update itself when then data set 
			// in the adapter changes. It will appear to the user that nothing has happened.
			ListView.RefreshDrawableState();

			Log.Verbose("FileListFragment", "Displaying the contents of directory {0}.", directory);
		}
	}
}
