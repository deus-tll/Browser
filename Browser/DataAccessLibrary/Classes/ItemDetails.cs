using System;
using Windows.UI.Xaml.Media.Imaging;

namespace DataAccessLibrary.Classes
{
	public class ItemDetails
	{
		public string Title { get; set; }
		public string Url { get; set; }
		public BitmapImage ImageSource { get; set; }
		public DateTime Date { get; set; }

	}
}
