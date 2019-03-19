using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Messaging;
using GO.UWP.Player.Contracts;
using GO.UWP.Player.Messages;
using GO.UWP.Player.Model;
using GO.UWP.Player.ViewModel;

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

            operatorBox.ItemsSource = Operators.OperatorsList;
            operatorBox.DisplayMemberPath = "Item1";
            operatorBox.SelectedIndex = 0;
        }

        private async void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            LoginResponse lr = await mvm.TryLogin(usernameTextBox.Text, passBox.Password, operatorBox.SelectedIndex);
            if (lr.Error == null)
            {
                settings.Username = usernameTextBox.Text;
                settings.Password = passBox.Password;
                settings.OperatorId = operatorBox.SelectedIndex;

                mvm.CurrentUser = lr;
                Messenger.Default.Send(new NavigateMainFrameMessage(typeof(HomePage)));
            }
            else
            {
                ErrorTextBlock.Text = lr.ErrorMessage;
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
