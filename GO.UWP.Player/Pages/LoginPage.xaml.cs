using System;
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Messaging;
using GO.UWP.Player.Contracts;
using GO.UWP.Player.Messages;
using GO.UWP.Player.Model;
using GO.UWP.Player.Static;
using GO.UWP.Player.ViewModel;
using Microsoft.AppCenter.Analytics;

namespace GO.UWP.Player.Pages
{
    public sealed partial class LoginPage : Page
    {
        private MainViewModel mvm => (MainViewModel)DataContext;
        private ISettingsService settings;

        public LoginPage()
        {
            this.InitializeComponent();

            settings = ServiceLocator.Current.GetInstance<ISettingsService>();

            countryBox.ItemsSource = Static.Static.CountriesList;
            countryBox.DisplayMemberPath = "Name";
            countryBox.SelectionChanged += CountryBox_SelectionChanged;
        }

        private async void CountryBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (countryBox.SelectedItem is CountryItem selected)
            {
                List<OperatorItem> operators = await mvm.LoadOperators(selected);
                operatorBox.ItemsSource = operators;
                operatorBox.DisplayMemberPath = "Name";
                operatorBox.SelectedIndex = 0;
            }
        }

        private async void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (countryBox.SelectedItem is CountryItem selectedC && operatorBox.SelectedItem is OperatorItem selectedO)
            {
                LoginResponse lr = await mvm.TryLogin(usernameTextBox.Text, passBox.Password, selectedO.Id, selectedC);
                if (lr != null && lr.Error == null)
                {
                    Analytics.TrackEvent("LoginSuccess");

                    settings.NationalDomain = selectedC.NationalDomain;
                    settings.OperatorId = selectedO.Id;
                    settings.Username = usernameTextBox.Text;
                    settings.Password = passBox.Password;

                    mvm.CurrentUser = lr;
                    mvm.CurrentlySelectedCountry = selectedC;
                    Messenger.Default.Send(new NavigateMainFrameMessage(typeof(HomePage)));
                }
                else
                {
                    Analytics.TrackEvent("LoginFailed");
                    ErrorTextBlock.Text = lr?.Error == null ? "Unable to Login" : lr.ErrorMessage;
                }
            }
        }

        private void LoginPage_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                FocusManager.TryMoveFocus(FocusNavigationDirection.Down);
                e.Handled = true;
            }
        }
    }
}
