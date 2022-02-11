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
using VisitorSignInSystem.Models;
using Windows.UI.Xaml.Documents;
using Windows.UI.Text;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
// add queue count to summary

namespace VisitorSignInSystem
{
    public sealed partial class SummaryPage : Page
    {
        DispatcherTimer dispatcherTimer;

        public SummaryPage()
        {
            this.InitializeComponent();

            // return to splash page if no activity from user after 1 minute
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            //dispatcherTimer.Interval = new System.TimeSpan(0, 0, 60);
            //dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object sender, object e)
        {
            ((DispatcherTimer)sender).Stop();
            this.Frame.Navigate(typeof(SplashViewPage));
        }

        private void SignInResult()
        {
            TextBlock tb = new TextBlock();
            tb.TextAlignment = TextAlignment.Center;
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Padding = new Thickness(5, 5, 5, 5);

            SolidColorBrush br = new SolidColorBrush(Windows.UI.Colors.SteelBlue);

            tb.Foreground = br;
            tb.FontFamily = new FontFamily("Century Gothic");
            tb.FontSize = 46;

            Run run = new Run();

            run.Text = ((App)Application.Current).visitor.FirstName + (((App)Application.Current).visitor.LastName.Length > 0 ? " " + ((App)Application.Current).visitor.LastName : "") + " has been entered into the queue.";
            tb.Inlines.Add(run);
            tb.Inlines.Add(new LineBreak());

            run = new Run();
            run.Text = @"Please proceed to the waiting area.";
            tb.Inlines.Add(run);
            tb.Inlines.Add(new LineBreak());

            run = new Run();
            run.FontStyle = FontStyle.Italic;
            run.FontWeight = FontWeights.Light;
            run.Text = "Thank you.";
            tb.Inlines.Add(run);

            spnMessage.Children.Add(tb);

            // return to splash page after 8 seconds
            dispatcherTimer.Interval = new System.TimeSpan(0, 0, 8);
            dispatcherTimer.Start();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ((App)Application.Current).visitor = (Visitor)e.Parameter;
        }

        private void Page_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (dispatcherTimer != null && dispatcherTimer.IsEnabled)
                dispatcherTimer.Stop();

            this.Frame.Navigate(typeof(SignInViewPage));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SignInResult();
        }
    }
}
