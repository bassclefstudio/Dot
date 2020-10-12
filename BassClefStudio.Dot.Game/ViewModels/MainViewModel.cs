﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using BassClefStudio.Dot.Core;
using BassClefStudio.Dot.Core.Levels;
using BassClefStudio.Dot.Core.Rendering;
using BassClefStudio.Dot.Serialization;
using BassClefStudio.NET.Core;
using BassClefStudio.UWP.Core;
using BassClefStudio.UWP.Navigation.DI;
using BassClefStudio.UWP.Services.Views;
using Newtonsoft.Json.Linq;
using SkiaSharp;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace BassClefStudio.Dot.Game.ViewModels
{
    public class MainViewModel : Observable, IViewModel
    {
        public GameState GameState { get; set; }

        public GameRenderer GameRenderer { get; private set; }

        public IStatusBarService StatusBarService { get; }

        public MainViewModel(GameState gameState, GameRenderer renderer, IStatusBarService statusBarService)
        {
            GameState = gameState;
            GameRenderer = renderer;
            StatusBarService = statusBarService;

            GameRenderer.AttachedContext = GameState;
            GameRenderer.ViewCamera = GameState.Camera;
            OpenCommand = new RelayCommandBuilder(ImportFile).Command;
            PlayDefaultCommand = new RelayCommandBuilder(PlayDefault).Command;
        }

        public ICommand OpenCommand { get; }
        public ICommand PlayDefaultCommand { get; }

        public async Task Initialize()
        { }

        public void PaintSurface(SKCanvas canvas, SKSize canvasSize)
        {
            GameState.Camera.SetView(new Vector2(canvasSize.Width, canvasSize.Height), 4);
            GameRenderer.Render(canvas);
        }

        public void Update(float deltaFrames)
        {
            GameState.Update(deltaFrames);
        }

        public async Task ImportFile()
        {
            await StatusBarService.StartAsync();
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".json");
            var file = await picker.PickSingleFileAsync();

            using (JsonGameService serializerService = new JsonGameService())
            {
                if (file != null)
                {
                    var json = await FileIO.ReadTextAsync(file);
                    GameState.Map = serializerService.ReadItem(JToken.Parse(json));
                }
            }
            await StatusBarService.StopAsync();
        }

        public async Task PlayDefault()
        {
            await StatusBarService.StartAsync();
            var installData = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("Data\\Game.json");
            using (JsonGameService serializerService = new JsonGameService())
            {
                if (installData != null)
                {
                    var json = await FileIO.ReadTextAsync(installData);
                    GameState.Map = serializerService.ReadItem(JToken.Parse(json));
                }
            }
            await StatusBarService.StopAsync();
        }
    }
}
