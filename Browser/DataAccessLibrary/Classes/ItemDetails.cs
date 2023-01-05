﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
