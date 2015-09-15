using Android.App;
using Android.Views;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.Design.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Android.Widget;

namespace NavigationViewApp
{
	[Activity(Label = "NavigationViewApp", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/MyTheme")]
    public class MainActivity : AppCompatActivity
	{
		Toolbar			toolbar;
		DrawerLayout	drawerLayout;
		NavigationView	navigationView;

		private void ToastShow(string str)
		{
			Toast
				.MakeText(this, str, ToastLength.Short)
				.Show();
			//Snackbar
			//	.Make(parentLayout, "Message sent", Snackbar.LengthLong)
			//	.SetAction("Undo", (view) => { /*Undo message sending here.*/ })
			//	.Show(); // Don’t forget to show!
		}

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			SetSupportActionBar(toolbar);
			//Enable support action bar to display hamburger
			SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu_white);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			SupportActionBar.Title = Resources.GetString(Resource.String.ApplicationName);
			//SupportActionBar.Subtitle = Resources.GetString(Resource.String.SubTitle);

			drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
			navigationView.NavigationItemSelected += (sender, e) => {
				e.MenuItem.SetChecked(true);
				//react to click here and swap fragments or navigate
				//drawerLayout.CloseDrawers();
			};
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.home, menu);
			return base.OnCreateOptionsMenu(menu);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId) {
				case Android.Resource.Id.Home:
				drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
				return true;
			}
			return base.OnOptionsItemSelected(item);
		}
	}
}

