using System;
using System.Linq;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            main.CurrentSearchSugestions = null;

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if(main.CurrentCategoriesItems == null || main.CurrentCategoriesItems.Count == 0) main.LoadCategories();
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only get results when it was a user typing, 
            // otherwise assume the value got filled in by TextMemberPath 
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //Set the ItemsSource to be your filtered dataset
                //sender.ItemsSource = dataset;
                if (sender is AutoSuggestBox sb && sb.Text.Length >= 3)
                {
                    main.Search(sb.Text);
                }
            }
        }


        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (sender is AutoSuggestBox asb && args.SelectedItem is ContentsItem ci)
            {
                //asb.Text = ci.Name;
            }
        }


        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
                if (sender is AutoSuggestBox && args.ChosenSuggestion is ContentsItem ci)
                {
                    main.OpenDetailCommand.Execute(ci);
                }
            }
            else
            {
//                if (sender is AutoSuggestBox asb)
//                {
//                    FocusManager.TryMoveFocus(FocusNavigationDirection.Down);
//                }
                // Use args.QueryText to determine what to do.
            }
        }

        private void HomePage_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.GamepadRightShoulder || e.Key == VirtualKey.GamepadLeftShoulder)
            {
                //mainNavigationView.items
            }
        }

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            main.OpenDetailCommand.Execute(e.ClickedItem);
        }
    }
}
