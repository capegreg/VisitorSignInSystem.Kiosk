using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

/// <summary>
/// Landing page
/// Reset global VARS
/// visitor
/// dispatcherTimer
/// </summary>
 
namespace VisitorSignInSystem
{
    public sealed partial class SplashViewPage : Page
    {
        public SplashViewPage()
        {
            InitializeComponent();
            SplashStoryboard.Begin();
        }

        public static string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }

        private void Page_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            // reset global VARS here
            ((App)Application.Current).visitor = new Models.Visitor();
            //((App)Application.Current).dispatcherTimer = new DispatcherTimer();

            if (((App)Application.Current).dispatcherTimer != null && ((App)Application.Current).dispatcherTimer.IsEnabled)
                ((App)Application.Current).dispatcherTimer.Stop();

            this.Frame.Navigate(typeof(SignInViewPage));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            VersionText.Text = $"Manatee County Property Appraiser © {DateTime.Now.Year} (v{GetAppVersion()})";
        }
    }
}
