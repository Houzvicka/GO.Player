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
using GO.UWP.Player.Static;
using Microsoft.Media.TimedText;

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

        public List<CategoriesItem> CurrentCategoriesItems
        {
            get { return currentCategoriesItems; }
            set { Set(ref currentCategoriesItems, value); }
        }
        private List<CategoriesItem> currentCategoriesItems;

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

        public CountryItem CurrentlySelectedCountry
        {
            get { return currentlySelectedCountry; }
            set { Set(ref currentlySelectedCountry, value); }
        }
        private CountryItem currentlySelectedCountry;

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

            if (!settings.NationalDomain.IsNullOrWhiteSpace()) CurrentlySelectedCountry = Static.Static.CountriesList.First(c => c.NationalDomain == settings.NationalDomain);

            RegisterOrLoadCurrentDevice();
        }

        public void RegisterOrLoadCurrentDevice()
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

        public async Task<List<OperatorItem>> LoadOperators(CountryItem selected)
        {
            Operators operD = await communication.GetOperators(config.DefaultOperatorUri(selected.CountryCodeLong, selected.LanguageCode, "XONE"));
            Operators operO = await communication.GetOperators(config.ListOperatorsUri(selected.CountryCodeShort, selected.LanguageCode, "XONE"));

            List<OperatorItem> result = operD.Items;
            result.AddRange(operO.Items);
            return result;
        }

        public async Task<LoginResponse> TryLogin(string login, string password, Guid operatorId, CountryItem selected)
        {
            return await communication.Login(config.HboAccountLoginUri(selected.CountryCodeLong, selected.LanguageCode, "XONE"), login, password, operatorId, CurrentDevice);
        }

        public async void Login(string login, string password, Guid operatorId, CurrentDevice device)
        {
            CurrentUser = await communication.Login(config.HboAccountLoginUri(CurrentlySelectedCountry.CountryCodeLong, CurrentlySelectedCountry.LanguageCode, "XONE"), login, password, operatorId, device);
            // TODO if error logout
        }

        public async void LoadCategories()
        {
            CurrentCategoriesItems = (await communication.GetCategories(config.CategoriesUri(CurrentlySelectedCountry.CountryCodeShort, CurrentlySelectedCountry.LanguageCode))).Items.GetRange(0, 3);

            foreach (CategoriesItem item in CurrentCategoriesItems)
            {
                item.Container = (await communication.GetCategory(new Uri(item.ObjectUrl))).Container;
            }

            CurrentlySelectedMainPageCategory = CurrentCategoriesItems.FirstOrDefault();
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
                            CurrentlySelectedShow = await communication.GetShowDetail(content.ObjectUrl);

                            Messenger.Default.Send(new NavigateMainFrameMessage(typeof(EpisodePage)));
                            break;
                        case 2L: //Series
                            CurrentlySelectedDetail = await communication.GetShowDetail(content.ObjectUrl);

                            if (CurrentlySelectedDetail.Parent != null) CurrentlySelectedDetail = CurrentlySelectedDetail.Parent;

                            Messenger.Default.Send(new NavigateMainFrameMessage(typeof(DetailPage)));
                            break;
                        case 3L: //Episode
                            CurrentlySelectedShow = await communication.GetShowDetail(content.ObjectUrl);

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
            var response = await  communication.GetSearchResults(config.SearchUri(CurrentlySelectedCountry.CountryCodeShort, CurrentlySelectedCountry.LanguageCode, "XONE"), searchQuery);
            if (response != null && response.Container[0].Success)
                CurrentSearchSugestions = response.Container[0].Contents.Items;
        }

        public async void PlayContent(object show)
        {
            if (show is ContentsItem ci && ci.AllowPlay)
            {
                CurrentlySelectedVideo = await communication.GetPlayableLink(config.PurchaseUri(CurrentlySelectedCountry.CountryCodeShort, CurrentlySelectedCountry.LanguageCode, "XONE"), ci.Id, CurrentDevice.Individualization, settings.OperatorId, "CES", "XONE");
                Messenger.Default.Send(new NavigateMainFrameMessage(typeof(PlayerPage)));
            }
        }
    }
}
