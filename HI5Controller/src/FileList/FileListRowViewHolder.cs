namespace com.xamarin.recipes.filepicker
{
	using Android.Widget;
	using FloatingActionButton = Android.Support.Design.Widget.FloatingActionButton;

	using Java.Lang;

	/// <summary>
	///   This class is used to hold references to the views contained in a list row.
	/// </summary>
	/// <remarks>
	///   This is an optimization so that we don't have to always look up the
	///   ImageView and the TextView for a given row in the ListView.
	/// </remarks>
	public class FileListRowViewHolder : Object
	{
		public FileListRowViewHolder(TextView timeTextView, TextView textView, FloatingActionButton fab)
		{
			TimeTextView = timeTextView;
			TextView = textView;
			Fab = fab;
		}

		public FloatingActionButton Fab { get; private set; }
		public TextView TextView { get; private set; }
		public TextView TimeTextView { get; private set; }

		/// <summary>
		///   This method will update the TextView and the ImageView that are
		///   are
		/// </summary>
		/// <param name="fileName"> </param>
		/// <param name="fileImageResourceId"> </param>
		public void Update(string fileTime, string fileName, int fileImageResourceId)
		{
			if (fileTime == null) {
				TimeTextView.Text = "";
				TimeTextView.Visibility = Android.Views.ViewStates.Gone;
			} else {
				TimeTextView.Text = fileTime;
				TimeTextView.Visibility = Android.Views.ViewStates.Visible;
			}
			TextView.Text = fileName;
			Fab.SetImageResource(fileImageResourceId);
		}
	}
}
