using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
            if (sender is AutoSuggestBox && args.SelectedItem is ContentsItem ci)
            {
                main.OpenDetailCommand.Execute(ci);
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
                // Use args.QueryText to determine what to do.
            }
        }
    }
}
