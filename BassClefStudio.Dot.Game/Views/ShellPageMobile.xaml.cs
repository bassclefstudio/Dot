using BassClefStudio.UWP.Navigation.DI;
using BassClefStudio.Dot.Game.ViewModels;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BassClefStudio.Dot.Game.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellPageMobile : Page, IViewWithViewModel<ShellViewModel>
    {
        public ShellViewModel ViewModel { get; set; }

        public ShellPageMobile()
        {
            this.InitializeComponent();
        }

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    NavigationService.Frame = this.mainFrame;
        //    if (e.Parameter is ActivationInfo info)
        //    {
        //        NavigationService.Navigate(info.ChildPageType, info.Parameter);
        //    }
        //}

        private void NavigationView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            ViewModel.GoBack();
        }

        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            ViewModel.SelectedObject = args.SelectedItem;
            ViewModel.NavigateToSelectedPage();
        }

        public void OnViewModelInitialized()
        { }
    }
}
