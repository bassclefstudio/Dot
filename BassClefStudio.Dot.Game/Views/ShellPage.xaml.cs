using BassClefStudio.UWP.Navigation;
using BassClefStudio.UWP.Navigation.DI;
using BassClefStudio.Dot.Game.ViewModels;
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
using BassClefStudio.UWP.Services.Views;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BassClefStudio.Dot.Game.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShellPage : Page, IViewWithViewModel<ShellViewModel>
    {
        public ShellViewModel ViewModel { get; set; }

        public ShellPage()
        {
            this.InitializeComponent();
            TitleBarService.HideTitleBar(this.AppTitleBar);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationService.Frame = this.mainFrame;
        }

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
