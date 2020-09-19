using BassClefStudio.Dot.Core;
using BassClefStudio.Dot.Serialization;
using BassClefStudio.UWP.Core;
using BassClefStudio.UWP.Navigation.DI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace BassClefStudio.Dot.Game.ViewModels
{
    public class SettingsViewModel : IViewModel
    {
        public GameState GameState { get; set; }

        public SettingsViewModel(GameState gameState)
        {
            GameState = gameState;
            OpenCommand = new RelayCommandBuilder(OpenFileAsync).Command;
        }

        public ICommand OpenCommand { get; }

        public async Task Initialize()
        { }

        public async Task OpenFileAsync()
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".json");
            var file = await picker.PickSingleFileAsync();

            JsonGameService serializerService = new JsonGameService();

            if (file != null)
            {
                var json = await FileIO.ReadTextAsync(file);
                GameState.Map = serializerService.ReadItem(JToken.Parse(json));
            }
        }
    }
}
