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
using System.Windows.Shapes;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace ThingiBrowse
{
    public partial class InfoPage : PhoneApplicationPage
    {
        ApplicationBarIconButton applicationBarButtonInfo;

        public InfoPage()
        {
            InitializeComponent();
            applicationBarButtonInfo = getInfoButton();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (applicationBarButtonInfo != null)
            {
                ApplicationBar.Buttons.Remove(applicationBarButtonInfo);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (applicationBarButtonInfo != null)
            {
                ApplicationBar.Buttons.Add(applicationBarButtonInfo);
            }
        }

        private ApplicationBarIconButton getInfoButton()
        {
            foreach (var button in ApplicationBar.Buttons)
            {
                ApplicationBarIconButton applicationBarButton = button as ApplicationBarIconButton;
                if (applicationBarButton.IconUri.ToString().Contains("info"))
                {
                    return applicationBarButton;
                }
            }
            return null;
        }
    }
}