using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage.Streams;
using Windows.System;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Messaging;
using HBO.UWP.Player.Contracts;
using HBO.UWP.Player.Helpers;
using HBO.UWP.Player.Messages;
using HBO.UWP.Player.Model;
using HBO.UWP.Player.Pages;
using HBO.UWP.Player.ViewModel;
using Newtonsoft.Json;

namespace HBO.UWP.Player
{
    public sealed partial class MainPage
    {
        private MainViewModel mvm => (MainViewModel) DataContext;
        private ISettingsService settings;

        public MainPage()
        {
            this.InitializeComponent();

            settings = ServiceLocator.Current.GetInstance<ISettingsService>();
            Messenger.Default.Register<NavigateMainFrameMessage>(this, NavigateFrameTo);

            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Xbox")
            {
                ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
            }

            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
        }

        private void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(settings.Username) || string.IsNullOrEmpty(settings.Password))
            {
                NavigateFrameTo(new NavigateMainFrameMessage(typeof(LoginPage)));
            }
            else
            {
                mvm.Login(settings.Username, settings.Password, 0, mvm.CurrentDevice);
                NavigateFrameTo(new NavigateMainFrameMessage(typeof(HomePage)));
            }
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!mainFrame.CanGoBack) return;
            mainFrame.GoBack();
            if (!mainFrame.CanGoBack) SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            e.Handled = true;
        }

        private void NavigateFrameTo(NavigateMainFrameMessage obj)
        {
            if (mainFrame.SourcePageType != obj.PageType) mainFrame.Navigate(obj.PageType);
            if (obj.PageType == typeof(HomePage))
            {
                mainFrame.BackStack.Clear();
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (args.Handled)
            {
                return;
            }
            if (args.VirtualKey == VirtualKey.GamepadMenu)
            {
                mainMenu.IsPaneOpen = !mainMenu.IsPaneOpen;
                Debug.WriteLine("Menu");
                args.Handled = true;
            }
            if (args.VirtualKey == VirtualKey.GamepadB && mainMenu.IsPaneOpen)
            {
                mainMenu.IsPaneOpen = !mainMenu.IsPaneOpen;
                Debug.WriteLine("Back");
                args.Handled = true;
            }
            if (args.VirtualKey == VirtualKey.GamepadB)
            {
                if(mainFrame.CanGoBack) mainFrame.GoBack();
                Debug.WriteLine("Back");
                args.Handled = true;
            }
        }

        private void HamburgerButton_OnClick(object sender, RoutedEventArgs e)
        {
            mainMenu.IsPaneOpen = !mainMenu.IsPaneOpen;
        }

        private void MainMenu_OnPaneClosed(SplitView sender, object args)
        {

        }

        private void MainMenu_OnPaneOpened(SplitView sender, object args)
        {
//            var first = mainLixtBox.Items?.FirstOrDefault();
//            if (first != null && first is ListBoxItem lbi) lbi.Focus(FocusState.Programmatic);
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only get results when it was a user typing, 
            // otherwise assume the value got filled in by TextMemberPath 
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //Set the ItemsSource to be your filtered dataset
                //sender.ItemsSource = dataset;
            }
        }


        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            // Set sender.Text. You can use args.SelectedItem to build your text string.
        }


        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
            }
            else
            {
                // Use args.QueryText to determine what to do.
            }
        }

        private void MainLixtBox_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            mainMenu.IsPaneOpen = !mainMenu.IsPaneOpen;
            
            NavigateFrameTo(new NavigateMainFrameMessage(typeof(HomePage)));
        }
    }
}
