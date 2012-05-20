using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace ThingiBrowse
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        Thing ThingDetails = null;
        bool isDataLoaded = false;

        // Constructor
        public DetailsPage()
        {
            InitializeComponent();
        }

        // When page is navigated to set data context to selected item in list
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string thingUrl = "";
            if (NavigationContext.QueryString.TryGetValue("selectedItem", out thingUrl) && !isDataLoaded)
            {
                WebClient newThingsWebClient = new WebClient();
                newThingsWebClient.DownloadStringAsync(new Uri(thingUrl));
                newThingsWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(ThingDownloadStringCompleted);
            }
        }

        void ThingDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            lock (this)
            {
                if (e.Error == null)
                {
                    try
                    {
                        ThingDetails = ThingiverseHTMLParser.getThing(e.Result);
                        DataContext = ThingDetails;
                        showContent();
                    }
                    catch (Exception ex)
                    {
                        showResponseErrorMessage();
                    }
                }
                else
                {
                    showNetworkErrorMessage();
                }
            }
        }

        private void showNetworkErrorMessage()
        {
            thingDetailsProgressBar.Visibility = Visibility.Collapsed;
            MessageBox.Show(Strings.network_error_message, Strings.network_error_title, MessageBoxButton.OK);
        }

        private void showResponseErrorMessage()
        {
            MessageBox.Show(Strings.response_error_message, Strings.response_error_title, MessageBoxButton.OK);
        }

        private void showContent()
        {
            if (ThingDetails != null)
            {
                if (ThingDetails.ThingDescription == null || ThingDetails.ThingDescription.Length <= 0)
                {
                    DescriptionPanel.Visibility = Visibility.Collapsed;
                }
                if (ThingDetails.ThingInstructions == null || ThingDetails.ThingInstructions.Length <= 0)
                {
                    InstructionsPanel.Visibility = Visibility.Collapsed;
                }
                FilesPanel.Children.Clear();
                if (ThingDetails.ThingFiles.Count > 0)
                {
                    var fileEnumerator = ThingDetails.ThingFiles.GetEnumerator();
                    while (fileEnumerator.MoveNext())
                    {
                        var fileEntry = fileEnumerator.Current;
                        String[] fileDetails = fileEntry.Value;
                        StackPanel fileDetailPanel = new StackPanel();
                        fileDetailPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;

                        Image fileImage = new Image();
                        fileImage.Source = new BitmapImage(new Uri(fileDetails[2]));
                        fileDetailPanel.Children.Add(fileImage);
                        //Disable ImagePage for default image
                        if (ThingDetails.ThingLargeImageUrl.EndsWith("/image:0"))
                        {
                            ThingImage.IsEnabled = false;
                        }

                        StackPanel fileDetailTextPanel = new StackPanel();
                        fileDetailTextPanel.Orientation = System.Windows.Controls.Orientation.Vertical;
                        fileDetailPanel.Children.Add(fileDetailTextPanel);

                        TextBlock fileTitle = new TextBlock();
                        fileTitle.FontSize = 23;
                        fileTitle.Foreground = new SolidColorBrush(Color.FromArgb(255, 8, 82, 151));
                        fileTitle.Margin = new Thickness(22, 0, 0, 0);
                        fileTitle.TextAlignment = TextAlignment.Left;
                        fileTitle.Text = fileDetails[0];
                        fileDetailTextPanel.Children.Add(fileTitle);

                        TextBlock fileSize = new TextBlock();
                        fileSize.FontSize = 23;
                        fileSize.Foreground = new SolidColorBrush(Color.FromArgb(255, 8, 82, 151));
                        fileSize.Margin = new Thickness(22, 0, 0, 0);
                        fileSize.TextAlignment = TextAlignment.Left;
                        fileSize.Text = fileDetails[1];
                        fileDetailTextPanel.Children.Add(fileSize);

                        FilesPanel.Children.Add(fileDetailPanel);
                    }
                }
                else
                {
                    FilesPanel.Visibility = Visibility.Collapsed;
                    FilesLabel.Visibility = Visibility.Collapsed;
                }
                thingDetailsProgressBar.Visibility = Visibility.Collapsed;
                ContentPanel.Visibility = Visibility.Visible;
                isDataLoaded = true;
            }
        }

        private void thingImageClicked(object sender, RoutedEventArgs args)
        {
            NavigationService.Navigate(new Uri("/ImagePage.xaml?image=" + ThingDetails.ThingLargeImageUrl, UriKind.Relative));
        }
    }
}