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
        public ObservableCollection<ThingResultListItem> NewThingsItems { get; private set; }
        public int NewThingsLastPageIndex { get; private set; }
        public int NewThingsCurrentPageIndex { get; set; }
        public ObservableCollection<ThingResultListItem> PopularThingsItems { get; private set; }
        public int PopularThingsLastPageIndex { get; private set; }
        public int PopularThingsCurrentPageIndex { get; set; }
        bool isMainPageLoaded = false;
        private static int NEW_THINGS_PAGE = 1;
        private static int POPULAR_THINGS_PAGE = 2;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            NewThingsItems = new ObservableCollection<ThingResultListItem>();
            PopularThingsItems = new ObservableCollection<ThingResultListItem>();
            NewThingsCurrentPageIndex = 1;
            PopularThingsCurrentPageIndex = 1;
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
                    NewThingsProgressBar.VerticalAlignment = VerticalAlignment.Center;
                    PopularThingsProgressBar.VerticalAlignment = VerticalAlignment.Center;
                    NewThingsProgressBar.Visibility = Visibility.Visible;
                    PopularThingsProgressBar.Visibility = Visibility.Visible;
                    loadData(false, NEW_THINGS_PAGE, NewThingsCurrentPageIndex, NewThingsLastPageIndex);
                    loadData(false, POPULAR_THINGS_PAGE, PopularThingsCurrentPageIndex, PopularThingsLastPageIndex);
                    isMainPageLoaded = true;
                }
                else
                {
                    NewThingsProgressBar.Visibility = Visibility.Collapsed;
                    PopularThingsProgressBar.Visibility = Visibility.Collapsed;
                    showNetworkErrorMessage();
                }
            }

            if (alreadyHookedScrollEvents)
                return;

            alreadyHookedScrollEvents = true;
            ScrollViewer newThingsScrollViewer = FindSimpleVisualChild<ScrollViewer>(NewThingsResultList);
            setUpScrollViewer(newThingsScrollViewer, new EventHandler<VisualStateChangedEventArgs>(group_CurrentStateChanged_NewThings));
            ScrollViewer popularThingsScrollViewer = FindSimpleVisualChild<ScrollViewer>(PopularThingsResultList);
            setUpScrollViewer(popularThingsScrollViewer, new EventHandler<VisualStateChangedEventArgs>(group_CurrentStateChanged_PopularThings));
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

        void group_CurrentStateChanged_NewThings(object sender, VisualStateChangedEventArgs e)
        {
            group_CurrentStateChanged_Things(sender, e, NewThingsResultList, NewThingsProgressBar, NEW_THINGS_PAGE);
        }

        void group_CurrentStateChanged_PopularThings(object sender, VisualStateChangedEventArgs e)
        {
            group_CurrentStateChanged_Things(sender, e, PopularThingsResultList, PopularThingsProgressBar, POPULAR_THINGS_PAGE);
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
                        if (thingType == NEW_THINGS_PAGE)
                        {
                            loadData(true, NEW_THINGS_PAGE, NewThingsCurrentPageIndex, NewThingsLastPageIndex);
                        }
                        else if (thingType == POPULAR_THINGS_PAGE)
                        {
                            loadData(true, POPULAR_THINGS_PAGE, PopularThingsCurrentPageIndex, PopularThingsLastPageIndex);
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
            if (thingPageType == NEW_THINGS_PAGE)
            {
                NewThingsCurrentPageIndex = pageIndex;
                webClient.DownloadStringAsync(new Uri("http://www.thingiverse.com/newest/page:" + pageIndex));
                webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(NewThingsDownloadStringCompleted);
            }
            else if (thingPageType == POPULAR_THINGS_PAGE)
            {
                PopularThingsCurrentPageIndex = pageIndex;
                webClient.DownloadStringAsync(new Uri("http://www.thingiverse.com/popular/page:" + pageIndex));
                webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(PopularThingsDownloadStringCompleted);
            }
        }

        void NewThingsDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            ThingsDownloadStringCompleted(sender, e, NewThingsItems, NewThingsResultList, NEW_THINGS_PAGE, NewThingsProgressBar, NewThingsCurrentPageIndex);
        }

        void PopularThingsDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            ThingsDownloadStringCompleted(sender, e, PopularThingsItems, PopularThingsResultList, POPULAR_THINGS_PAGE, PopularThingsProgressBar, PopularThingsCurrentPageIndex);
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
                        if (thingType == NEW_THINGS_PAGE)
                        {
                            NewThingsLastPageIndex = ThingiverseHTMLParser.getNewThingsLastPageIndex(e.Result);
                        }
                        else if (thingType == POPULAR_THINGS_PAGE)
                        {
                            PopularThingsLastPageIndex = ThingiverseHTMLParser.getPopularThingsLastPageIndex(e.Result);
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
                NewThingsCurrentPageIndex = 1;
                NewThingsLastPageIndex = 0;
                PopularThingsCurrentPageIndex = 1;
                PopularThingsLastPageIndex = 0;
                NewThingsProgressBar.Visibility = Visibility.Visible;
                PopularThingsProgressBar.Visibility = Visibility.Visible;
                loadData(false, NEW_THINGS_PAGE, NewThingsCurrentPageIndex, NewThingsLastPageIndex);
                loadData(false, POPULAR_THINGS_PAGE, PopularThingsCurrentPageIndex, PopularThingsLastPageIndex);
            };
            ApplicationBar.Buttons.Insert(0, refresh);
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
                NewThingsResultList.ItemTemplate = (DataTemplate)Resources["listItemPortrait"];
                PopularThingsResultList.ItemTemplate = (DataTemplate)Resources["listItemPortrait"];
            }
            else
            {
                NewThingsResultList.ItemTemplate = (DataTemplate)Resources["listItemLandscape"];
                PopularThingsResultList.ItemTemplate = (DataTemplate)Resources["listItemLandscape"];
            }
            refreshItemsListsAfterOrientationChange();
        }

        //This needs to be done to apply the new DataTemplates
        private void refreshItemsListsAfterOrientationChange()
        {
            NewThingsResultList.ItemsSource = null;
            NewThingsResultList.ItemsSource = NewThingsItems;
            PopularThingsResultList.ItemsSource = null;
            PopularThingsResultList.ItemsSource = PopularThingsItems;
        }
    }
}