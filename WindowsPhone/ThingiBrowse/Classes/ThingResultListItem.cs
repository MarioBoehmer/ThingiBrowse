using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ThingiBrowse
{
    public class ThingResultListItem
    {
        public String ThingTitle { get; set; }
        public String ThingCreatedBy { get; set; }
        public String ThingTime { get; set; }
        public String ThingUrl { get; set; }
        public String ThingCreatorUrl { get; set; }
        public String ThingImageUrl { get; set; }

        public ThingResultListItem(String ThingTitle, String ThingCreatedBy, String ThingTime, String ThingUrl, String ThingCreatorUrl, String ThingImageUrl)
        {
            this.ThingTitle = ThingTitle;
            this.ThingCreatedBy = ThingCreatedBy;
            this.ThingTime = ThingTime;
            this.ThingUrl = ThingUrl;
            this.ThingCreatorUrl = ThingCreatorUrl;
            this.ThingImageUrl = ThingImageUrl;
        }

    }
}
