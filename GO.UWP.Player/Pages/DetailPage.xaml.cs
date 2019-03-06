using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using CommonServiceLocator;
using GO.UWP.Player.ViewModel;

namespace GO.UWP.Player.Pages
{
    public sealed partial class DetailPage
    {
        private MainViewModel main => (MainViewModel)DataContext;

        public DetailPage()
        {
            this.InitializeComponent();
        }

        private void DetailsGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            main.OpenDetailCommand.Execute(e.ClickedItem);
        }
    }
}
