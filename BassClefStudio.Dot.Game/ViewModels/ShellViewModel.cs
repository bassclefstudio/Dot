using Decklan.UWP.Navigation.DI;
using Decklan.UWP.Navigation.Shell;
using BassClefStudio.Dot.Game.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.Dot.Game.ViewModels
{
    public class ShellViewModel : NavigationViewModel, IViewModel
    {
        public ShellViewModel()
        {
            InitializePages(
                new NavigationPageItem[]
                {
                    new NavigationPageItem("Game", '\uE7FC', typeof(MainPage))
                },
                typeof(SettingsPage));
        }

        public async Task Initialize()
        { }
    }
}
