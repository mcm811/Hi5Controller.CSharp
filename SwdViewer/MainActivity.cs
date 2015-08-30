using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;

namespace SwdViewer
{
    [Activity(Label = "SwdViewer", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.NoTitle);
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            //this.Window.ClearFlags(WindowManagerFlags.Fullscreen);

            // Set our view from the "main" layout resource
            //SetContentView(Resource.Layout.Main);
            //SetContentView(Resource.Layout.TestTableLayout);

            TableLayout table = new TableLayout(this);
            table.StretchAllColumns = true;
            table.ShrinkAllColumns = true;

            TableRow rowTitle = new TableRow(this);
            rowTitle.SetGravity(GravityFlags.CenterHorizontal);

            TableRow rowDayLables = new TableRow(this);
            TableRow rowHighs = new TableRow(this);
            TableRow rowLows = new TableRow(this);
            //TableRow rowConditions = new TableRow(this);
            //rowConditions.SetGravity(GravityFlags.Center);

            EditText empty = new EditText(this);

            // title column/row
            EditText title = new EditText(this);
            title.SetText("Java Weather Table", EditText.BufferType.Editable);
            title.TextSize = 18;
            title.Gravity = GravityFlags.Center;
            title.SetTypeface(Typeface.Serif, TypefaceStyle.Bold);

            TableRow.LayoutParams layoutParams = new TableRow.LayoutParams();
            layoutParams.Span = 4;

            rowTitle.AddView(title, layoutParams);

            // labels column
            EditText highsLabel = new EditText(this);
            highsLabel.SetText("Day High", EditText.BufferType.Editable);
            highsLabel.Typeface = Typeface.DefaultBold;

            EditText lowsLabel = new EditText(this);
            lowsLabel.SetText("Day Low", EditText.BufferType.Editable);
            lowsLabel.Typeface = Typeface.DefaultBold;

            //EditText conditionsLabel = new EditText(this);
            //conditionsLabel.Text = "Conditions";
            //conditionsLabel.Typeface = Typeface.DefaultBold;
            
            rowDayLables.AddView(empty);
            rowHighs.AddView(highsLabel);
            rowLows.AddView(lowsLabel);
            //rowConditions.AddView(conditionsLabel);

            // day 1 column
            EditText day1Label = new EditText(this);
            day1Label.SetText("Feb 7", EditText.BufferType.Editable);
            day1Label.SetTypeface(Typeface.Serif, TypefaceStyle.Bold);

            EditText day1High = new EditText(this);
            day1High.SetText("28°F", EditText.BufferType.Editable);
            day1High.Gravity = GravityFlags.CenterHorizontal;

            EditText day1Low = new EditText(this);
            day1Low.SetText("15°F", EditText.BufferType.Editable);
            day1Low.Gravity = GravityFlags.CenterHorizontal;

            //ImageView day1Conditions = new ImageView(this);
            //day1Conditions.SetImageResource(Resource.Drawable.hot);

            rowDayLables.AddView(day1Label);
            rowHighs.AddView(day1High);
            rowLows.AddView(day1Low);
            //rowConditions.AddView(day1Conditions);

            // day2 column
            EditText day2Label = new EditText(this);
            day2Label.SetText("Feb 8", EditText.BufferType.Editable);
            day2Label.SetTypeface(Typeface.Serif, TypefaceStyle.Bold);

            EditText day2High = new EditText(this);
            day2High.SetText("26°F", EditText.BufferType.Editable);
            day2High.Gravity = GravityFlags.CenterHorizontal;

            EditText day2Low = new EditText(this);
            day2Low.SetText("14°F", EditText.BufferType.Editable);
            day2Low.Gravity = GravityFlags.CenterHorizontal;

            //ImageView day2Conditions = new ImageView(this);
            //day2Conditions.SetImageResource(Resource.Drawable.pt_cloud);

            rowDayLables.AddView(day2Label);
            rowHighs.AddView(day2High);
            rowLows.AddView(day2Low);
            //rowConditions.AddView(day2Conditions);

            // day3 column
            EditText day3Label = new EditText(this);
            day3Label.SetText("Feb 9", EditText.BufferType.Editable);
            day3Label.SetTypeface(Typeface.Serif, TypefaceStyle.Bold);

            EditText day3High = new EditText(this);
            day3High.SetText("23°F", EditText.BufferType.Editable);
            day3High.Gravity = GravityFlags.CenterHorizontal;
            EditText day3Low = new EditText(this);
            day3Low.SetText("3°F", EditText.BufferType.Editable);
            day3Low.Gravity = GravityFlags.CenterHorizontal;

            //ImageView day3Conditions = new ImageView(this);
            //day3Conditions.SetImageResource(Resource.Drawable.snow);

            rowDayLables.AddView(day3Label);
            rowHighs.AddView(day3High);
            rowLows.AddView(day3Low);
            //rowConditions.AddView(day3Conditions);

            // day4 column
            EditText day4Label = new EditText(this);
            day4Label.SetText("Feb 10", EditText.BufferType.Editable);
            day4Label.SetTypeface(Typeface.Serif, TypefaceStyle.Bold);

            EditText day4High = new EditText(this);
            day4High.SetText("17°F", EditText.BufferType.Editable);
            day4High.Gravity = GravityFlags.CenterHorizontal;

            EditText day4Low = new EditText(this);
            day4Low.SetText("5°F", EditText.BufferType.Editable);
            day4Low.Gravity = GravityFlags.CenterHorizontal;

            //ImageView day4Conditions = new ImageView(this);
            //day4Conditions.SetImageResource(Resource.Drawable.lt_snow);

            rowDayLables.AddView(day4Label);
            rowHighs.AddView(day4High);
            rowLows.AddView(day4Low);
            //rowConditions.AddView(day4Conditions);

            // day5 column
            EditText day5Label = new EditText(this);
            day5Label.SetText("Feb 11", EditText.BufferType.Editable);
            day5Label.SetTypeface(Typeface.Serif, TypefaceStyle.Bold);

            EditText day5High = new EditText(this);
            day5High.SetText("19°F", EditText.BufferType.Editable);
            day5High.Gravity = GravityFlags.CenterHorizontal;

            EditText day5Low = new EditText(this);
            day5Low.SetText("6°F", EditText.BufferType.Editable);
            day5Low.Gravity = GravityFlags.CenterHorizontal;

            //ImageView day5Conditions = new ImageView(this);
            //day5Conditions.SetImageResource(Resource.Drawable.pt_sun);

            rowDayLables.AddView(day5Label);
            rowHighs.AddView(day5High);
            rowLows.AddView(day5Low);
            //rowConditions.AddView(day5Conditions);

            table.AddView(rowTitle);
            table.AddView(rowDayLables);
            table.AddView(rowHighs);
            table.AddView(rowLows);
            //table.AddView(rowConditions);

            SetContentView(table);
        }
    }
}
