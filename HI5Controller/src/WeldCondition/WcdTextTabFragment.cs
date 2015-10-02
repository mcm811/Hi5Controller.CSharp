using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.IO;
using Android.Util;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using FloatingActionButton = Android.Support.Design.Widget.FloatingActionButton;
using System.Threading.Tasks;

namespace Com.Changyoung.HI5Controller
{
	public class WcdTextTabFragment : Android.Support.V4.App.Fragment, IRefresh
	{
		private View mView;
		private TextView mTvWcd;
		private FloatingActionButton mFab;	// 다시 읽어오기

		private string dirPath;
		private string robotPath;

		private void LogDebug(string msg)
		{
			Log.Debug(Context.PackageName, "WcdTextFragement: " + msg);
		}

		private void ToastShow(string str)
		{
			Toast.MakeText(Context, str, ToastLength.Short).Show();
			LogDebug(str);
		}

		private void SnackbarShow(View viewParent, string str)
		{
			Snackbar.Make(viewParent, str, Snackbar.LengthLong)
					.SetAction("Undo", (mView) => { /*Undo message sending here.*/ })
					.Show(); // Don’t forget to show!
		}

		async private Task<string> ReadFile(string fileName)
		{
			string st = "";
			try {
				using (StreamReader sr = new StreamReader(fileName, Encoding.GetEncoding("euc-kr"))) {
					st = await sr.ReadToEndAsync();
					sr.Close();
				}
			} catch {
				LogDebug("파일이 없습니다: " + fileName);
			}

			return st;
		}

		async private void ReadFile()
		{
			dirPath = Pref.Path;
			robotPath = Path.Combine(dirPath, "ROBOT.SWD");
			mTvWcd.Text = await ReadFile(robotPath);
		}

		public void Refresh(bool forced = false)
		{
			if (forced || dirPath != Pref.Path || mTvWcd.Text.Length == 0) {
				ReadFile();
			}
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			LogDebug("OnCreateView");
			mView = inflater.Inflate(Resource.Layout.wcd_text_tab_fragment, container, false);

			mTvWcd = mView.FindViewById<TextView>(Resource.Id.wcdTextView);

			// 떠 있는 액션버튼
			mFab = mView.FindViewById<FloatingActionButton>(Resource.Id.fab);
			//mFab.Elevation = 6;
			mFab.Click += (sender, e) =>
			{
				Refresh(forced: true);
			};

			var refresher = mView.FindViewById<SwipeRefreshLayout>(Resource.Id.srl);
			if (refresher != null) {
				refresher.Refresh += delegate
				{
					Refresh(forced: true);
					refresher.Refreshing = false;
				};
			}

			return mView;
		}
	}
}