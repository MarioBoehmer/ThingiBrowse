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
using Microsoft.Phone.Controls;
using System.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using System.Windows.Navigation;

namespace ThingiBrowse
{
    public partial class MainPage : PhoneApplicationPage
    {

        bool alreadyHookedScrollEvents = false;

        public ObservableCollection<ThingResultListItem> FeaturedThingsItems { get; private set; }
        public ObservableCollection<ThingResultListItem> NewestDerivativesThingsItems { get; private set; }
        public ObservableCollection<ThingResultListItem> PopularThingsItems { get; private set; }
        public ObservableCollection<ThingResultListItem> NewThingsItems { get; private set; }

        public int FeaturedThingsLastPageIndex { get; private set; }
        public int NewestDerivativesThingsLastPageIndex { get; private set; }
        public int PopularThingsLastPageIndex { get; private set; }
        public int NewThingsLastPageIndex { get; private set; }

        public int FeaturedThingsCurrentPageIndex { get; set; }
        public int NewestDerivativesThingsCurrentPageIndex { get; set; }
        public int PopularThingsCurrentPageIndex { get; set; }
        public int NewThingsCurrentPageIndex { get; set; }
        
        bool isMainPageLoaded = false;

        private const int FEATURED_THINGS_PAGE = 1;
        private const int NEWEST_DERIVATIVES_THINGS_PAGE = 2;
        private const int POPULAR_THINGS_PAGE = 3;
        private const int NEW_THINGS_PAGE = 4;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            FeaturedThingsItems = new ObservableCollection<ThingResultListItem>();
            NewestDerivativesThingsItems = new ObservableCollection<ThingResultListItem>();
            PopularThingsItems = new ObservableCollection<ThingResultListItem>();
            NewThingsItems = new ObservableCollection<ThingResultListItem>();
            
            DataContext = this;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void Thing_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            ThingResultListItem resultListItem = b.DataContext as ThingResultListItem;
            // Navigate to the new page
            NavigationService.Navigate(new Uri("/DetailsPage.xaml?selectedItem=" + resultListItem.ThingUrl, UriKind.Relative));
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isMainPageLoaded)
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    reloadThingResults();
                    
