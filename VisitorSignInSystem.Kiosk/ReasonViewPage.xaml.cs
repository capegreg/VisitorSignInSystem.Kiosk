using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using VisitorSignInSystem.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using VisitorSignInSystem.Services;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.UI.Core;
using System.Globalization;

/// <summary>
/// Second page, present visitor with reason for visit options
/// Can make single choice
/// Reasons are sent from server
/// </summary>

namespace VisitorSignInSystem
{
    /// <summary>
    /// Global VARS declared in App.xaml.cs
    /// visitor, stores sign in data
    /// dispatcherTimer, used to timeout any page and return to splash
    /// </summary>
    public sealed partial class ReasonsViewPage : Page
    {
        // SignalR hub
        HubConnection connection;
        // handles idle display
        DispatcherTimer dispatcherTimer;
        // read settings
        // AppData\Local\Packages\VisitorSignInSystemKiosk_qnehf03sphg6a\LocalState\KioskLocalSettings
        Configuration kioskSettings;

        readonly string fileName = "kiosk_settings.json";
        readonly string folderName = "KioskLocalSettings";

        private ObservableCollection<Category> Reasons;

        public bool storyBoardCompleted { get; set; }
        public bool visitorAddSuccess { get; set; }
        public string KioskErrorMessage { get; set; }

        public ReasonsViewPage()
        {
            this.InitializeComponent();

            LoadingIndicator.IsActive = true;
            LoadingIndicatorText.Visibility = Visibility.Visible;

            KioskErrorMessage = "";

            // load this here
            Reasons = new ObservableCollection<Category>();

            ReasonGridSelectedStoryboard.Completed += ReasonGridSelectedStoryboard_Completed;
            ReasonSelectedFinishStoryboard.Completed += ReasonSelectedFinishStoryboard_Completed;

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

        private void LoadReasons(List<Category> cats)
        {
            Reasons.Clear();

            foreach (var c in cats)
            {
                c.Icon = $"/Assets/" + c.Icon;
                Reasons.Add(new Category { Id = c.Id, Description = c.Description, Icon = c.Icon });
            }

            stackInstructions.Visibility = Visibility.Visible;
            LoadingIndicator.IsActive = false;
            LoadingIndicatorText.Visibility = Visibility.Collapsed;
        }

        #region ********* config settings *********
        public async Task<Configuration> GetKioskSettings()
        {
            Configuration c = null;
            try
            {
                Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var kioskFolder = await localFolder.CreateFolderAsync(folderName, Windows.Storage.CreationCollisionOption.OpenIfExists);
                var files = await kioskFolder.GetFilesAsync();
                var desiredFile = files.FirstOrDefault(x => x.Name == fileName);
                if (desiredFile != null)
                {
                    var json = await Windows.Storage.FileIO.ReadTextAsync(desiredFile);
                    c = Newtonsoft.Json.JsonConvert.DeserializeObject<Configuration>(json);
                }
            }
            catch (System.NullReferenceException)
            {
                KioskErrorMessage = "Unable to reference storage.";
            }
            catch (Newtonsoft.Json.JsonSerializationException)
            {
                KioskErrorMessage = "Unable to read from Newton.";
            }
            catch (Exception)
            {
                KioskErrorMessage = "Unable to establish kiosk settings.";
            }
            return c;
        }
        private async Task<bool> KioskSettings()
        {
            kioskSettings = await GetKioskSettings();
            return kioskSettings != null;
        }
        #endregion

        private void GoToSplash()
        {
            if (connection != null)
                connection.StopAsync();

            if (dispatcherTimer != null && dispatcherTimer.IsEnabled)
                dispatcherTimer.Stop();

            dispatcherTimer = null;

            ReadyToSubmit.Visibility = Visibility.Collapsed;
            ReasonGridSelected.Visibility = Visibility.Collapsed;
            stackInstructions.Visibility = Visibility.Collapsed;

            btnFinish.IsEnabled = true;
            storyBoardCompleted = false;

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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ((App)Application.Current).visitor = (Visitor)e.Parameter;
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            if (dispatcherTimer != null && dispatcherTimer.IsEnabled)
                dispatcherTimer.Stop();

            dispatcherTimer = null;

            if (((App)Application.Current).visitor != null)
            {
                AddVisitor(((App)Application.Current).visitor);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            GoToSplash();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            bool ok = await KioskSettings();
            if (ok)
            {
                await OpenConnection();
            }
            else
            {
                LoadingIndicator.IsActive = false;
                LoadingIndicatorText.Visibility = Visibility.Collapsed;
                KioskError.Text = KioskErrorMessage;
            }

            if (dispatcherTimer == null)
                dispatcherTimer = new DispatcherTimer();
        }

        private void ReasonGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            Category item = e.ClickedItem as Category;
            ((App)Application.Current).visitor.VisitCategoryId = (ushort)item.Id;

            if (!storyBoardCompleted)
            {
                ReadyToSubmit.Visibility = Visibility.Visible;
                ReasonGridSelected.Visibility = Visibility.Visible;
                ReasonSelectedFinishStoryboard.Begin();
                btnFinish.IsEnabled = true;
            }

            // ok to replay
            ReasonGridSelectedStoryboard.Begin();
            ReasonGridSelectedText.Text = $"You chose: {item.Description}";
        }

        private void ReasonSelectedFinishStoryboard_Completed(object sender, object e)
        {
            storyBoardCompleted = true;
        }

        private void ReasonGridSelectedStoryboard_Completed(object sender, object e)
        {
            // not implemented
        }

        private async Task OpenConnection()
        {
            connection = new HubConnectionBuilder()
                .WithUrl(kioskSettings.Host, options =>
                {
                    options.UseDefaultCredentials = true;
                })
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .WithAutomaticReconnect()
                .Build();

            connection.HandshakeTimeout = TimeSpan.FromSeconds(90);

            try
            {
                // do all db calls after successful connection to server
                await connection.StartAsync();
                SendJoinGroup();
            }
            catch (Exception error)
            {
                Debug.WriteLine(error);
            }

            connection.Reconnecting += (error) =>
            {
                return Task.CompletedTask;
            };

            connection.Reconnected += (connectionId) =>
            {
                SendJoinGroup();
                return Task.CompletedTask;
            };

            /*
             * If the client doesn't successfully reconnect within its first four attempts, 
             * the HubConnection will transition to the Disconnected state and fire the Closed event. 
             * This provides an opportunity to attempt to restart the connection manually or inform 
             * users the connection has been permanently lost.
             * */
            connection.Closed += async (error) =>
            {
                await connection.StopAsync();
                await connection.DisposeAsync();
            };

            #region server messages

            connection.On<bool>("VisitorWasAdded", (m) =>
            {
                _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => VisitorAdded(m));
            });

            connection.On<List<Category>>("Categories", (m) =>
            {
                _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => LoadReasons(m));
            });
            #endregion
        } // end OpenConnection

