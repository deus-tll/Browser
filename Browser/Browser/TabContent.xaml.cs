using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls;
using Browser.Classes;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Web.WebView2.Core;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Popups;
using DataAccessLibrary;
using DataAccessLibrary.Classes;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

namespace Browser
{
	public sealed partial class TabContent : UserControl
	{
		private List<string> _searchTermsLocal = new List<string>();
		private List<ItemDetails> _historyDetailsList;
		private List<ItemDetails> _bookmarksDetailsList;
		public TabViewItem CurrentTab { get; set; }

		private readonly bool _isIncognito;

		public TabContent(bool isIncognito = false)
		{
			this.InitializeComponent();

			_isIncognito = isIncognito;
			if (_isIncognito)
				img_Incognito.Visibility = Visibility.Visible;
			
		}

		public string WebAddres
		{
			get { return (string)GetValue(WebAddresProperty); }
			set { SetValue(WebAddresProperty, value); }
		}


		public static readonly DependencyProperty WebAddresProperty = DependencyProperty.Register
																	  (
																		   "WebAddres",
																		   typeof(string),
																		   typeof(TabContent),
																		   new PropertyMetadata("https://www.google.com")
																	  );


		//////////////////////////////
		//////////////////////////////
		private void btn_Forward_Click(object sender, RoutedEventArgs e)
		{
			if (wv_Browser.CanGoForward)
				wv_Browser.GoForward();
		}


		private void btn_Back_Click(object sender, RoutedEventArgs e)
		{
			if (wv_Browser.CanGoBack)
				wv_Browser.GoBack();
		}


		private void btn_Home_Click(object sender, RoutedEventArgs e)
		{
			wv_Browser.Source = new Uri("https://www.google.com");
		}


		private void btn_Refresh_Click(object sender, RoutedEventArgs e)
		{
			wv_Browser.CoreWebView2.Reload();
		}


		private void btn_Stop_Click(object sender, RoutedEventArgs e)
		{
			wv_Browser.CoreWebView2.Stop();
		}
		//////////////////////////////
		//////////////////////////////
		

		//////////////////////////////
		//////////////////////////////
		private void mfl_NewTabMenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (_isIncognito)
				MainPage.Current.AddTab(true);
			else
				MainPage.Current.AddTab();
		}


		private void mfl_DownloadMenuItem_Click(object sender, RoutedEventArgs e)
		{
			wv_Browser.CoreWebView2.OpenDefaultDownloadDialog();
		}


		private void mfl_DevtoolsMenuItem_Click(object sender, RoutedEventArgs e)
		{
			wv_Browser.CoreWebView2.OpenDevToolsWindow();
		}


