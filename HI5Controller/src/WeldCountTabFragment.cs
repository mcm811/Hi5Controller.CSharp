using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.IO;
using Fragment = Android.Support.V4.App.Fragment;
using Android.Support.V4.Widget;

namespace Com.Changyoung.HI5Controller
{
	public class WeldCountTabFragment : Fragment
	{
		private View view;
		private ListView mListView;
		private WeldCountAdapter mWeldCountAdapter;

		private string dirPath;

		private void LogDebug(string msg)
		{
			Log.Debug(Context.PackageName, "WeldCountTabFragment: " + msg);
		}

		private void ToastShow(string str)
		{
			Toast.MakeText(Context, str, ToastLength.Short).Show();
			LogDebug(str);
		}

		public string PrefPath
		{
			get
			{
				try {
					using (var prefs = Application.Context.GetSharedPreferences(Context.PackageName, FileCreationMode.Private)) {
						return prefs.GetString("dirpath_file", Android.OS.Environment.ExternalStorageDirectory.AbsolutePath);
					}
				} catch {
					return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
				}
			}
			set
			{
				try {
					if (PrefPath != value) {
						using (var prefs = Application.Context.GetSharedPreferences(Context.PackageName, FileCreationMode.Private)) {
							var prefEditor = prefs.Edit();
							prefEditor.PutString("dirpath_file", value);
							prefEditor.Commit();
							ToastShow("경로 저장: " + value);
						}
					}
				} catch {
				}
			}
		}

		public void Refresh(bool forced = false)
		{
			if (forced || dirPath != PrefPath || mWeldCountAdapter.Count == 0) {
				LogDebug("Refresh: " + dirPath + " : " + PrefPath + " : " + mWeldCountAdapter.Count.ToString());
				dirPath = PrefPath;
				mWeldCountAdapter.Clear();
				try {
					var dir = new DirectoryInfo(dirPath);
					foreach (var item in dir.GetFileSystemInfos()) {
						if (item.FullName.EndsWith(".JOB")) {
							mWeldCountAdapter.Add(new JobFile(item.FullName));
						}
					}
				} catch {
				}
			}
		}

		public override void OnCreate(Bundle bundle)
		{
			LogDebug("OnCreate");
			base.OnCreate(bundle);

			dirPath = PrefPath;
			mWeldCountAdapter = new WeldCountAdapter(Context, Resource.Layout.WeldCountRow);
			try {
				var dir = new DirectoryInfo(PrefPath);
				foreach (var item in dir.GetFileSystemInfos()) {
					if (item.FullName.EndsWith(".JOB")) {
						mWeldCountAdapter.Add(new JobFile(item.FullName));
					}
				}
			} catch {
			}
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate(Resource.Layout.WeldCountTabFragment, container, false);

			mListView = view.FindViewById<ListView>(Resource.Id.weldCountListView);
			mListView.Adapter = mWeldCountAdapter;
			mListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
			{ };
			mListView.ItemLongClick += (object sender, AdapterView.ItemLongClickEventArgs e) =>
			{ };

			var refresher = view.FindViewById<SwipeRefreshLayout>(Resource.Id.srl);
			if (refresher != null) {
				refresher.Refresh += delegate
				{
					Refresh(forced: true);
					refresher.Refreshing = false;
				};
			}

			return view;
		}
	}
}