        /// <summary>
        /// Callback after visitor add
        /// TODO: handle when unsuccessful
        /// </summary>
        /// <param name="m"></param>
        private void VisitorAdded(bool m)
        {
            if (m)
            {
                if (connection != null)
                    connection.StopAsync();

                visitorAddSuccess = true;
                ResetTimer();
                this.Frame.Navigate(typeof(SummaryPage), ((App)Application.Current).visitor);
            }
        }

        private async void SendJoinGroup()
        {
            ResetTimer();
            await connection.InvokeAsync("JoinGroup", kioskSettings.Name, kioskSettings.Location, "Kiosk", null);
        }

        private async void AddVisitor(Visitor visitor)
        {
            visitorAddSuccess = false;

            // formal case visitor
            visitor.FirstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(visitor.FirstName.Trim().ToLower());
            visitor.LastName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(visitor.LastName.Trim().ToLower());

            SignalRKioskService kioskService = new SignalRKioskService(connection);
            try
            {
                await kioskService.SendAddVisitorMessage(new Visitor()
                {
                    FirstName = visitor.FirstName,
                    LastName = visitor.LastName,
                    IsHandicap = visitor.IsHandicap,
                    VisitCategoryId = visitor.VisitCategoryId,
                    Kiosk = kioskSettings.Name,
                    Location = kioskSettings.Location
                });
            }
            catch (Exception)
            {
            }
        }
    } // end 

    public class Configuration
    {
        public int Location { get; set; }
        public string Host { get; set; }
        public string Name { get; set; }
        public int Await { get; set; }
    }
}