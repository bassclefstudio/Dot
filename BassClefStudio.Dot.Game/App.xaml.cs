using Autofac;
using BassClefStudio.Dot.Core;
using BassClefStudio.Dot.Core.Rendering;
using BassClefStudio.Dot.Game.Views;
using BassClefStudio.UWP.Lifecycle;
using BassClefStudio.UWP.Navigation;
using BassClefStudio.UWP.Navigation.DI;
using BassClefStudio.UWP.Navigation.Extensions;
using BassClefStudio.UWP.Services.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace BassClefStudio.Dot.Game
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : UWP.Lifecycle.Application
    {
        public override void BuildContainer(ContainerBuilder builder)
        {
            builder.AddNavigationService();
            NavigationService.InitializeContainer(BuildNavigationContainer);
            builder.RegisterType<LaunchNavigationActivationHandler>().AsImplementedInterfaces();
        }

        void BuildNavigationContainer(ContainerBuilder builder)
        {
            builder.AddViewModels(typeof(App).GetTypeInfo().Assembly);
            builder.RegisterType<GameState>().SingleInstance();
            builder.RegisterType<GameRenderer>().SingleInstance();
            builder.AddStatusBarService();
        }
    }

    public class LaunchNavigationActivationHandler : NavigationActivationHandler<LaunchActivatedEventArgs>
    {
        public LaunchNavigationActivationHandler(IEnumerable<INavigationHandler> handlers) : base(handlers)
        { }

        /// <inheritdoc/>
        public override bool StartNavigation(UWP.Lifecycle.Application app, LaunchActivatedEventArgs args, INavigationHandler handler)
        {
            return handler.ActivateWindow(app, typeof(MainPage), args);
        }
    }
}
