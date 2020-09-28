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
using BassClefStudio.UWP.Navigation.DI;
using SkiaSharp.Views.UWP;
using System.Diagnostics;
using Windows.UI.Core;

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
            Window.Current.CoreWindow.KeyDown += KeyInputOn;
            Window.Current.CoreWindow.KeyUp += KeyInputOff;
            Focus(FocusState.Keyboard);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown -= KeyInputOn;
            Window.Current.CoreWindow.KeyUp -= KeyInputOff;
        }

        Stopwatch frameWatch = new Stopwatch();
        private void OnPaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            float time = frameWatch.ElapsedMilliseconds / 30f;
            time = time > 1 ? 1 : time;
            frameWatch.Restart();
            ViewModel.Update(time);

            // Get canvas and info
            var canvas = e.Surface.Canvas;
            SKSwapChainPanel panel = (sender as SKSwapChainPanel);
            // Draw
            ViewModel.PaintSurface(canvas, panel.CanvasSize);
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
    }
}
