using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using GO.UWP.Player.Contracts;
using GO.UWP.Player.Services;

namespace GO.UWP.Player.ViewModel
{
	public class ViewModelLocator
	{
		public ViewModelLocator()
		{
			ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

			SimpleIoc.Default.Register<IConfigService, ConfigService>();
			SimpleIoc.Default.Register<ISettingsService, SettingsService>();
			SimpleIoc.Default.Register<ICommunicationService, CommunicationService>();
            
			SimpleIoc.Default.Register<MainViewModel>();
		}
        
		public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
	}
}