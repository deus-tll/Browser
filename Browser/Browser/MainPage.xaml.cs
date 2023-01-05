using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Core;

namespace Browser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
		public static MainPage Current;
        private CoreApplicationViewTitleBar _coreTitleBar;
		private ApplicationViewTitleBar _titleBar;

		public MainPage()
        {
            this.InitializeComponent();

			Current= this;

            _coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            _coreTitleBar.ExtendViewIntoTitleBar = true;

            _titleBar = ApplicationView.GetForCurrentView().TitleBar;
            _titleBar.ButtonBackgroundColor = Colors.Transparent;

            Window.Current.SetTitleBar(AppTitleBar);

			_coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
			_coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

			Window.Current.CoreWindow.Activated += CoreWindow_Activated;

			DefaultTabLoad();

		}


		//////////////////////////////
		//////////////////////////////
		private void DefaultTabLoad()
		{
			tvi_DefaultTab.HeaderTemplate = Application.Current.Resources["TabHeader_Template"] as DataTemplate;


			TabContent tabContent = new TabContent();
			tabContent.CurrentTab = tvi_DefaultTab;
			tabContent.WebAddres = "https://www.google.com";
			tvi_DefaultTab.Content = tabContent;
		}
		//////////////////////////////
		//////////////////////////////
		

		//////////////////////////////
		//////////////////////////////
		private void CoreWindow_Activated(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.WindowActivatedEventArgs args)
		{
			UISettings settings = new UISettings();

			if(args.WindowActivationState == CoreWindowActivationState.Deactivated)
				AppTitleBar.Background= new SolidColorBrush(Colors.SlateBlue);
			else
				AppTitleBar.Background = new SolidColorBrush(Colors.Transparent);
		}


		private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
		{
			if (sender.IsVisible)
				AppTitleBar.Visibility = Visibility.Visible;
			else
				AppTitleBar.Visibility = Visibility.Collapsed;
			
		}


		private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
		{
			LeftPaddingColumn.Width = new GridLength(_coreTitleBar.SystemOverlayLeftInset);
			RightPaddingColumn.Width = new GridLength(_coreTitleBar.SystemOverlayRightInset);
		}
		//////////////////////////////
		//////////////////////////////


		//////////////////////////////
		//////////////////////////////
		public void AddTab(bool isIncognito = false, string Address = "https://www.google.com")
		{
			TabViewItem newTab = new TabViewItem();
			newTab.HeaderTemplate = Application.Current.Resources["TabHeader_Template"] as DataTemplate;
			
			TabContent tabContent = new TabContent(isIncognito);
			tabContent.CurrentTab = newTab;
			tabContent.WebAddres = Address;
			newTab.Content = tabContent;
			
			tabView.TabItems.Add(newTab);
			tabView.SelectedItem= newTab;
		}

		private void TabView_AddTabButtonClick(Microsoft.UI.Xaml.Controls.TabView sender, object args)
		{
			AddTab();
		}

		private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
		{
            if (tabView.TabItems.Count > 1)
			{
				(args.Tab.Content as TabContent).CloseWebView();
				sender.TabItems.Remove(args.Tab);
			}
			else
			{
				Environment.Exit(0);
			}
		}
		//////////////////////////////
		//////////////////////////////
	}
}
