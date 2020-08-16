using BassClefStudio.Dot.Core;
using Decklan.UWP.Navigation.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.Dot.Client.ViewModels
{
    public class MainViewModel : IViewModel
    {
        public GameState GameState { get; }

        public MainViewModel(GameState gameState)
        {

        }

        public async Task Initialize()
        {
        }
    }
}
