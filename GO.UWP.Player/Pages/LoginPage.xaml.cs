using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
