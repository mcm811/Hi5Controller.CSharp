namespace com.xamarin.recipes.filepicker
{
	using Android.App;
	using Android.Content;
	using Android.OS;
	using Android.Support.V4.App;
	using Android.Views;
	using Android.Widget;
	using HI5Controller;
	using System.IO;
	using System.Text;

	[Activity(Label = "@string/app_name", MainLauncher = false, Icon = "@drawable/folder", Theme = "@style/MyTheme")]
	public class FilePickerActivity : FragmentActivity
	{
		private FileListFragment fileList;
		private Button button;

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

			button = FindViewById<Button>(Resource.Id.btnFolderSelect);
			button.Click += (object sender, System.EventArgs e) =>
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

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}
	}
}
