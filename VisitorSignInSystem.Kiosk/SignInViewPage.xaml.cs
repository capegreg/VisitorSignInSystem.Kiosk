using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using VisitorSignInSystem.Models;
using System.Linq;
using System.Diagnostics;
using Windows.UI.Xaml.Media;
using Windows.UI;

/// <summary>
/// First page to collect data from visitor
/// Input fields
/// First name
/// Last name
/// Handicap check
/// </summary>

namespace VisitorSignInSystem
{
    public sealed partial class SignInViewPage : Page
    {        
        Brush paoBlue = (Brush)App.Current.Resources["PaoSolidColorBrushBlue"];
        Brush paoDisabledGrey = (Brush)App.Current.Resources["PaoSolidColorBrushDisabled"];

        DispatcherTimer dispatcherTimer;

        public SignInViewPage()
        {
            InitializeComponent();
            FirstNameTextBox.Focus(FocusState.Programmatic);

            // return to splash page if no activity from user after 1 minute
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new System.TimeSpan(0, 0, 60);
            dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object sender, object e)
        {
            GoToSplash();
        }

        private void GoToSplash()
        {
            if (dispatcherTimer != null && dispatcherTimer.IsEnabled)
                dispatcherTimer.Stop();

            dispatcherTimer = null;

            this.Frame.Navigate(typeof(SplashViewPage));
        }

        private void Page_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            // restart timer
            ResetTimer();
        }

        private void ResetTimer()
        {
            if (dispatcherTimer != null && !dispatcherTimer.IsEnabled)
                dispatcherTimer.Start();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FirstNameTextBox.Text = "";
            LastNameTextBox.Text = "";
            IsHandicapCheckBox.IsChecked = false;

            if (dispatcherTimer == null)
                dispatcherTimer = new DispatcherTimer();
        }

        private void FirstNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ResetTimer();
            btnNext.IsEnabled = PageIsValid();

            Brush br;

            if (btnNext.IsEnabled)
            {
                br = paoBlue;
            }
            else
            {
                br = paoDisabledGrey;
            }
            nextEllipse.Fill = br;
        }

        private bool PageIsValid()
        {
            return (bool)(FirstNameTextBox.Text.Length > 0 & LastNameTextBox.Text.Length > 0);
        }

        private void LastNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ResetTimer();
            btnNext.IsEnabled = PageIsValid();

            Brush br;

            if (btnNext.IsEnabled)
            {
                br = paoBlue;
            } else
            {
                br = paoDisabledGrey;
            }
            nextEllipse.Fill = br;
        }

        // Restrict First\Last NameTextBox punctuation, digits, use of CTRL key and emoji (set on textbox)
        // cancel must be true if either a, b or c are true, and d is true
        // cancel must be false if both a, b, or c are false, and d is true or false
        // (a || b || c) ^ d
        private void FirstNameTextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => (char.IsWhiteSpace(c) || char.IsDigit(c) || char.IsControl(c)) ^ IsAllowedChar(c));
        }

        private void LastNameTextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => (char.IsWhiteSpace(c) || char.IsDigit(c) || char.IsControl(c)) ^ IsAllowedChar(c));
        }

        /// <summary>
        /// Allow special punctuation only
        /// </summary>
        /// <param name="c"></param>
        /// <returns>bool</returns>
        private bool IsAllowedChar(char c)
        {
            // allow hyphen and single quote
            switch ((char)c)
            {
                case (char)39:
                    return char.IsPunctuation(c) && !(c == (char)39);
                case (char)45:
                    return char.IsPunctuation(c) && !(c == (char)45);
                default:
                    return char.IsPunctuation(c);
            }
        }

        private void btnQuit_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (dispatcherTimer != null && dispatcherTimer.IsEnabled)
                dispatcherTimer.Stop();

            dispatcherTimer = null;

            this.Frame.Navigate(typeof(SplashViewPage));
        }

        private void btnNext_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (dispatcherTimer != null && dispatcherTimer.IsEnabled)
                dispatcherTimer.Stop();

            dispatcherTimer = null;

            ((App)Application.Current).visitor.FirstName = FirstNameTextBox.Text;
            ((App)Application.Current).visitor.LastName = LastNameTextBox.Text;
            ((App)Application.Current).visitor.IsHandicap = Convert.ToBoolean(IsHandicapCheckBox.IsChecked);

            this.Frame.Navigate(typeof(ReasonsViewPage), ((App)Application.Current).visitor);
        }

        private void IsHandicapCheckBox_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            ResetTimer();
            btnNext.IsEnabled = PageIsValid();
        }
    }
}
