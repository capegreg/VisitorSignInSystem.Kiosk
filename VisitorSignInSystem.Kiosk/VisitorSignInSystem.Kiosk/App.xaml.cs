using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using VisitorSignInSystem.Services;
using Windows.UI.ViewManagement;
using VisitorSignInSystem.Models;

/// <summary>
/// Kiosk sign in UWP Application
/// Created by Gregory Bologna
/// UAT release date: 07/29/2021
/// </summary>

namespace VisitorSignInSystem
{

    public sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;

        // ****************************************************
        // global vars, instantiated in SplashViewPage tapped
        // global avoids memory issue where values are not cleared 
        public Visitor visitor;
        public DispatcherTimer dispatcherTimer;
        // ****************************************************

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        public App()
        {
            InitializeComponent();
            UnhandledException += OnAppUnhandledException;

            // Deferred execution until used. Check https://docs.microsoft.com/dotnet/api/system.lazy-1 for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }
            // app will open in fullscreen
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // TODO WTS: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/uwp/api/windows.ui.xaml.application.unhandledexception
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(SplashViewPage));
        }
    }
}