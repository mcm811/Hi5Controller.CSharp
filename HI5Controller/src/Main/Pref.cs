
using Android.App;
using Android.Content;
using Android.Util;
using Android.Widget;
using System.IO;
using System.Text;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace Com.Changyoung.HI5Controller
{
	public class Pref
	{
		//private readonly static string PackageName = Application.PackageName; // Context.PackageName
		//public readonly static string StoragePath = "/storage";
		public readonly static string PackageName = "Com.Changyoung.HI5Conroller.App";
		private readonly static string StoragePath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

		public readonly static string WorkPathKey = "work_path";
		public readonly static string BackupPathKey = "backup_path";

		public static string WorkPath
		{
			get { return GetPath(WorkPathKey); }
			set { SetPath(WorkPathKey, value); }
		}

		public static string BackupPath
		{
			get {
				return Path.Combine(Path.GetFullPath(WorkPath), "Backup");
			}
			set { SetPath(BackupPathKey, value); }
		}

		public static string GetPath(string key)
		{
			try {
				using (var prefs = Application.Context.GetSharedPreferences(PackageName, FileCreationMode.Private)) {
					return prefs.GetString(key, StoragePath);
				}
			} catch {
				return StoragePath;
			}
		}

		public static void SetPath(string key, string value)
		{
			try {
				if (key != null && value != null) {
					using (var prefs = Application.Context.GetSharedPreferences(PackageName, FileCreationMode.Private)) {
						var prefEditor = prefs.Edit();
						prefEditor.PutString(key, value);
						prefEditor.Commit();
					}
				}
			} catch { }
		}

		public static void TextViewDialog(Context context, string path, string text = null)
		{
			var textView = new TextView(context);
			textView.SetPadding(10, 10, 10, 10);
			textView.SetTextSize(ComplexUnitType.Sp, 10f);
			var scrollView = new ScrollView(context);
			scrollView.AddView(textView);
			AlertDialog.Builder dialog = new AlertDialog.Builder(context);
			dialog.SetView(scrollView);

			if (text == null) {
				try {
					using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding("euc-kr"))) {
						textView.Text = sr.ReadToEnd();
						sr.Close();
					}
				} catch { }
			} else {
				textView.Text = text;
			}

			dialog.SetPositiveButton("닫기", delegate
			{ });

			dialog.Show();
		}
	}
}