using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Browser.Classes
{
	public class TabDetails
	{
		public string TabHeaderTitle { get; set; }
		public BitmapImage TabHeaderFavicon { get; set; }
		public string SiteUrl { get; set; }

	}
}
