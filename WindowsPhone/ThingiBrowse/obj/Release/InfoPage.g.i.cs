﻿#pragma checksum "C:\Dev\IDE\Git_Workspace\ThingiBrowse\WindowsPhone\ThingiBrowse\InfoPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "45C10AC230A4017E13B6A4058D318D6B"
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
    
    
    public partial class InfoPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBlock InfoTitle;
        
        internal System.Windows.Controls.TextBlock InfoText;
        
        internal System.Windows.Controls.TextBlock LicenseTitle;
        
        internal System.Windows.Controls.TextBlock LicenseText;
        
        internal System.Windows.Controls.TextBlock SourceTitle;
        
        internal System.Windows.Controls.TextBlock SourceText;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/ThingiBrowse;component/InfoPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.InfoTitle = ((System.Windows.Controls.TextBlock)(this.FindName("InfoTitle")));
            this.InfoText = ((System.Windows.Controls.TextBlock)(this.FindName("InfoText")));
            this.LicenseTitle = ((System.Windows.Controls.TextBlock)(this.FindName("LicenseTitle")));
            this.LicenseText = ((System.Windows.Controls.TextBlock)(this.FindName("LicenseText")));
            this.SourceTitle = ((System.Windows.Controls.TextBlock)(this.FindName("SourceTitle")));
            this.SourceText = ((System.Windows.Controls.TextBlock)(this.FindName("SourceText")));
        }
    }
}