                    isMainPageLoaded = true;
                }
                else
                {
                    FeaturedThingsProgressBar.Visibility = Visibility.Collapsed;
                    NewestDerivativesThingsProgressBar.Visibility = Visibility.Collapsed;
                    PopularThingsProgressBar.Visibility = Visibility.Collapsed;
                    NewThingsProgressBar.Visibility = Visibility.Collapsed;
                   
                    showNetworkErrorMessage();
                }
            }

            if (alreadyHookedScrollEvents)
                return;

            alreadyHookedScrollEvents = true;

            ScrollViewer featuredThingsScrollViewer = FindSimpleVisualChild<ScrollViewer>(FeaturedThingsResultList);
            ScrollViewer newestDerivativesThingsScrollViewer = FindSimpleVisualChild<ScrollViewer>(NewestDerivativesThingsResultList);
            ScrollViewer popularThingsScrollViewer = FindSimpleVisualChild<ScrollViewer>(PopularThingsResultList);
            ScrollViewer newThingsScrollViewer = FindSimpleVisualChild<ScrollViewer>(NewThingsResultList);

            featuredThingsScrollViewer.Tag = FEATURED_THINGS_PAGE;
            newestDerivativesThingsScrollViewer.Tag = NEWEST_DERIVATIVES_THINGS_PAGE;
            popularThingsScrollViewer.Tag = POPULAR_THINGS_PAGE;
            newThingsScrollViewer.Tag = NEW_THINGS_PAGE;

            setUpScrollViewer(featuredThingsScrollViewer, new EventHandler<VisualStateChangedEventArgs>(group_CurrentStateChanged_ThingsResultList));
            setUpScrollViewer(newestDerivativesThingsScrollViewer, new EventHandler<VisualStateChangedEventArgs>(group_CurrentStateChanged_ThingsResultList));
            setUpScrollViewer(popularThingsScrollViewer, new EventHandler<VisualStateChangedEventArgs>(group_CurrentStateChanged_ThingsResultList));
            setUpScrollViewer(newThingsScrollViewer, new EventHandler<VisualStateChangedEventArgs>(group_CurrentStateChanged_ThingsResultList));
        }

        private void setUpScrollViewer(ScrollViewer scrollViewer, EventHandler<VisualStateChangedEventArgs> eventHandler)
        {
            if (scrollViewer != null)
            {
                // Visual States are always on the first child of the control template  
                FrameworkElement element = VisualTreeHelper.GetChild(scrollViewer, 0) as FrameworkElement;
                if (element != null)
                {
                    VisualStateGroup group = FindVisualState(element, "ScrollStates");
                    if (group != null)
                    {
                        group.CurrentStateChanged += eventHandler;
                    }
                }
            }
        }

        VisualStateGroup FindVisualState(FrameworkElement element, string name)
        {
            if (element == null)
                return null;

            System.Collections.IList groups = VisualStateManager.GetVisualStateGroups(element);
            foreach (VisualStateGroup group in groups)
                if (group.Name == name)
                    return group;

            return null;
        }

        T FindSimpleVisualChild<T>(DependencyObject element) where T : class
        {
            while (element != null)
            {

                if (element is T)
                    return element as T;

                element = VisualTreeHelper.GetChild(element, 0);
            }

            return null;
        }

        void group_CurrentStateChanged_ThingsResultList(object sender, VisualStateChangedEventArgs e)
        {
            int thingType = (int)e.Control.Tag;
            switch (thingType)
            {
                case FEATURED_THINGS_PAGE: group_CurrentStateChanged_Things(sender, e, FeaturedThingsResultList, FeaturedThingsProgressBar, FEATURED_THINGS_PAGE);
                    break;
                case NEWEST_DERIVATIVES_THINGS_PAGE: group_CurrentStateChanged_Things(sender, e, NewestDerivativesThingsResultList, NewestDerivativesThingsProgressBar, NEWEST_DERIVATIVES_THINGS_PAGE);
                    break;
                case POPULAR_THINGS_PAGE: group_CurrentStateChanged_Things(sender, e, PopularThingsResultList, PopularThingsProgressBar, POPULAR_THINGS_PAGE);
                    break;
                case NEW_THINGS_PAGE: group_CurrentStateChanged_Things(sender, e, NewThingsResultList, NewThingsProgressBar, NEW_THINGS_PAGE);
                    break;
            }
            
        }

        private void group_CurrentStateChanged_Things(object sender, VisualStateChangedEventArgs e, ListBox thingListBox, ProgressBar progressBar, int thingType)
        {
            if (e.NewState.Name == "NotScrolling")
            {
                ListBoxItem lastitem = thingListBox.ItemContainerGenerator.ContainerFromItem(thingListBox.Items[thingListBox.Items.Count - 1]) as ListBoxItem;
                if (lastitem != null)
                {
                    GeneralTransform transform = lastitem.TransformToVisual(thingListBox);
                    Point childToParentCoordinates = transform.Transform(new Point(0, 0));
                    bool lastInView = childToParentCoordinates.Y < thingListBox.ActualHeight;
                    if (lastInView)
                    {
                        progressBar.Visibility = Visibility.Visible;
                        switch (thingType)
                        {
                            case FEATURED_THINGS_PAGE: loadData(true, FEATURED_THINGS_PAGE, FeaturedThingsCurrentPageIndex, FeaturedThingsLastPageIndex);
                                break;
                            case NEWEST_DERIVATIVES_THINGS_PAGE: loadData(true, NEWEST_DERIVATIVES_THINGS_PAGE, NewestDerivativesThingsCurrentPageIndex, NewestDerivativesThingsLastPageIndex);
                                break;
                            case POPULAR_THINGS_PAGE: loadData(true, POPULAR_THINGS_PAGE, PopularThingsCurrentPageIndex, PopularThingsLastPageIndex);
                                break;
                            case NEW_THINGS_PAGE: loadData(true, NEW_THINGS_PAGE, NewThingsCurrentPageIndex, NewThingsLastPageIndex);
                                break;
                        }
                    }
                }
            }
        }

        private void showNetworkErrorMessage()
        {
            MessageBox.Show(Strings.network_error_message, Strings.network_error_title, MessageBoxButton.OK);
        }

        private void showResponseErrorMessage()
        {
            MessageBox.Show(Strings.response_error_message, Strings.response_error_title, MessageBoxButton.OK);
        }

        private void loadData(bool isPaging, int thingPageType, int currentPage, int lastPageIndex)
        {
            int pageIndex = 1;
            WebClient webClient = new WebClient();
            
            if (isPaging)
            {
                pageIndex = currentPage + 1;
                if (pageIndex > lastPageIndex)
                {
                    return;
                }
            }
            switch (thingPageType)
            {
                case FEATURED_THINGS_PAGE: FeaturedThingsCurrentPageIndex = pageIndex;
                    webClient.DownloadStringAsync(new Uri("http://www.thingiverse.com/featured/page:" + pageIndex), thingPageType);
                    break;
                case NEWEST_DERIVATIVES_THINGS_PAGE: NewestDerivativesThingsCurrentPageIndex = pageIndex;
                    webClient.DownloadStringAsync(new Uri("http://www.thingiverse.com/derivatives/page:" + pageIndex), thingPageType);
                    break;
                case POPULAR_THINGS_PAGE: PopularThingsCurrentPageIndex = pageIndex;
                    webClient.DownloadStringAsync(new Uri("http://www.thingiverse.com/popular/page:" + pageIndex), thingPageType);
                    break;
                case NEW_THINGS_PAGE: NewThingsCurrentPageIndex = pageIndex;
                    webClient.DownloadStringAsync(new Uri("http://www.thingiverse.com/newest/page:" + pageIndex), thingPageType);
                    break;
            }
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(ThingResultsDownloadStringCompleted);
        }

        void ThingResultsDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            int thingPageType = (int)e.UserState;
            switch (thingPageType)
            {
                case FEATURED_THINGS_PAGE: ThingsDownloadStringCompleted(sender, e, FeaturedThingsItems, FeaturedThingsResultList, FEATURED_THINGS_PAGE, FeaturedThingsProgressBar, FeaturedThingsCurrentPageIndex);
                    break;
                case NEWEST_DERIVATIVES_THINGS_PAGE: ThingsDownloadStringCompleted(sender, e, NewestDerivativesThingsItems, NewestDerivativesThingsResultList, NEWEST_DERIVATIVES_THINGS_PAGE, NewestDerivativesThingsProgressBar, NewestDerivativesThingsCurrentPageIndex);
                    break;
                case POPULAR_THINGS_PAGE: ThingsDownloadStringCompleted(sender, e, PopularThingsItems, PopularThingsResultList, POPULAR_THINGS_PAGE, PopularThingsProgressBar, PopularThingsCurrentPageIndex);
                    break;
                case NEW_THINGS_PAGE: ThingsDownloadStringCompleted(sender, e, NewThingsItems, NewThingsResultList, NEW_THINGS_PAGE, NewThingsProgressBar, NewThingsCurrentPageIndex);
                    break;
            }
        }

        private void ThingsDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e, ObservableCollection<ThingResultListItem> resultListThingItems, ListBox listBox, int thingType, ProgressBar progressBar, int currentPageIndex)
        {
            lock (this)
            {
                if (e.Error == null)
                {
                    try
                    {
                        List<ThingResultListItem> resultListItems = ThingiverseHTMLParser.getThingResultListItems(e.Result);
                        if (currentPageIndex == 1)
                        {
                            //Scroll ListBox up before updating and clear it
                            var listBoxThingsEnumerator = resultListThingItems.GetEnumerator();
                            if (listBoxThingsEnumerator.MoveNext())
                                listBox.ScrollIntoView(listBoxThingsEnumerator.Current);
                            resultListThingItems.Clear();
                        }

                        int lastPageIndex = ThingiverseHTMLParser.getThingsLastPageIndex(e.Result);
                        switch (thingType)
                        { 
                            case FEATURED_THINGS_PAGE: FeaturedThingsLastPageIndex = lastPageIndex;
                                break;
                            case NEWEST_DERIVATIVES_THINGS_PAGE: NewestDerivativesThingsLastPageIndex = lastPageIndex;
                                break;
                            case POPULAR_THINGS_PAGE: PopularThingsLastPageIndex = lastPageIndex;
                                break;
                            case NEW_THINGS_PAGE: NewThingsLastPageIndex = lastPageIndex;
                                break;
                        }
                        
                        foreach (ThingResultListItem resultListItem in resultListItems)
                        {
                            resultListThingItems.Add(resultListItem);
                        }
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
                    progressBar.Visibility = Visibility.Collapsed;
            }
        }

        private void addApplicationBarButtonRefresh()
        {
            ApplicationBarIconButton refresh = new ApplicationBarIconButton(new Uri("/Images/appbar.refresh.rest.png", UriKind.Relative));
            refresh.Text = Strings.refresh;
            refresh.Click += (s, e) =>
            {
                reloadThingResults(); 
            };
            ApplicationBar.Buttons.Insert(0, refresh);
        }

        private void reloadThingResults()
        {
            FeaturedThingsCurrentPageIndex = 1;
            NewestDerivativesThingsCurrentPageIndex = 1;
            PopularThingsCurrentPageIndex = 1;
            NewThingsCurrentPageIndex = 1;

            FeaturedThingsLastPageIndex = 0;
            NewestDerivativesThingsLastPageIndex = 0;
            PopularThingsLastPageIndex = 0;
            NewThingsLastPageIndex = 0;

            FeaturedThingsProgressBar.Visibility = Visibility.Visible;
            NewestDerivativesThingsProgressBar.Visibility = Visibility.Visible;
            PopularThingsProgressBar.Visibility = Visibility.Visible;
            NewThingsProgressBar.Visibility = Visibility.Visible;

            loadData(false, FEATURED_THINGS_PAGE, FeaturedThingsCurrentPageIndex, FeaturedThingsLastPageIndex);
            loadData(false, NEWEST_DERIVATIVES_THINGS_PAGE, NewestDerivativesThingsCurrentPageIndex, NewestDerivativesThingsLastPageIndex);
            loadData(false, POPULAR_THINGS_PAGE, PopularThingsCurrentPageIndex, PopularThingsLastPageIndex);
            loadData(false, NEW_THINGS_PAGE, NewThingsCurrentPageIndex, NewThingsLastPageIndex);
        }

        private void removeApplicationBarButtonRefresh()
        {
            ApplicationBar.Buttons.RemoveAt(0);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            addApplicationBarButtonRefresh();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            removeApplicationBarButtonRefresh();
        }

        private void MainPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if (e.Orientation.ToString().Contains("Portrait"))
            {
                FeaturedThingsResultList.ItemTemplate = (DataTemplate)Resources["listItemPortrait"];
                NewestDerivativesThingsResultList.ItemTemplate = (DataTemplate)Resources["listItemPortrait"];
                PopularThingsResultList.ItemTemplate = (DataTemplate)Resources["listItemPortrait"];
                NewThingsResultList.ItemTemplate = (DataTemplate)Resources["listItemPortrait"];
            }
            else
            {
                FeaturedThingsResultList.ItemTemplate = (DataTemplate)Resources["listItemLandscape"];
                NewestDerivativesThingsResultList.ItemTemplate = (DataTemplate)Resources["listItemLandscape"];
                PopularThingsResultList.ItemTemplate = (DataTemplate)Resources["listItemLandscape"];
                NewThingsResultList.ItemTemplate = (DataTemplate)Resources["listItemLandscape"];
            }
            refreshItemsListsAfterOrientationChange();
        }

        //This needs to be done to apply the new DataTemplates
        private void refreshItemsListsAfterOrientationChange()
        {
            FeaturedThingsResultList.ItemsSource = null;
            NewestDerivativesThingsResultList.ItemsSource = null;
            PopularThingsResultList.ItemsSource = null;
            NewThingsResultList.ItemsSource = null;

            FeaturedThingsResultList.ItemsSource = FeaturedThingsItems;
            NewestDerivativesThingsResultList.ItemsSource = NewestDerivativesThingsItems;
            PopularThingsResultList.ItemsSource = PopularThingsItems;
            NewThingsResultList.ItemsSource = NewThingsItems;
        }
    }
}