using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Com.Changmin.HI5Controller;
using System;
using System.IO;
using System.Text;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using DialogFragment = Android.Support.V4.App.DialogFragment;
using FloatingActionButton = Android.Support.Design.Widget.FloatingActionButton;

namespace com.xamarin.recipes.filepicker
{
	public class OnFilePickEventArgs : EventArgs
	{
		private string dirPath;

		public string DirPath
		{
			get { return dirPath; }
			set { dirPath = value; }
		}

		public OnFilePickEventArgs(string path)
		{
			DirPath = path;
		}
	}

	public class FilePickerDialog : DialogFragment
	{
		private FloatingActionButton fab;

		private string dirPath;

		public event EventHandler<OnFilePickEventArgs> onFIlePickComplete;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.FIlePicker, container, false);

			//var fileList = (FileListFragment)SupportFragmentManager.FindFragmentById(Resource.Id.file_list_fragment);
			//fileList = (FileListFragment)view.FindFragmentById(Resource.Id.file_list_fragment);
			//fileList.DirPath = Intent.GetStringExtra("dir_path");

			fab = view.FindViewById<FloatingActionButton>(Resource.Id.fab);
			fab.Click += (object sender, System.EventArgs e) =>
			{
				dirPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                onFIlePickComplete.Invoke(this, new OnFilePickEventArgs(dirPath));
				this.Dismiss();
			};

			return view;
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			Dialog.Window.RequestFeature(WindowFeatures.NoTitle); // Set the title bar to invisible
			base.OnActivityCreated(savedInstanceState);
			//Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation;
		}
	}
}
