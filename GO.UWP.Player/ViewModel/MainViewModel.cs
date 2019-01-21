using System;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GO.UWP.Player.Contracts;
using GO.UWP.Player.Messages;
using GO.UWP.Player.Model;
using GO.UWP.Player.Pages;

namespace GO.UWP.Player.ViewModel
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

        public CategoriesItem CurrentlySelectedCategory
        {
            get { return currentlySelectedCategory; }
            set { Set(ref currentlySelectedCategory, value); }
        }
        private CategoriesItem currentlySelectedCategory;

        public ContentsItem CurrentlySelectedShow
        {
            get { return currentlySelectedShow; }
            set { Set(ref currentlySelectedShow, value); }
        }
        private ContentsItem currentlySelectedShow;

        public ContentsItem CurrentlySelectedDetail
        {
            get { return currentlySelectedDetaily; }
            set { Set(ref currentlySelectedDetaily, value); }
        }
        private ContentsItem currentlySelectedDetaily;

        public Video CurrentlySelectedVideo
        {
            get { return currentlySelectedVideo; }
            set { Set(ref currentlySelectedVideo, value); }
        }
        private Video currentlySelectedVideo;

        private IConfigService config;
        private ISettingsService settings;
        private ICommunicationService communication;
        
        public RelayCommand<object> PlayCommand => new RelayCommand<object>(PlayContent);
        public RelayCommand<object> OpenDetailCommand => new RelayCommand<object>(LoadCurrentContent);

        public MainViewModel(IConfigService config, ISettingsService settings, ICommunicationService communication)
        {
            this.config = config;
            this.settings = settings;
            this.communication = communication;

            RegisterOrLoadCurrentDevice();
            LoadCategories();
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

        public async Task<LoginResponse> TryLogin(string login, string password)
        {
            return await communication.Login(config.HboAccountLoginUri, login, password, 0, CurrentDevice);
        }

        public async void Login(string login, string password, int operatorId, CurrentDevice device)
        {
            CurrentUser = await communication.Login(config.HboAccountLoginUri, login, password, operatorId, device);
        }

        public async void LoadCategories()
        {
            CurrentCategories = await communication.GetCategories(config.CategoriesUri);
            CurrentlySelectedCategory = CurrentCategories.Items.FirstOrDefault();
        }

        public async void LoadCurrentContent(object currentType)
        {
            switch (currentType)
            {
                case CategoriesItem category:
                    string url = category.ObjectUrl;
                    url = url.Replace("{sort}", "0");
                    url = url.Replace("{pageIndex}", "1");
                    url = url.Replace("{pageSize}", "1024");

                    CurrentlySelectedCategory = await communication.GetCategory(new Uri(url));

                    Messenger.Default.Send(new NavigateMainFrameMessage(typeof(CategoryDetailPage)));
                    break;
                case ContentsItem content:
                    switch (content.ContentType)
                    {
                        case 1L:
                            CurrentlySelectedShow = content;

                            Messenger.Default.Send(new NavigateMainFrameMessage(typeof(EpisodePage)));
                            break;
                        case 2L:
                            CurrentlySelectedDetail = await communication.GetShowDetail(content.ObjectUrl);

                            Messenger.Default.Send(new NavigateMainFrameMessage(typeof(DetailPage)));
                            break;
                        case 3L:
                            CurrentlySelectedShow = content;

                            Messenger.Default.Send(new NavigateMainFrameMessage(typeof(EpisodePage)));
                            break;
                    }
                    break;
            }
        }

        public async void PlayContent(object show)
        {
            if (show is ContentsItem ci)
            {
                CurrentlySelectedVideo = await communication.GetPlayableLink(config.PurchaseUri, ci.Id, CurrentDevice.Individualization);

                Messenger.Default.Send(new NavigateMainFrameMessage(typeof(PlayerPage)));
            }
        }
    }
}
