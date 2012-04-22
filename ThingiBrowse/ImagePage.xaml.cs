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

namespace ThingiBrowse
{
    public partial class ImagePage : PhoneApplicationPage
    {

        public ImagePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string image = "";
            if (NavigationContext.QueryString.TryGetValue("image", out image))
            {
                WebClient newThingsWebClient = new WebClient();
                newThingsWebClient.DownloadStringAsync(new Uri(image));
                newThingsWebClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(ImageDownloadStringCompleted);
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
                        showContent(ThingiverseHTMLParser.getLargeImageUrl(e.Result));
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
            imageProgressBar.Visibility = Visibility.Collapsed;
            MessageBox.Show(Strings.network_error_message, Strings.network_error_title, MessageBoxButton.OK);
        }

        private void showResponseErrorMessage()
        {
            MessageBox.Show(Strings.response_error_message, Strings.response_error_title, MessageBoxButton.OK);
        }

        private void showContent(String imageUrl)
        {
            Image.Source = new BitmapImage(new Uri(imageUrl));
            Image.Visibility = Visibility.Visible;
            Image.SizeChanged += new SizeChangedEventHandler(imageSizeChangedHandler);
        }

        private void imageSizeChangedHandler(object sender, SizeChangedEventArgs args)
        {
            imageProgressBar.Visibility = Visibility.Collapsed;
        }
    }
}