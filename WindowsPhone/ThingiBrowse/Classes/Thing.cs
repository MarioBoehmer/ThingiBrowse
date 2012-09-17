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
using System.Collections.Generic;

namespace ThingiBrowse
{
    public class Thing
    {
        public String ThingTitle { get; set; }
        public String ThingCreatedBy { get; set; }
        public String ThingCreatorImageUrl { get; set; }
        public String ThingDate { get; set; }
        public String ThingDescription { get; set; }
        public String ThingCreatorUrl { get; set; }
        public String ThingImageUrl { get; set; }
        public String ThingLargeImageUrl { get; set; }
        public String ThingInstructions { get; set; }
        public Dictionary<String, String[]> ThingFiles { get; set; }
        public List<String> ThingAllImageDetailPageUrls { get; set; }

        public Thing(String ThingTitle, String ThingCreatedBy, String ThingCreatorImageUrl, String ThingDate, String ThingDescription, String ThingCreatorUrl, String ThingImageUrl, String ThingLargeImageUrl, String ThingInstructions, Dictionary<String, String[]> ThingFiles, List<String> ThingAllImageDetailPageUrls)
        {
            this.ThingTitle = ThingTitle;
            this.ThingCreatedBy = ThingCreatedBy;
            this.ThingCreatorImageUrl = ThingCreatorImageUrl;
            this.ThingDate = ThingDate;
            this.ThingDescription = ThingDescription;
            this.ThingCreatorUrl = ThingCreatorUrl;
            this.ThingImageUrl = ThingImageUrl;
            this.ThingLargeImageUrl = ThingLargeImageUrl;
            this.ThingInstructions = ThingInstructions;
            this.ThingFiles = ThingFiles;
            this.ThingAllImageDetailPageUrls = ThingAllImageDetailPageUrls;
        }
    }
}
