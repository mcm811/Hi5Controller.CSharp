﻿using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Android.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SwdEditor
{
	[Activity(Label = "SWD 편집기", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		static readonly List<string> weldConditionData = new List<string>()	{
			"	- 1=1,1,280,10.0,4.0,0.0,0.0",
			"	- 2=2,1,280,10.0,4.0,0.0,0.0",
			"	- 3=3,1,280,10.0,4.0,0.0,0.0",
			"	- 4=4,1,280,10.0,4.0,0.0,0.0",
			"	- 5=5,1,280,10.0,4.0,0.0,0.0",
			"	- 6=6,1,300,10.0,4.0,0.0,0.0",
			"	- 7=7,1,280,10.0,4.0,0.0,0.0",
			"	- 8=8,1,280,10.0,4.0,0.0,0.0",
			"	- 9=9,1,280,10.0,4.0,0.0,0.0",
			"	-10=10,1,280,10.0,4.0,0.0,0.0",
			"	-11=11,1,280,10.0,4.0,0.0,0.0",
			"	-12=12,1,280,10.0,4.0,0.0,0.0",
			"	-13=13,1,280,10.0,4.0,0.0,0.0",
			"	-14=14,1,280,10.0,4.0,0.0,0.0",
			"	-15=15,1,280,10.0,4.0,0.0,0.0",
			"	-16=16,1,250,10.0,4.0,0.0,0.0",
			"	-17=17,1,250,10.0,4.0,0.0,0.0",
			"	-18=18,1,250,10.0,4.0,0.0,0.0",
			"	-19=19,1,250,10.0,4.0,0.0,0.0",
			"	-20=20,1,250,10.0,4.0,0.0,0.0",
			"	-21=21,1,250,10.0,4.0,0.0,0.0",
			"	-22=22,1,250,10.0,4.0,0.0,0.0",
			"	-23=23,1,250,10.0,4.0,0.0,0.0",
			"	-24=24,1,250,10.0,4.0,0.0,0.0",
			"	-25=25,1,250,10.0,4.0,0.0,0.0",
			"	-26=26,1,280,10.0,4.0,0.0,0.0",
			"	-27=27,1,280,10.0,4.0,0.0,0.0",
			"	-28=28,1,280,10.0,4.0,0.0,0.0",
			"	-29=29,1,280,10.0,4.0,0.0,0.0",
			"	-30=30,1,280,10.0,4.0,0.0,0.0",
			"	-31=31,1,280,10.0,4.0,0.0,0.0",
			"	-32=32,1,280,10.0,4.0,0.0,0.0",
			"	-33=33,1,280,10.0,4.0,0.0,0.0",
			"	-34=34,1,280,10.0,4.0,0.0,0.0",
			"	-35=35,1,280,10.0,4.0,0.0,0.0",
			"	-36=36,1,280,10.0,4.0,0.0,0.0",
			"	-37=37,1,250,10.0,4.0,0.0,0.0",
			"	-38=38,1,250,10.0,4.0,0.0,0.0",
			"	-39=39,1,250,10.0,4.0,0.0,0.0",
			"	-40=40,1,250,10.0,4.0,0.0,0.0",
			"	-41=41,1,250,10.0,4.0,0.0,0.0",
			"	-42=42,1,250,10.0,4.0,0.0,0.0",
			"	-43=43,1,250,10.0,4.0,0.0,0.0",
			"	-44=44,1,250,10.0,4.0,0.0,0.0",
			"	-45=45,1,250,10.0,4.0,0.0,0.0",
			"	-46=46,1,250,10.0,4.0,0.0,0.0",
			"	-47=47,1,250,10.0,4.0,0.0,0.0",
			"	-48=48,1,250,10.0,4.0,0.0,0.0",
			"	-49=49,1,250,10.0,4.0,0.0,0.0",
			"	-50=50,1,250,10.0,4.0,0.0,0.0",
			"	-51=51,1,250,10.0,4.0,0.0,0.0",
			"	-52=52,1,250,10.0,4.0,0.0,0.0",
			"	-53=53,1,250,10.0,4.0,0.0,0.0",
			"	-54=54,1,250,10.0,4.0,0.0,0.0",
			"	-55=55,1,250,10.0,4.0,0.0,0.0",
			"	-56=56,1,250,10.0,4.0,0.0,0.0",
			"	-57=57,1,250,10.0,4.0,0.0,0.0",
			"	-58=58,1,250,10.0,4.0,0.0,0.0",
			"	-59=59,1,250,10.0,4.0,0.0,0.0",
			"	-60=60,1,250,10.0,4.0,0.0,0.0",
			"	-61=61,1,250,10.0,4.0,0.0,0.0",
			"	-62=62,1,250,10.0,4.0,0.0,0.0",
			"	-63=63,1,250,10.0,4.0,0.0,0.0",
			"	-64=64,1,250,10.0,4.0,0.0,0.0",
			"	-65=65,1,250,10.0,4.0,0.0,0.0",
			"	-66=66,1,250,10.0,4.0,0.0,0.0",
			"	-67=67,1,250,10.0,4.0,0.0,0.0",
			"	-68=68,1,250,10.0,4.0,0.0,0.0",
			"	-69=69,1,250,10.0,4.0,0.0,0.0",
			"	-70=70,1,250,10.0,4.0,0.0,0.0",
			"	-71=71,1,250,10.0,4.0,0.0,0.0",
			"	-72=72,1,250,10.0,4.0,0.0,0.0",
			"	-73=73,1,250,10.0,4.0,0.0,0.0",
			"	-74=74,1,250,10.0,4.0,0.0,0.0",
			"	-75=75,1,250,10.0,4.0,0.0,0.0",
			"	-76=76,1,250,10.0,4.0,0.0,0.0",
			"	-77=77,1,250,10.0,4.0,0.0,0.0",
			"	-78=78,1,250,10.0,4.0,0.0,0.0",
			"	-79=79,1,250,10.0,4.0,0.0,0.0",
			"	-80=80,1,250,10.0,4.0,0.0,0.0",
			"	-81=81,1,250,10.0,4.0,0.0,0.0",
			"	-82=82,1,250,10.0,4.0,0.0,0.0",
			"	-83=83,1,250,10.0,4.0,0.0,0.0",
			"	-84=84,1,250,10.0,4.0,0.0,0.0",
			"	-85=85,1,250,10.0,4.0,0.0,0.0",
			"	-86=86,1,250,10.0,4.0,0.0,0.0",
			"	-87=87,1,250,10.0,4.0,0.0,0.0",
			"	-88=88,1,250,10.0,4.0,0.0,0.0",
			"	-89=89,1,250,10.0,4.0,0.0,0.0",
			"	-90=90,1,250,10.0,4.0,0.0,0.0",
			"	-91=91,1,250,10.0,4.0,0.0,0.0",
			"	-92=92,1,250,10.0,4.0,0.0,0.0",
			"	-93=93,1,250,10.0,4.0,0.0,0.0",
			"	-94=94,1,250,10.0,4.0,0.0,0.0",
			"	-95=95,1,250,10.0,4.0,0.0,0.0",
			"	-96=96,1,250,10.0,4.0,0.0,0.0",
			"	-97=97,1,250,10.0,4.0,0.0,0.0",
			"	-98=98,1,250,10.0,4.0,0.0,0.0",
			"	-99=99,1,250,10.0,4.0,0.0,0.0",
			"	-100=100,1,250,10.0,4.0,0.0,0.0",
			"	-101=101,1,250,10.0,4.0,0.0,0.0",
			"	-102=102,1,250,10.0,4.0,0.0,0.0",
			"	-103=103,1,250,10.0,4.0,0.0,0.0",
			"	-104=104,1,250,10.0,4.0,0.0,0.0",
			"	-105=105,1,250,10.0,4.0,0.0,0.0",
			"	-106=106,1,300,10.0,4.0,0.0,0.0",
			"	-107=107,1,300,10.0,4.0,0.0,0.0",
			"	-108=108,1,250,10.0,4.0,0.0,0.0",
			"	-109=109,1,250,10.0,4.0,0.0,0.0",
			"	-110=110,1,250,10.0,4.0,0.0,0.0",
			"	-111=111,1,250,10.0,4.0,0.0,0.0",
			"	-112=112,1,250,10.0,4.0,0.0,0.0",
			"	-113=113,1,250,10.0,4.0,0.0,0.0",
			"	-114=114,1,250,10.0,4.0,0.0,0.0",
			"	-115=115,1,250,10.0,4.0,0.0,0.0",
			"	-116=116,1,250,10.0,4.0,0.0,0.0",
			"	-117=117,1,250,10.0,4.0,0.0,0.0",
			"	-118=118,1,250,10.0,4.0,0.0,0.0",
			"	-119=119,1,250,10.0,4.0,0.0,0.0",
			"	-120=120,1,250,10.0,4.0,0.0,0.0",
			"	-121=121,1,250,10.0,4.0,0.0,0.0",
			"	-122=122,1,250,10.0,4.0,0.0,0.0",
			"	-123=123,1,250,10.0,4.0,0.0,0.0",
			"	-124=124,1,250,10.0,4.0,0.0,0.0",
			"	-125=125,1,250,10.0,4.0,0.0,0.0",
			"	-126=126,1,250,10.0,4.0,0.0,0.0",
			"	-127=127,1,250,10.0,4.0,0.0,0.0",
			"	-128=128,1,250,10.0,4.0,0.0,0.0",
			"	-129=129,1,250,10.0,4.0,0.0,0.0",
			"	-130=130,1,250,10.0,4.0,0.0,0.0",
			"	-131=131,1,250,10.0,4.0,0.0,0.0",
			"	-132=132,1,250,10.0,4.0,0.0,0.0",
			"	-133=133,1,250,10.0,4.0,0.0,0.0",
			"	-134=134,1,250,10.0,4.0,0.0,0.0",
			"	-135=135,1,250,10.0,4.0,0.0,0.0",
			"	-136=136,1,250,10.0,4.0,0.0,0.0",
			"	-137=137,1,250,10.0,4.0,0.0,0.0",
			"	-138=138,1,250,10.0,4.0,0.0,0.0",
			"	-139=139,1,250,10.0,4.0,0.0,0.0",
			"	-140=140,1,250,10.0,4.0,0.0,0.0",
			"	-141=141,1,250,10.0,4.0,0.0,0.0",
			"	-142=142,1,250,10.0,4.0,0.0,0.0",
			"	-143=143,1,250,10.0,4.0,0.0,0.0",
			"	-144=144,1,250,10.0,4.0,0.0,0.0",
			"	-145=145,1,250,10.0,4.0,0.0,0.0",
			"	-146=146,1,250,10.0,4.0,0.0,0.0",
			"	-147=147,1,250,10.0,4.0,0.0,0.0",
			"	-148=148,1,250,10.0,4.0,0.0,0.0",
			"	-149=149,1,250,10.0,4.0,0.0,0.0",
			"	-150=150,1,200,10.0,4.0,0.0,0.0",
			"	-151=151,1,100,10.0,4.0,0.0,0.0",
			"	-152=152,1,100,10.0,4.0,0.0,0.0",
			"	-153=153,1,100,10.0,4.0,0.0,0.0",
			"	-154=154,1,100,10.0,4.0,0.0,0.0",
			"	-155=155,1,100,10.0,4.0,0.0,0.0",
			"	-156=156,1,100,10.0,4.0,0.0,0.0",
			"	-157=157,1,100,10.0,4.0,0.0,0.0",
			"	-158=158,1,100,10.0,4.0,0.0,0.0",
			"	-159=159,1,100,10.0,4.0,0.0,0.0",
			"	-160=160,1,100,10.0,4.0,0.0,0.0",
			"	-161=161,1,100,10.0,4.0,0.0,0.0",
			"	-162=162,1,100,10.0,4.0,0.0,0.0",
			"	-163=163,1,100,10.0,4.0,0.0,0.0",
			"	-164=164,1,100,10.0,4.0,0.0,0.0",
			"	-165=165,1,100,10.0,4.0,0.0,0.0",
			"	-166=166,1,100,10.0,4.0,0.0,0.0",
			"	-167=167,1,100,10.0,4.0,0.0,0.0",
			"	-168=168,1,100,10.0,4.0,0.0,0.0",
			"	-169=169,1,100,10.0,4.0,0.0,0.0",
			"	-170=170,1,100,10.0,4.0,0.0,0.0",
			"	-171=171,1,100,10.0,4.0,0.0,0.0",
			"	-172=172,1,100,10.0,4.0,0.0,0.0",
			"	-173=173,1,100,10.0,4.0,0.0,0.0",
			"	-174=174,1,100,10.0,4.0,0.0,0.0",
			"	-175=175,1,100,10.0,4.0,0.0,0.0",
			"	-176=176,1,100,10.0,4.0,0.0,0.0",
			"	-177=177,1,100,10.0,4.0,0.0,0.0",
			"	-178=178,1,100,10.0,4.0,0.0,0.0",
			"	-179=179,1,100,10.0,4.0,0.0,0.0",
			"	-180=180,1,100,10.0,4.0,0.0,0.0",
			"	-181=181,1,100,10.0,4.0,0.0,0.0",
			"	-182=182,1,100,10.0,4.0,0.0,0.0",
			"	-183=183,1,100,10.0,4.0,0.0,0.0",
			"	-184=184,1,100,10.0,4.0,0.0,0.0",
			"	-185=185,1,100,10.0,4.0,0.0,0.0",
			"	-186=186,1,100,10.0,4.0,0.0,0.0",
			"	-187=187,1,100,10.0,4.0,0.0,0.0",
			"	-188=188,1,100,10.0,4.0,0.0,0.0",
			"	-189=189,1,100,10.0,4.0,0.0,0.0",
			"	-190=190,1,100,10.0,4.0,0.0,0.0",
			"	-191=191,1,100,10.0,4.0,0.0,0.0",
			"	-192=192,1,100,10.0,4.0,0.0,0.0",
			"	-193=193,1,100,10.0,4.0,0.0,0.0",
			"	-194=194,1,100,10.0,4.0,0.0,0.0",
			"	-195=195,1,100,10.0,4.0,0.0,0.0",
			"	-196=196,1,100,10.0,4.0,0.0,0.0",
			"	-197=197,1,100,10.0,4.0,0.0,0.0",
			"	-198=198,1,100,10.0,4.0,0.0,0.0",
			"	-199=199,1,100,10.0,4.0,0.0,0.0",
			"	-200=200,1,100,10.0,4.0,0.0,0.0",
			"	-201=201,1,100,10.0,4.0,0.0,0.0",
			"	-202=202,1,100,10.0,4.0,0.0,0.0",
			"	-203=203,1,100,10.0,4.0,0.0,0.0",
			"	-204=204,1,100,10.0,4.0,0.0,0.0",
			"	-205=205,1,100,10.0,4.0,0.0,0.0",
			"	-206=206,1,100,10.0,4.0,0.0,0.0",
			"	-207=207,1,100,10.0,4.0,0.0,0.0",
			"	-208=208,1,100,10.0,4.0,0.0,0.0",
			"	-209=209,1,100,10.0,4.0,0.0,0.0",
			"	-210=210,1,100,10.0,4.0,0.0,0.0",
			"	-211=211,1,100,10.0,4.0,0.0,0.0",
			"	-212=212,1,100,10.0,4.0,0.0,0.0",
			"	-213=213,1,100,10.0,4.0,0.0,0.0",
			"	-214=214,1,100,10.0,4.0,0.0,0.0",
			"	-215=215,1,100,10.0,4.0,0.0,0.0",
			"	-216=216,1,100,10.0,4.0,0.0,0.0",
			"	-217=217,1,100,10.0,4.0,0.0,0.0",
			"	-218=218,1,100,10.0,4.0,0.0,0.0",
			"	-219=219,1,100,10.0,4.0,0.0,0.0",
			"	-220=220,1,100,10.0,4.0,0.0,0.0",
			"	-221=221,1,100,10.0,4.0,0.0,0.0",
			"	-222=222,1,100,10.0,4.0,0.0,0.0",
			"	-223=223,1,100,10.0,4.0,0.0,0.0",
			"	-224=224,1,100,10.0,4.0,0.0,0.0",
			"	-225=225,1,100,10.0,4.0,0.0,0.0",
			"	-226=226,1,100,10.0,4.0,0.0,0.0",
			"	-227=227,1,100,10.0,4.0,0.0,0.0",
			"	-228=228,1,100,10.0,4.0,0.0,0.0",
			"	-229=229,1,100,10.0,4.0,0.0,0.0",
			"	-230=230,1,100,10.0,4.0,0.0,0.0",
			"	-231=231,1,100,10.0,4.0,0.0,0.0",
			"	-232=232,1,100,10.0,4.0,0.0,0.0",
			"	-233=233,1,100,10.0,4.0,0.0,0.0",
			"	-234=234,1,100,10.0,4.0,0.0,0.0",
			"	-235=235,1,100,10.0,4.0,0.0,0.0",
			"	-236=236,1,100,10.0,4.0,0.0,0.0",
			"	-237=237,1,100,10.0,4.0,0.0,0.0",
			"	-238=238,1,100,10.0,4.0,0.0,0.0",
			"	-239=239,1,100,10.0,4.0,0.0,0.0",
			"	-240=240,1,100,10.0,4.0,0.0,0.0",
			"	-241=241,1,100,10.0,4.0,0.0,0.0",
			"	-242=242,1,100,10.0,4.0,0.0,0.0",
			"	-243=243,1,100,10.0,4.0,0.0,0.0",
			"	-244=244,1,100,10.0,4.0,0.0,0.0",
			"	-245=245,1,100,10.0,4.0,0.0,0.0",
			"	-246=246,1,100,10.0,4.0,0.0,0.0",
			"	-247=247,1,100,10.0,4.0,0.0,0.0",
			"	-248=248,1,100,10.0,4.0,0.0,0.0",
			"	-249=249,1,100,10.0,4.0,0.0,0.0",
			"	-250=250,1,100,10.0,4.0,0.0,0.0",
			"	-251=251,1,100,10.0,4.0,0.0,0.0",
			"	-252=252,1,100,10.0,4.0,0.0,0.0",
			"	-253=253,1,100,10.0,4.0,0.0,0.0",
			"	-254=254,1,130,10.0,4.0,0.0,0.0",
			"	-255=255,1,130,10.0,4.0,0.0,0.0",
		};

        protected override void OnCreate(Bundle bundle) {
			base.OnCreate(bundle);

			SetContentView(Resource.Layout.Main);

			//RequestWindowFeature(WindowFeatures.NoTitle);
			//this.Window.AddFlags(WindowManagerFlags.Fullscreen);
			//this.Window.ClearFlags(WindowManagerFlags.Fullscreen);

			AnalogClock ac = FindViewById <AnalogClock>(Resource.Id.analogClock1);
			ac.ScaleY = 1.5F;
			ac.ScaleX = 1.5F;

			var toast = Toast.MakeText(this, "시계를 터치하세요", ToastLength.Short);
			toast.Show();

			ac.Click += (sender, e) => {
				var intent = new Intent(this, typeof(SwdActivity));
				intent.PutStringArrayListExtra("weld_condition_data", weldConditionData);
				StartActivity(intent);
			};
		}
	}
}
