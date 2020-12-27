using Gtk;
using System;

namespace BassClefStudio.Dot.Gtk
{
	class Program
	{
		//[STAThread]
		public static void Main(string[] args)
		{
			Application.Init();

			using (var app = new Application("com.bassclefstudio.dot", GLib.ApplicationFlags.None))
			{
				app.Register(GLib.Cancellable.Current);

				var win = new MainWindow();
				app.AddWindow(win);

				win.Show();
				Application.Run();
			}
		}
	}
}
