using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.IO;
using System.Text;
using Android.Support.V4.Widget;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using DialogFragment = Android.Support.V4.App.DialogFragment;
using FloatingActionButton = Android.Support.Design.Widget.FloatingActionButton;
using Com.Changmin.HI5Controller;

namespace com.xamarin.recipes.filepicker
{
	[Activity(Label = "@string/app_name", MainLauncher = false, Icon = "@drawable/folder", Theme = "@style/MyTheme")]
	public class FilePickerActivity : FragmentActivity
	{
		private FloatingActionButton fab;
		private FileListFragment fileList;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.FIlePicker);
			//View FilePickerView = LayoutInflater.From(this).Inflate(Resource.Layout.FIlePicker, null);
			//AlertDialog.Builder filePickerDialog = new AlertDialog.Builder(this);
			//filePickerDialog.SetView(FilePickerView);
			//filePickerDialog.SetNegativeButton("취소", delegate
			//{
			//});
			//filePickerDialog.SetPositiveButton("확인", delegate
			//{
			//});
			//filePickerDialog.Show();

			fileList = (FileListFragment)SupportFragmentManager.FindFragmentById(Resource.Id.file_list_fragment);
			fileList.DirPath = Intent.GetStringExtra("dir_path");

			// 떠 있는 액션버튼
			fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
			fab.Click += (sender, e) =>
			{
				Intent intent = new Intent(this, typeof(FilePickerActivity));
				intent.PutExtra("dir_path", fileList.DirPath);
				SetResult(Result.Ok, intent);
				Finish();
			};
		}

		public override void OnBackPressed()
		{
			if (fileList.DirPath != "/")
				fileList.RefreshFilesList(Path.GetDirectoryName(fileList.DirPath));
			else
				base.OnBackPressed();
		}

		protected override void OnStop()
		{
			Finish();
			base.OnStop();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}
	}
}
