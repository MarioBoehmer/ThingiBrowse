﻿#pragma checksum "C:\Dev\IDE\Git_Workspace\ThingiBrowse\WindowsPhone\ThingiBrowse\DetailsPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "5EA41D4CFE25C2D6CCC925A8D67B4168"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.544
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace ThingiBrowse {
    
    
    public partial class DetailsPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel ContentPanel;
        
        internal System.Windows.Controls.TextBlock ThingTitle;
        
        internal System.Windows.Controls.Button ThingImage;
        
        internal System.Windows.Controls.TextBlock ThingImageGalleryLabel;
        
        internal System.Windows.Controls.TextBlock ThingCreatedByLabel;
        
        internal System.Windows.Controls.TextBlock ThingCreatedBy;
        
        internal System.Windows.Controls.TextBlock ThingCreategOnLabel;
        
        internal System.Windows.Controls.TextBlock ThingCreatedOn;
        
        internal System.Windows.Controls.StackPanel DescriptionPanel;
        
        internal System.Windows.Controls.TextBlock ThingDescriptionLabel;
        
        internal System.Windows.Controls.TextBlock ThingDescription;
        
        internal System.Windows.Controls.StackPanel InstructionsPanel;
        
        internal System.Windows.Controls.TextBlock ThingInstructionsLabel;
        
        internal System.Windows.Controls.TextBlock ThingInstructions;
        
        internal System.Windows.Controls.TextBlock FilesLabel;
        
        internal System.Windows.Controls.StackPanel FilesPanel;
        
        internal System.Windows.Controls.ProgressBar thingDetailsProgressBar;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/ThingiBrowse;component/DetailsPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.StackPanel)(this.FindName("ContentPanel")));
            this.ThingTitle = ((System.Windows.Controls.TextBlock)(this.FindName("ThingTitle")));
            this.ThingImage = ((System.Windows.Controls.Button)(this.FindName("ThingImage")));
            this.ThingImageGalleryLabel = ((System.Windows.Controls.TextBlock)(this.FindName("ThingImageGalleryLabel")));
            this.ThingCreatedByLabel = ((System.Windows.Controls.TextBlock)(this.FindName("ThingCreatedByLabel")));
            this.ThingCreatedBy = ((System.Windows.Controls.TextBlock)(this.FindName("ThingCreatedBy")));
            this.ThingCreategOnLabel = ((System.Windows.Controls.TextBlock)(this.FindName("ThingCreategOnLabel")));
            this.ThingCreatedOn = ((System.Windows.Controls.TextBlock)(this.FindName("ThingCreatedOn")));
            this.DescriptionPanel = ((System.Windows.Controls.StackPanel)(this.FindName("DescriptionPanel")));
            this.ThingDescriptionLabel = ((System.Windows.Controls.TextBlock)(this.FindName("ThingDescriptionLabel")));
            this.ThingDescription = ((System.Windows.Controls.TextBlock)(this.FindName("ThingDescription")));
            this.InstructionsPanel = ((System.Windows.Controls.StackPanel)(this.FindName("InstructionsPanel")));
            this.ThingInstructionsLabel = ((System.Windows.Controls.TextBlock)(this.FindName("ThingInstructionsLabel")));
            this.ThingInstructions = ((System.Windows.Controls.TextBlock)(this.FindName("ThingInstructions")));
            this.FilesLabel = ((System.Windows.Controls.TextBlock)(this.FindName("FilesLabel")));
            this.FilesPanel = ((System.Windows.Controls.StackPanel)(this.FindName("FilesPanel")));
            this.thingDetailsProgressBar = ((System.Windows.Controls.ProgressBar)(this.FindName("thingDetailsProgressBar")));
        }
    }
}

