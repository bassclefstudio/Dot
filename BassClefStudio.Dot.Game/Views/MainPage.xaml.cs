﻿using BassClefStudio.Dot.Game.ViewModels;
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
using BassClefStudio.UWP.Navigation.DI;
using SkiaSharp.Views.UWP;
using System.Diagnostics;
using Windows.UI.Core;
using BassClefStudio.UWP.Services.Views;
using BassClefStudio.NET.Core;
using System.Threading.Tasks;
using BassClefStudio.Graphics.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BassClefStudio.Dot.Game.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IViewWithViewModel<MainViewModel>
    {
        public MainViewModel ViewModel { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
        }

        public void OnViewModelInitialized()
        {
            TitleBarService.HideTitleBar(this.titleBar, this.loadingBar);
            Window.Current.CoreWindow.KeyDown += KeyInputOn;
            Window.Current.CoreWindow.KeyUp += KeyInputOff;
            this.win2dPanel.Focus(FocusState.Pointer);

            IGraphicsView graphicsView = new Win2DGraphicsView(this.win2dPanel);
            graphicsView.UpdateRequested += GraphicsView_UpdateRequested;
            ViewModel.LoadingChanged += ViewModel_LoadingChanged;
        }

        private void GraphicsView_UpdateRequested(object sender, UpdateRequestEventArgs e)
        {
            ViewModel.Draw(e);
        }

        private void ViewModel_LoadingChanged(object sender, EventArgs e)
        {
            win2dPanel.Paused = ViewModel.Loading;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown -= KeyInputOn;
            Window.Current.CoreWindow.KeyUp -= KeyInputOff;
        }

        private void KeyInputOn(object sender, KeyEventArgs e)
        {
            if(e.VirtualKey == Windows.System.VirtualKey.Up)
            {
                ViewModel.GameState.Inputs.Jump = true;
            }
            else if (e.VirtualKey == Windows.System.VirtualKey.Right)
            {
                ViewModel.GameState.Inputs.MoveRight = true;
            }
            else if (e.VirtualKey == Windows.System.VirtualKey.Left)
            {
                ViewModel.GameState.Inputs.MoveLeft = true;
            }
        }

        private void KeyInputOff(object sender, KeyEventArgs e)
        {
            if (e.VirtualKey == Windows.System.VirtualKey.Up)
            {
                ViewModel.GameState.Inputs.Jump = false;
            }
            else if (e.VirtualKey == Windows.System.VirtualKey.Right)
            {
                ViewModel.GameState.Inputs.MoveRight = false;
            }
            else if (e.VirtualKey == Windows.System.VirtualKey.Left)
            {
                ViewModel.GameState.Inputs.MoveLeft = false;
            }
        }

        private void GameUpdate(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedUpdateEventArgs args)
        {
            ViewModel.Update((float)(args.Timing.ElapsedTime / TimeSpan.FromMilliseconds(30)));
        }
    }
}
