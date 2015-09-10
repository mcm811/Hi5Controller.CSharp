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

	[Activity(Label = "@string/app_name", MainLauncher = false, Icon = "@drawable/folder")]
	public class FilePickerActivity : FragmentActivity
	{
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
		}

		public override void OnBackPressed()
		{
			base.OnBackPressed();
			//Toast.MakeText(this, WcdActivity.path, ToastLength.Short).Show();
			Intent intent = new Intent(this, typeof(FilePickerActivity));
			intent.PutExtra("dir_path", WcdActivity.path);
			SetResult(Result.Ok, intent);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (WcdActivity.path != null) {
				//using (var sw = new StreamWriter(OpenFileOutput("dirpath_file", FileCreationMode.Private))) {
				//	sw.Write(WcdActivity.path);
				//	sw.Close();
				//}
				using (var prefs = Application.Context.GetSharedPreferences(Application.PackageName, FileCreationMode.Private)) {
					var prefEditor = prefs.Edit();
					prefEditor.PutString("dirpath_file", WcdActivity.path);
					prefEditor.Commit();
				}
			}
		}
	}
}
