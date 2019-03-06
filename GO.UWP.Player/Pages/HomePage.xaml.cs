using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GO.UWP.Player.ViewModel;
using GO.UWP.Player.Model;

namespace GO.UWP.Player.Pages
{
    public sealed partial class HomePage
    {
        private MainViewModel main => (MainViewModel)DataContext;

        public HomePage()
        {
            this.InitializeComponent();
        }
    }
}