		private void mfl_HistoryMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MenuFlyoutDisplayingData(ref _historyDetailsList, SmallHistoryMenu, DataAccess.TableHistory, HistoryFlyoutMenu, btn_Menu);
		}


		private void mfl_BookMarksMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MenuFlyoutDisplayingData(ref _bookmarksDetailsList, SmallBookmarksMenu, DataAccess.TableBookmarks, BookmarksFlyoutMenu, btn_Menu);
		}


		private void MenuFlyoutDisplayingData(ref List<ItemDetails> itemDetails, ListView listView, string table, Flyout flyout, Button button)
		{
			try
			{
				itemDetails = DataAccess.GetAllItems(table);
				listView.ItemsSource = itemDetails;
			}
			catch (Exception)
			{

			}


			flyout.ShowAt(button);
			flyout.Placement = FlyoutPlacementMode.TopEdgeAlignedRight;
		}
		//////////////////////////////
		//////////////////////////////


		//////////////////////////////
		//////////////////////////////
		private void asb_searchUrlBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
		{
			if (sender.Text != string.Empty)
			{
				wv_Browser.Source = new Uri("https://www.google.com/search?q=" + sender.Text);

				if (!_isIncognito)
				{
					DataAccess.DeleteSearchTermFromTable(sender.Text);
					DataAccess.AddSearchTermToTable(sender.Text, DateTime.Now);
				}

				GetSearchTermsList();
			}
		}


		private void asb_searchUrlBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
		{
			List<string> filteredSearchTerms = new List<string>();

			foreach (string searchTerm in _searchTermsLocal)
			{
				if (searchTerm.ToLower().StartsWith(sender.Text.ToLower()))
				{
					filteredSearchTerms.Add(searchTerm);
				}
			}
			sender.ItemsSource = filteredSearchTerms;
		}


		private void asb_searchUrlBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
		{
			if (!_isIncognito)
			{
				DataAccess.DeleteSearchTermFromTable(sender.Text);
				DataAccess.AddSearchTermToTable(sender.Text, DateTime.Now);
			}


			wv_Browser.Source = new Uri("https://www.google.com/search?q=" + sender.Text);
		}


		private void asb_searchUrlBox_GotFocus(object sender, RoutedEventArgs e)
		{
			GetSearchTermsList();
		}


		private void GetSearchTermsList()
		{
			_searchTermsLocal.Clear();

			_searchTermsLocal = DataAccess.GetAllSearhcedTerms();

			_searchTermsLocal.Reverse();
		}
		//////////////////////////////
		//////////////////////////////


		//////////////////////////////
		//////////////////////////////
		private void wv_Browser_NavigationCompleted(WebView2 sender, CoreWebView2NavigationCompletedEventArgs args)
		{
			CurrentTab.HeaderTemplate = Application.Current.Resources["TabHeader_Template"] as DataTemplate;

			TabDetails tabDetails = new TabDetails();
			tabDetails.SiteUrl = wv_Browser.CoreWebView2.Source;
			tabDetails.TabHeaderTitle = wv_Browser.CoreWebView2.DocumentTitle;

			BitmapImage bmImage = new BitmapImage
			(
				new Uri("https://t3.gstatic.com/faviconV2?client=SOCIAL&type=FAVICON&fallback_opts=TYPE,SIZE,URL&url=" + tabDetails.SiteUrl + "&size=50")
			);

			tabDetails.TabHeaderFavicon = bmImage;

			CurrentTab.DataContext = tabDetails;

			asb_searchUrlBox.Text = wv_Browser.CoreWebView2.Source;

			if (!_isIncognito)
			{
				DataAccess.DeleteItemFromTable(DataAccess.TableHistory, wv_Browser.CoreWebView2.Source);
				DataAccess.AddItemToTable(DataAccess.TableHistory, wv_Browser.CoreWebView2.DocumentTitle, wv_Browser.CoreWebView2.Source, DateTime.Now);
			}


			btn_Stop.Visibility = Visibility.Collapsed;
			BrowserProgressBar.Visibility = Visibility.Collapsed;
		}


		private void wv_Browser_NavigationStarting(WebView2 sender, CoreWebView2NavigationStartingEventArgs args)
		{
			btn_Stop.Visibility = Visibility.Visible;
			BrowserProgressBar.Visibility = Visibility.Visible;
		}


		private void wv_Browser_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
		{
			wv_Browser.CoreWebView2.NewWindowRequested += wv_Browser_NewWindowRequested;
			wv_Browser.Source = new Uri(WebAddres);
		}


		private void wv_Browser_NewWindowRequested(CoreWebView2 sender, CoreWebView2NewWindowRequestedEventArgs args)
		{
			args.Handled = true;
			if (_isIncognito)
				MainPage.Current.AddTab(true, args.Uri);
			else
				MainPage.Current.AddTab(false, args.Uri);

		}
		//////////////////////////////
		//////////////////////////////


		//////////////////////////////
		//////////////////////////////
		private void SearchBookmarksMenuFlyout_Click(object sender, RoutedEventArgs e)
		{
			ToggleSearch(BookmarksSearchMenuItem, BookmarksSmallTitle);
		}


		private void SearchHistoryMenuFlyout_Click(object sender, RoutedEventArgs e)
		{
			ToggleSearch(HistorySearchMenuItem, HistorySmallTitle);
		}


		private void ToggleSearch(TextBox textBox, TextBlock textBlock)
		{
			if (textBox.Visibility == Visibility.Collapsed)
			{
				textBox.Visibility = Visibility.Visible;
				textBlock.Visibility = Visibility.Collapsed;
			}
			else
			{
				textBox.Visibility = Visibility.Collapsed;
				textBlock.Visibility = Visibility.Visible;
			}
		}
		//////////////////////////////
		//////////////////////////////


		//////////////////////////////
		//////////////////////////////
		private void SmallBookmarksMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			NavigationTo(sender, ref _bookmarksDetailsList);
		}


		private void SmallHistoryMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			NavigationTo(sender, ref _historyDetailsList);
		}


		private void NavigationTo(object sender, ref List<ItemDetails> itemDetails)
		{
			var s = sender as ListView;

			if (s.SelectedIndex > -1)
				wv_Browser.Source = new Uri(itemDetails[s.SelectedIndex].Url);
		}
		//////////////////////////////
		//////////////////////////////


		//////////////////////////////
		//////////////////////////////
		private void btn_Bookmarks_Click(object sender, RoutedEventArgs e)
		{
			textbox_bm_title.Text = wv_Browser.CoreWebView2.DocumentTitle;
			textbox_bm_url.Text = wv_Browser.CoreWebView2.Source;
		}


		private async void btn_AddBookmark_Click(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(textbox_bm_title.Text) && !string.IsNullOrEmpty(textbox_bm_url.Text))
			{
				DataAccess.DeleteItemFromTable(DataAccess.TableBookmarks, textbox_bm_url.Text);
				DataAccess.AddItemToTable(DataAccess.TableBookmarks, textbox_bm_title.Text, textbox_bm_url.Text, DateTime.Now);
			}
			else
			{
				MessageDialog msg = new MessageDialog("Fields must be filled");
				await msg.ShowAsync();
			}
		}


		private void btn_CancelBookmark_Click(object sender, RoutedEventArgs e)
		{
			btn_BookmarkFlyout.Hide();
		}
		//////////////////////////////
		//////////////////////////////


		//////////////////////////////
		//////////////////////////////
		private void BookmarksSearchMenuItem_TextChanged(object sender, TextChangedEventArgs e)
		{
			ListFiltering(sender, SmallBookmarksMenu, ref _bookmarksDetailsList);
		}


		private void HistorySearchMenuItem_TextChanged(object sender, TextChangedEventArgs e)
		{
			ListFiltering(sender, SmallHistoryMenu, ref _historyDetailsList);
		}


		private void ListFiltering(object sender, ListView listView, ref List<ItemDetails> sourceList)
		{
			var s = sender as TextBox;
			List<ItemDetails> filteredList = new List<ItemDetails>();

			if (s.Text == string.Empty)
			{
				listView.ItemsSource = sourceList;
				return;
			}

			foreach (var item in sourceList)
			{
				if (s.Text.ToLower().StartsWith("https://") || s.Text.ToLower().StartsWith("http://"))
				{
					if (item.Url.ToLower().StartsWith(s.Text.ToLower()))
					{
						filteredList.Add(item);
						continue;
					}
				}

				if (item.Title.ToLower().StartsWith(s.Text.ToLower()))
				{
					filteredList.Add(item);
				}
			}

			listView.ItemsSource = filteredList;
		}
		//////////////////////////////
		//////////////////////////////


		//////////////////////////////
		//////////////////////////////
		private void ClearHistoryDataFlyout_Click(object sender, RoutedEventArgs e)
		{
			ClearDataFromTable(DataAccess.TableHistory, "Are you sure you want to delete your browser history?");
		}


		private void ClearBookmarksDataFlyout_Click(object sender, RoutedEventArgs e)
		{
			ClearDataFromTable(DataAccess.TableBookmarks, "Are you sure you want to delete your Bookmarks?");
		}


		private async void ClearDataFromTable(string table, string message)
		{
			MessageDialog msg = new MessageDialog(message);
			msg.Commands.Add(new UICommand("Yes", null));
			msg.Commands.Add(new UICommand("No", null));
			msg.DefaultCommandIndex = 0;
			msg.CancelCommandIndex = 1;

			var cmd = await msg.ShowAsync();

			if (cmd.Label == "Yes")
			{
				DataAccess.DeleteAllRecordsFromTable(table);
			}
		}
		//////////////////////////////
		//////////////////////////////


		//////////////////////////////
		//////////////////////////////
		private async void mfl_NewWindowItem_Click(object sender, RoutedEventArgs e)
		{
			await OpenNewPageAsWindowAsync(typeof(MainPage));
		}


		private void mfl_NewPrivateTabItem_Click(object sender, RoutedEventArgs e)
		{
			MainPage.Current.AddTab(true);
		}


		private async Task<bool> OpenNewPageAsWindowAsync(Type t)
		{
			var view = CoreApplication.CreateNewView();
			int id = 0;

			await view.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				var frame = new Frame();
				frame.Navigate(t, null);
				Window.Current.Content = frame;
				Window.Current.Activate();
				id = ApplicationView.GetForCurrentView().Id;
			});

			return await ApplicationViewSwitcher.TryShowAsStandaloneAsync(id);
		}
		//////////////////////////////
		//////////////////////////////


		public void CloseWebView()
		{
			if (!_isIncognito)
			{
				DataAccess.DeleteItemFromTable(DataAccess.TableHistory, wv_Browser.CoreWebView2.Source);
				DataAccess.AddItemToTable(DataAccess.TableHistory, wv_Browser.CoreWebView2.DocumentTitle, wv_Browser.CoreWebView2.Source, DateTime.Now);
			}

			wv_Browser.Close();
		}
	}
}
