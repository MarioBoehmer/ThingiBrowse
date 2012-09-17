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
using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using ThingiBrowse.Classes;

namespace ThingiBrowse
{
    public partial class ImagePage : PhoneApplicationPage
    {

        ObservableCollection<ThingImage> thingImages = null;

        public ImagePage()
        {
            InitializeComponent();

            thingImages = new ObservableCollection<ThingImage>();
            lstImage.ItemsSource = thingImages;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            String imageDetailUrls = "";
            if (NavigationContext.QueryString.TryGetValue("imageDetailUrls", out imageDetailUrls))
            {
                String[] imageDetailUrlsArray = imageDetailUrls.Split('|');
                foreach(String imageDetailUrl in imageDetailUrlsArray) {
                    WebClient newThingsWebClient = new WebClient();
                    newThingsWebClient.DownloadStringAsync(new Uri(imageDetailUrl));
                    newThingsWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(ImageDownloadStringCompleted);
                }
            }
        }

        void ImageDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            lock (this)
            {
                if (e.Error == null)
                {
                    try
                    {
                        ThingImage thingImage = new ThingImage();
                        thingImage.imageSource = new BitmapImage(new Uri(ThingiverseHTMLParser.getLargeImageUrl(e.Result)));
                        thingImages.Add(thingImage);
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
            //imageProgressBar.Visibility = Visibility.Collapsed;
            MessageBox.Show(Strings.network_error_message, Strings.network_error_title, MessageBoxButton.OK);
        }

        private void showResponseErrorMessage()
        {
            MessageBox.Show(Strings.response_error_message, Strings.response_error_title, MessageBoxButton.OK);
        }
    }
}