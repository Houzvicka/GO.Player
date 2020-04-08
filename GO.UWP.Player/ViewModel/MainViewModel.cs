using System;
using System.Collections.Generic;
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

        public CategoriesItem CurrentlySelectedMainPageCategory
        {
            get { return currentlySelectedMainPageCategory; }
            set { Set(ref currentlySelectedMainPageCategory, value); }
        }
        private CategoriesItem currentlySelectedMainPageCategory;

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

        public bool IsMainMenuPaneOpen
        {
            get { return isMainMenuPaneOpen; }
            set { Set(ref isMainMenuPaneOpen, value); }
        }
        private bool isMainMenuPaneOpen;

        public string CurrentSearchQuery
        {
            get { return currentSearchQuery; }
            set { Set(ref currentSearchQuery, value); }
        }
        private string currentSearchQuery;

        public List<ContentsItem> CurrentSearchSugestions
        {
            get { return currentSearchSugestions; }
            set { Set(ref currentSearchSugestions, value); }
        }
        private List<ContentsItem> currentSearchSugestions;

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
                settings.CurrentDeviceId = Guid.NewGuid().ToString();
                settings.Individualization = Guid.NewGuid().ToString();
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

        public async Task<LoginResponse> TryLogin(string login, string password, int operatorId)
        {
            return await communication.Login(config.HboAccountLoginUri, login, password, operatorId, CurrentDevice);
        }

        public async void Login(string login, string password, int operatorId, CurrentDevice device)
        {
            CurrentUser = await communication.Login(config.HboAccountLoginUri, login, password, operatorId, device);
        }

        public async void LoadCategories()
        {
            CurrentCategories = await communication.GetCategories(config.CategoriesUri);
            CurrentlySelectedMainPageCategory = CurrentCategories.Items.FirstOrDefault();
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

                    if(CurrentlySelectedCategory.Container.Count > 1) Messenger.Default.Send(new NavigateMainFrameMessage(typeof(CategoryPage)));
                    else Messenger.Default.Send(new NavigateMainFrameMessage(typeof(CategoryDetailPage)));
                    break;
                case Container container:
                    CurrentlySelectedCategory = await communication.GetCategory(container.ObjectUrl);
                    
                    Messenger.Default.Send(new NavigateMainFrameMessage(typeof(CategoryDetailPage)));
                    break;
                case ContentsItem content:
                    switch (content.ContentType)
                    {
                        case 1L: //Movie
                            CurrentlySelectedShow = content;

                            Messenger.Default.Send(new NavigateMainFrameMessage(typeof(EpisodePage)));
                            break;
                        case 2L: //Series
                            CurrentlySelectedDetail = await communication.GetShowDetail(content.ObjectUrl);

                            if (CurrentlySelectedDetail.Parent != null) CurrentlySelectedDetail = CurrentlySelectedDetail.Parent;

                            Messenger.Default.Send(new NavigateMainFrameMessage(typeof(DetailPage)));
                            break;
                        case 3L: //Episode
                            CurrentlySelectedShow = content;

                            Messenger.Default.Send(new NavigateMainFrameMessage(typeof(EpisodePage)));
                            break;
                        case 5L: //Episodes List
                            CurrentlySelectedDetail = await communication.GetShowDetail(content.ObjectUrl);

                            Messenger.Default.Send(new NavigateMainFrameMessage(typeof(DetailPage)));
                            break;
                    }
                    break;
            }
        }

        public async void Search(string searchQuery)
        {
            var response = await  communication.GetSearchResults(config.SearchUri, searchQuery);
            if (response != null && response.Container[0].Success)
                CurrentSearchSugestions = response.Container[0].Contents.Items;
        }

        public async void PlayContent(object show)
        {
            if (show is ContentsItem ci && ci.AllowPlay)
            {
                CurrentlySelectedVideo = await communication.GetPlayableLink(config.PurchaseUri, ci.Id, CurrentDevice.Individualization, settings.OperatorId);

#if DEBUG
                Messenger.Default.Send(new NavigateMainFrameMessage(typeof(DebugPlayerPage)));
#else
                Messenger.Default.Send(new NavigateMainFrameMessage(typeof(PlayerPage)));
#endif
            }
        }
    }
}
