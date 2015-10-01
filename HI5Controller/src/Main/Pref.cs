
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

		public static string Path
		{
			get
			{
				try {
					using (var prefs = Application.Context.GetSharedPreferences(PackageName, FileCreationMode.Private)) {
						return prefs.GetString("dirpath_file", StoragePath);
					}
				} catch {
					return StoragePath;
				}
			}
			set
			{
				try {
					if (value != null && value != Path) {
						using (var prefs = Application.Context.GetSharedPreferences(PackageName, FileCreationMode.Private)) {
							var prefEditor = prefs.Edit();
							prefEditor.PutString("dirpath_file", value);
							prefEditor.Commit();
						}
					}
				} catch {
				}
			}
		}
	}
}