using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using HBO.UWP.Player.Contracts;
using HBO.UWP.Player.Services;

namespace HBO.UWP.Player.ViewModel
{
	public class ViewModelLocator
	{
		public ViewModelLocator()
		{
			ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

			SimpleIoc.Default.Register<IConfigService, ConfigService>();
			SimpleIoc.Default.Register<ISettingsService, SettingsService>();
            
			SimpleIoc.Default.Register<MainViewModel>();
		}
        
		public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
	}
}