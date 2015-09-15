using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using HI5Controller;
using System;
using System.IO;
using System.Text;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using DialogFragment = Android.Support.V4.App.DialogFragment;

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

		public OnFilePickEventArgs(string dirPath)
		{
			DirPath = dirPath;
		}
	}

	public class FilePickerDialog : DialogFragment
	{
		private FileListFragment fileList;
		private Button button;
		private string dirPath;

		public event EventHandler<OnFilePickEventArgs> onFIlePickComplete;


		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.FIlePicker, container, false);	

			//fileList = (FileListFragment)SupportFragmentManager.FindFragmentById(Resource.Id.file_list_fragment);
			//fileList.DirPath = Intent.GetStringExtra("dir_path");

			button = view.FindViewById<Button>(Resource.Id.btnFolderSelect);
			button.Click += (object sender, System.EventArgs e) =>
			{
				//Intent intent = new Intent(this, typeof(FilePickerDialog));
				//intent.PutExtra("dir_path", fileList.DirPath);
				//SetResult(Result.Ok, intent);
				//Finish();
				onFIlePickComplete.Invoke(this, new OnFilePickEventArgs(dirPath));
				this.Dismiss();
			};

			return view;
		}

		//public override void OnBackPressed()
		//{
		//	if (fileList.DirPath != "/")
		//		fileList.RefreshFilesList(Path.GetDirectoryName(fileList.DirPath));
		//	else
		//		base.OnBackPressed();
		//}
		//
		//protected override void OnStop()
		//{
		//	Finish();
		//	base.OnStop();
		//}
		//
		//protected override void OnDestroy()
		//{
		//	base.OnDestroy();
		//}
	}
}
