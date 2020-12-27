using BassClefStudio.NET.Core;
using Gtk;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.Gtk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.Dot.Gtk
{
    public class MainWindow : Window
    {
        public MainViewModel ViewModel { get; set; }

        private SKDrawingArea skiaView;
		public MainWindow() : base(WindowType.Toplevel)
		{
			ViewModel = new MainViewModel(new Core.GameState(), new Core.Rendering.GameRenderer());

			skiaView = new SKDrawingArea();
			skiaView.PaintSurface += OnPaintSurface;
            this.Shown += MainWindowShown;
			skiaView.Show();
			Child = skiaView;

			SynchronousTask initTask = new SynchronousTask(AnimationLoop);
			initTask.RunTask();
		}

        private void MainWindowShown(object sender, EventArgs e)
        {
			this.KeyPressEvent += OnKeyPressEvent;
			this.KeyReleaseEvent += OnKeyReleaseEvent;
		}

        private bool isActive = true;
        protected override void OnHidden()
        {
			isActive = false;
			base.OnHidden();
			Application.Quit();
        }

        Stopwatch stopwatch = new Stopwatch();
		private async Task AnimationLoop()
		{
			stopwatch.Start();
			while (isActive)
			{
				skiaView.QueueDraw();
				await Task.Delay(TimeSpan.FromSeconds(1.0 / 30));
			}
			stopwatch.Stop();
		}

		private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
		{
			float time = stopwatch.ElapsedMilliseconds / 30f;
			time = time > 1 ? 1 : time;
			stopwatch.Restart();
			ViewModel.Update(time);

			// Get canvas and info
			var canvas = e.Surface.Canvas;
			// Draw
			ViewModel.PaintSurface(canvas, skiaView.CanvasSize);
		}

		[GLib.ConnectBefore]
		protected void OnKeyPressEvent(object o, KeyPressEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Up)
			{
				ViewModel.GameState.Inputs.Jump = true;
			}
			else if (args.Event.Key == Gdk.Key.Right)
			{
				ViewModel.GameState.Inputs.MoveRight = true;
			}
			else if (args.Event.Key == Gdk.Key.Left)
			{
				ViewModel.GameState.Inputs.MoveLeft = true;
			}
		}

		[GLib.ConnectBefore]
		protected void OnKeyReleaseEvent(object o, KeyReleaseEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Up)
			{
				ViewModel.GameState.Inputs.Jump = false;
			}
			else if (args.Event.Key == Gdk.Key.Right)
			{
				ViewModel.GameState.Inputs.MoveRight = false;
			}
			else if (args.Event.Key == Gdk.Key.Left)
			{
				ViewModel.GameState.Inputs.MoveLeft = false;
			}
		}
	}
}
