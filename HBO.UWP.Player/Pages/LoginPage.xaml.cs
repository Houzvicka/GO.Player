using System;
using System.Collections.Generic;
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
using CommonServiceLocator;
using GalaSoft.MvvmLight.Messaging;
using HBO.UWP.Player.Contracts;
using HBO.UWP.Player.Messages;
using HBO.UWP.Player.Model;
using HBO.UWP.Player.ViewModel;

namespace HBO.UWP.Player.Pages
{
    public sealed partial class LoginPage : Page
    {
        private MainViewModel mvm => (MainViewModel)DataContext;
        private ISettingsService settings;

        public LoginPage()
        {
            this.InitializeComponent();

            settings = ServiceLocator.Current.GetInstance<ISettingsService>();
        }

        private async void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            LoginResponse lr = await mvm.TryLogin(usernameTextBox.Text, passBox.Password);
            if (lr.Error == null)
            {
                settings.Username = usernameTextBox.Text;
                settings.Password = passBox.Password;

                mvm.CurrentUser = lr;
                Messenger.Default.Send(new NavigateMainFrameMessage(typeof(HomePage)));
            }
            else
            {
                ErrorTextBlock.Text = lr.ErrorMessage;
            }
        }
    }
}
