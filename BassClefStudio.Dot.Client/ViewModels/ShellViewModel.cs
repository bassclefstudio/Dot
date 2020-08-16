using BassClefStudio.Dot.Client.Views;
using Decklan.UWP.Navigation.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.Dot.Client.ViewModels
{
    public class ShellViewModel : NavigationViewModel
    {
        public ShellViewModel()
        {
            InitializePages(
                new NavigationPageItem[]
                {
                    new NavigationPageItem("Game", '\uE7FC', typeof(MainPage))
                },
                (NavigationPageItem)null);
        }
    }
}
