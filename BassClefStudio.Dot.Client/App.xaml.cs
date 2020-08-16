using BassClefStudio.Dot.Client.Views;
using Decklan.UWP.ApplicationModel;

namespace BassClefStudio.Dot.Client
{
    sealed partial class App : Application
    {
        public App() : base(typeof(ShellPage), typeof(MainPage))
        { }
    }
}
