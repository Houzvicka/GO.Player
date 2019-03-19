using System.Diagnostics;
using Windows.System;
using Windows.System.Profile;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Messaging;
using GO.UWP.Player.Contracts;
using GO.UWP.Player.Messages;
using GO.UWP.Player.ViewModel;

namespace GO.UWP.Player.Pages
{
    public sealed partial class MainPage
    {
        private MainViewModel main => (MainViewModel) DataContext;
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

            this.GotFocus += (object sender, RoutedEventArgs e) =>
            {
                FrameworkElement focus = FocusManager.GetFocusedElement() as FrameworkElement;
                if (focus != null)
                {
                    Debug.WriteLine("got focus: " + focus.Name + " (" +
                                    focus.GetType().ToString() + ")");
                }
            };
        }

        private void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(settings.Username) || string.IsNullOrEmpty(settings.Password))
            {
                NavigateFrameTo(new NavigateMainFrameMessage(typeof(LoginPage)));
            }
            else
            {
                main.Login(settings.Username, settings.Password, settings.OperatorId, main.CurrentDevice);
                NavigateFrameTo(new NavigateMainFrameMessage(typeof(HomePage)));
            }
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!mainFrame.CanGoBack) return;
            mainFrame.GoBack();
            if (!mainFrame.CanGoBack)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
            e.Handled = true;
        }

        private void NavigateFrameTo(NavigateMainFrameMessage obj)
        {
            if (mainFrame.SourcePageType != obj.PageType) mainFrame.Navigate(obj.PageType);
            if (obj.PageType == typeof(HomePage))
            {
                mainFrame.BackStack.Clear();
                //var element = FocusManager.FindFirstFocusableElement((Page)mainFrame.Content);
                //FocusManager.TryFocusAsync(element, FocusState.Programmatic);

                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs e)
        {
            if (e.Handled)
            {
                if (e.VirtualKey == VirtualKey.GamepadA && main.IsMainMenuPaneOpen)
                {
                    NavigateFrameTo(new NavigateMainFrameMessage(typeof(HomePage)));
                    main.IsMainMenuPaneOpen = !main.IsMainMenuPaneOpen;
                    e.Handled = true;
                }
                return;
            }
            if (e.VirtualKey == VirtualKey.GamepadMenu)
            {
                main.IsMainMenuPaneOpen = !main.IsMainMenuPaneOpen;
                Debug.WriteLine("Menu");
                e.Handled = true;
            }
            if (e.VirtualKey == VirtualKey.GamepadB && main.IsMainMenuPaneOpen)
            {
                main.IsMainMenuPaneOpen = !main.IsMainMenuPaneOpen;
                Debug.WriteLine("Back");
                e.Handled = true;
            }
            if (e.VirtualKey == VirtualKey.GamepadB)
            {
                if(mainFrame.CanGoBack) mainFrame.GoBack();
                Debug.WriteLine("Back");
                e.Handled = true;
            }
            if (e.VirtualKey == VirtualKey.Enter)
            {
                FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
                e.Handled = true;
            }
        }
    }
}
