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
		private ListView listView;
		private WeldCountAdapter weldCountAdapter;

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
		}

		public void Refresh(bool forced = false)
		{
			if (forced || dirPath != PrefPath || weldCountAdapter.Count == 0) {
				LogDebug("Refresh: " + dirPath + " : " + PrefPath + " : " + weldCountAdapter.Count.ToString());
				dirPath = PrefPath;
				try {
					var dir = new DirectoryInfo(dirPath);
					weldCountAdapter.Clear();
					foreach (var item in dir.GetFileSystemInfos()) {
						if (item.FullName.EndsWith(".JOB"))
							weldCountAdapter.Add(new JobFile(item.FullName));
					}
					weldCountAdapter.NotifyDataSetChanged();
				} catch {
				}
			}
		}

		public override void OnCreate(Bundle bundle)
		{
			LogDebug("OnCreate");
			base.OnCreate(bundle);

			dirPath = PrefPath;
			weldCountAdapter = new WeldCountAdapter(Context, Resource.Layout.WeldCountRow);
			try {
				var dir = new DirectoryInfo(PrefPath);
				foreach (var item in dir.GetFileSystemInfos()) {
					if (item.FullName.EndsWith(".JOB"))
						weldCountAdapter.Add(new JobFile(item.FullName));
				}
			} catch {
			}
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate(Resource.Layout.WeldCountTabFragment, container, false);

			var refresher = view.FindViewById<SwipeRefreshLayout>(Resource.Id.srl);
			if (refresher != null) {
				refresher.Refresh += delegate
				{
					Refresh(forced: true);
					refresher.Refreshing = false;
				};
			}

			int n = 0;
			listView = view.FindViewById<ListView>(Resource.Id.weldCountListView);
			listView.Adapter = weldCountAdapter;
			listView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
			{
				var jobFile = weldCountAdapter.GetItem(e.Position);
				jobFile.UpdateCN(n++);
				weldCountAdapter.NotifyDataSetChanged();
			};
			listView.ItemLongClick += (object sender, AdapterView.ItemLongClickEventArgs e) =>
			{
				var jobFile = weldCountAdapter.GetItem(e.Position);
				jobFile.LogRowString();
				jobFile.SaveFile();
            };

			return view;
		}
	}
}
