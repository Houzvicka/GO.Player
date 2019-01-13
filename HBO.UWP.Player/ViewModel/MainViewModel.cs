using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using HBO.UWP.Player.Contracts;
using HBO.UWP.Player.Helpers;
using HBO.UWP.Player.Helpers.Playback;
using HBO.UWP.Player.Model;
using HBO.UWP.Player.Pages;

namespace HBO.UWP.Player.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public LoginResponse CurrentUser
        {
            get { return currentUser; }
            set { Set(ref currentUser, value); }
        }
        private LoginResponse currentUser;

        public CurrentDevice CurrentDevice
        {
            get { return currentDevice; }
            set { Set(ref currentDevice, value); }
        }
        private CurrentDevice currentDevice;

        public Categories CurrentCategories
        {
            get { return currentCategories; }
            set { Set(ref currentCategories, value); }
        }
        private Categories currentCategories;

        public Page CurrentPage
        {
            get { return currentPage; }
            set { Set(ref currentPage, value); }
        }
        private Page currentPage;

        public PlayerPage PlayerPage
        {
            get { return playerPage; }
            set { Set(ref playerPage, value); }
        }
        private PlayerPage playerPage;

        private IConfigService config;
        private ISettingsService settings;
        private ICommunicationService communication;

        public MainViewModel(IConfigService config, ISettingsService settings, ICommunicationService communication)
        {
            this.config = config;
            this.settings = settings;
            this.communication = communication;
            
            PlayerPage = new PlayerPage();

            RegisterOrLoadCurrentDevice();
            LoadCategories();
            Login("houzvickajiri@gmail.com", "primer.magnate.claptrap", 0, CurrentDevice);
        }

        public async void RegisterOrLoadCurrentDevice()
        {
            if (string.IsNullOrEmpty(settings.CurrentDeviceId))
            {
                CurrentDevice = (await communication.SilentRegister(config.DeviceRegistrationUri)).Data.Customer.CurrentDevice;

                settings.CurrentDeviceId = CurrentDevice.Id.ToString();
                settings.Individualization = CurrentDevice.Individualization;
            }
            else
            {
                CurrentDevice = new CurrentDevice()
                {
                    Id = new Guid(settings.CurrentDeviceId),
                    Individualization = settings.Individualization
                };
            }
        }
        
        public async void Login(string login, string password, int operatorId, CurrentDevice device)
        {
            CurrentUser = await communication.Login(config.HboAccountLoginUri, login, password, operatorId, device);
        }

        public async void LoadCategories()
        {
            CurrentCategories = await communication.GetCategories(config.CategoriesUri);
        }

        public async void LoadCurrentContent()
        {

        }

        public async void PlayContent(Guid showId)
        {
            NavigateToPlayerPage();

            Video playUrl = await communication.GetPlayableLink(config.PurchaseUri, showId, CurrentDevice.Individualization);
            Uri mfestUri = new Uri(playUrl.Purchase.MediaUrl.AbsoluteUri + "/manifest");

            PlayerPage.SetupRequestConfigData(CurrentUser.Customer.Id, playUrl.Purchase);
            PlayerPage.Play(mfestUri);
        }

        private void NavigateToPlayerPage()
        {
            CurrentPage = PlayerPage;
        }
    }
}
