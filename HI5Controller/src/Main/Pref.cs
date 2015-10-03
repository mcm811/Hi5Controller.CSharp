
using Android.App;
using Android.Content;

namespace Com.Changyoung.HI5Controller
{
	public class Pref
	{
		//private readonly static string pkgName = Application.PackageName; // Context.PackageName
		//private readonly static string storagePath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
		public readonly static string PackageName = "Com.Changyoung.HI5Conroller.App";
		public readonly static string StoragePath = "/storage";

		public readonly static string WorkPathKey = "work_path";
		public readonly static string BackupPathKey = "backup_path";

		public static string WorkPath
		{
			get { return GetPath(WorkPathKey); }
			set { SetPath(WorkPathKey, value); }
		}

		public static string BackupPath
		{
			get { return GetPath(BackupPathKey); }
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
				if (value != null && value != BackupPath) {
					using (var prefs = Application.Context.GetSharedPreferences(PackageName, FileCreationMode.Private)) {
						var prefEditor = prefs.Edit();
						prefEditor.PutString(key, value);
						prefEditor.Commit();
					}
				}
			} catch { }
		}
	}
}