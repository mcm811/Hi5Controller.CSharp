namespace com.xamarin.recipes.filepicker
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	using Android.Content;
	using Android.Views;
	using Android.Widget;
	using Com.Changyoung.HI5Controller;
	using Android.Util;
	using FloatingActionButton = Android.Support.Design.Widget.FloatingActionButton;

	public class FileListAdapter : ArrayAdapter<FileSystemInfo>
	{
		private readonly Context _context;

		public FileListAdapter(Context context, IList<FileSystemInfo> fsi)
			: base(context, Resource.Layout.file_list_item, Android.Resource.Id.Text1, fsi)
		{
			_context = context;
		}

		/// <summary>
		///   We provide this method to get around some of the
		/// </summary>
		/// <param name="directoryContents"> </param>
		public void AddDirectoryContents(IEnumerable<FileSystemInfo> directoryContents)
		{
			Clear();
			// Notify the _adapter that things have changed or that there is nothing 
			// to display.
			if (directoryContents.Any()) {
#if __ANDROID_11__
				// .AddAll was only introduced in API level 11 (Android 3.0). 
				// If the "Minimum Android to Target" is set to Android 3.0 or 
				// higher, then this code will be used.
				AddAll(directoryContents.ToArray());
#else
                // This is the code to use if the "Minimum Android to Target" is
                // set to a pre-Android 3.0 API (i.e. Android 2.3.3 or lower).
                lock (this)
                    foreach (var fsi in directoryContents)
                    {
                        Add(fsi);
                    }
#endif

				NotifyDataSetChanged();
			} else {
				NotifyDataSetInvalidated();
			}
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var fileSystemEntry = GetItem(position);

			FileListRowViewHolder viewHolder;
			View row;
			if (convertView == null) {
				row = _context.GetLayoutInflater().Inflate(Resource.Layout.file_list_item, parent, false);
				viewHolder = new FileListRowViewHolder(row.FindViewById<TextView>(Resource.Id.file_picker_time),
													   row.FindViewById<TextView>(Resource.Id.file_picker_text),
													   row.FindViewById<FloatingActionButton>(Resource.Id.file_picker_fab));
				row.Tag = viewHolder;
			} else {
				row = convertView;
				viewHolder = (FileListRowViewHolder)row.Tag;
			}
			if (position == 0) {
				if (fileSystemEntry.FullName == "/") {
					viewHolder.Update(null, ".", Resource.Drawable.ic_android);
				} else {
					var p = Path.GetFileName(Path.GetDirectoryName(fileSystemEntry.FullName));
					viewHolder.Update(null, Path.Combine(p, ".."), Resource.Drawable.ic_file_upload);
				}
			} else {
				viewHolder.Update(fileSystemEntry.LastWriteTime.ToString(), fileSystemEntry.Name, fileSystemEntry.IsDirectory() ? Resource.Drawable.ic_folder_open : Resource.Drawable.ic_description);
			}

			return row;
		}
	}
}