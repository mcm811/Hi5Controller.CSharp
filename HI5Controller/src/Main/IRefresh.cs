namespace Com.Changyoung.HI5Controller
{
	internal interface IRefresh
	{
		void Refresh(bool forced = false);
		bool Refresh(string path);
		string OnBackPressedFragment();
		void Show(string str);
	}
}