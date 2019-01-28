using Windows.UI.Xaml.Controls;
using CommonServiceLocator;
using GO.UWP.Player.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GO.UWP.Player.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CategoryDetailPage : Page
    {
        private MainViewModel main;

        public CategoryDetailPage()
        {
            this.InitializeComponent();

            main = ServiceLocator.Current.GetInstance<MainViewModel>();
        }

        private void DetailsGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            main.OpenDetailCommand.Execute(e.ClickedItem);
        }
    }
}
