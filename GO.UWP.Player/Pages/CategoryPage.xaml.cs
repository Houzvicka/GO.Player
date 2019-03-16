using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GO.UWP.Player.ViewModel;

namespace GO.UWP.Player.Pages
{
    public sealed partial class CategoryPage : Page
    {
        private MainViewModel main => (MainViewModel)DataContext;

        public CategoryPage()
        {
            this.InitializeComponent();
        }

        private void DetailsGridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            main.OpenDetailCommand.Execute(e.ClickedItem);
        }
    }
}
