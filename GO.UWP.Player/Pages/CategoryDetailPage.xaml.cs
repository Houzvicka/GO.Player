using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CommonServiceLocator;
using GO.UWP.Player.ViewModel;

namespace GO.UWP.Player.Pages
{
    public sealed partial class CategoryDetailPage : Page
    {
        private MainViewModel main => (MainViewModel)DataContext;

        public CategoryDetailPage()
        {
            this.InitializeComponent();
        }

        private void DetailsGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            main.OpenDetailCommand.Execute(e.ClickedItem);
        }
    }
}
