using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PiProject
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            myFrame.Navigate(typeof(MeasurementsPage));
            TitleTextBlock.Text = "Measurements";
            /*navView.Header = "Home";
            navView.IsPaneOpen = false;

            navView.Loaded += NavView_Loaded;*/
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (home.IsSelected)
            {
                myFrame.Navigate(typeof(HomePage));
                TitleTextBlock.Text = "Home";
            }
            else if (smartcup.IsSelected)
            {
                myFrame.Navigate(typeof(DevicePage));
                TitleTextBlock.Text = "Device";
            }
            else if (measurements.IsSelected)
            {
                myFrame.Navigate(typeof(MeasurementsPage));
                TitleTextBlock.Text = "Measurements";
            }
            /*else if (settings.IsSelected)
            {
                myFrame.Navigate(typeof(SettingsPage));
                TitleTextBlock.Text = "Settings";
            }*/
            mySplitView.IsPaneOpen = false;
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            mySplitView.IsPaneOpen = !mySplitView.IsPaneOpen;
        }
        /*
        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            navView.IsPaneOpen = false;
        }
        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var itemContainer = args.InvokedItem as string;

            if (args.IsSettingsInvoked)
            {
                myFrame.Navigate(typeof(SettingsPage));
                navView.IsPaneOpen = false;
            }
            else
            {
                Type pageType = null;
                if (itemContainer == "Home")
                {
                    navView.Header = "Home";
                    pageType = typeof(HomePage);
                }
                else if (itemContainer == "Device")
                {
                    navView.Header = "Device";
                    pageType = typeof(DevicePage);
                }
                else if (itemContainer == "Measurements")
                {
                    navView.Header = "Measurements";
                    pageType = typeof(MeasurementsPage);
                }
                if (pageType != null)
                    myFrame.Navigate(pageType);
            }
            navView.IsPaneOpen = false;
        }
        */
    }
}
