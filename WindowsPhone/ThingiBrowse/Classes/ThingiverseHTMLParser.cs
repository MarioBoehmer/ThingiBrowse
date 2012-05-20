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
    public class ThingiverseHTMLParser
    {
        private static String thingTag = "<div class=\"thing-float\">";
        private static String imageUrlTagStart = "<img src=\"";
        private static String urlStart = "<a href=\"";
        private static String urlTagEnd = "\"";
        private static String thingUrlTagStart = "<div class=\"thing-name\"><a href=\"";
        private static String textTagStart = "\">";
        private static String textTagEnd = "</a>";
        private static String paragraphTagStart = "<p>";
        private static String paragraphTagEnd = "</p>";
        private static String thingCreatorUrlTagStart = "Created by <a href=\"";
        private static String thingTimeTagStart = "<div class=\"thing-time\">";
        private static String divTagStart = "<div>";
        private static String divTagEnd = "</div>";
        private static String lastPageIndexstartTagOffset = "\"Next\"";
        private static String newThingsLastPageIndexstartTag = "<a href=\"/newest/page:";
        private static String popularThingsLastPageIndexstartTag = "<a href=\"/popular/page:";
        private static String thingDetailsTitleTagOffset = "<div id=\"thing-meta\">";
        private static String thingDetailsTitleTagStart = "<h1>";
        private static String thingDetailsTitleTagEnd = "</h1>";
        private static String thingDetailsCreatedByTagOffset = "<div class=\"byline\">";
        private static String thingDetailsCreatorImageUrlOffset = "<div id=\"thing-creator\">";
        private static String thingDetailsCreatedDateStart = "Created on ";
        private static String thingDetailsCreatedDateEnd = "</";
        private static String thingDetailsDescriptionTagStart = "<div id=\"thing-description\">";
        private static String thingDetailsImageUrlTagOffset = "<div id=\"thing-gallery-main\">";
        private static String thingDetailsInstructionsTagOffset = "<h4>Instructions</h4>";
        private static String thingDetailsFilesTagOffset = "<div class=\"thing-status\">";
        private static String thingDetailsFilesTagOffset2 = "<div class=\"thing-file\"";
        private static String thingTitleTagStart = "title=\"";
        private static String largeImageTagOffset = "<div class=\"main-content\"";

        public static List<ThingResultListItem> getThingResultListItems(String html)
        {
            List<ThingResultListItem> resultListItems = new List<ThingResultListItem>();
            List<int> indices = new List<int>();
            int startIndex = 0;
            while ((startIndex = html.IndexOf(thingTag, startIndex)) > 0)
            {
                startIndex = startIndex + thingTag.Length;
                indices.Add(startIndex);
            }
            for (int x = 0; x < indices.Count; x++)
            {
                String thingHtml;
                if (x + 1 < indices.Count)
                {
                    thingHtml = html.Substring(indices[x], indices[x + 1] - indices[x]);
                }
                else
                {
                    thingHtml = html.Substring(indices[x]);
                }
                resultListItems.Add(getResultListItemFromHtmlSnippet(thingHtml));

            }
            return resultListItems;
        }

        public static Thing getThing(String html)
        {
            Dictionary<String, String[]> ThingFiles = new Dictionary<String, String[]>();

            //ThingTitle
            String ThingTitle = getStringForTags(thingDetailsTitleTagStart, thingDetailsTitleTagEnd, getStringForTags(thingDetailsTitleTagOffset, null, html));

            //ThingCreatedBy
            String ThingCreatedBy = getStringForTags(textTagStart, textTagEnd, getStringForTags(thingDetailsCreatedByTagOffset, null, html));

            //ThingCreatorImageUrl
            String ThingCreatorImageUrl = getStringForTags(imageUrlTagStart, urlTagEnd, getStringForTags(thingDetailsCreatorImageUrlOffset, thingDetailsCreatedByTagOffset, html));

            //ThingCreatorUrl
            String ThingCreatorUrl = getStringForTags(urlStart, urlTagEnd, getStringForTags(thingDetailsCreatedByTagOffset, null, html));

            //ThingDate
            String ThingDate = getStringForTags(thingDetailsCreatedDateStart, thingDetailsCreatedDateEnd, html).Replace("\t", ""); 

            //ThingDescription
            String ThingDescription = HTMLToTextConverter.convertHtmlToText(getStringForTags(thingDetailsDescriptionTagStart, divTagEnd, html));

            //ThingImageUrl
            String ThingImageUrl = getStringForTags(imageUrlTagStart, urlTagEnd, getStringForTags(thingDetailsImageUrlTagOffset, null, html));

            //ThingLargeImageUrl
            String ThingLargeImageUrl = "http://www.thingiverse.com" + getStringForTags(urlStart, urlTagEnd, getStringForTags(thingDetailsImageUrlTagOffset, null, html));

            //ThingInstructions
            String ThingInstructions = HTMLToTextConverter.convertHtmlToText(getStringForTags(paragraphTagStart, paragraphTagEnd, getStringForTags(thingDetailsInstructionsTagOffset, null, html)));

            //ThingFiles
            int thingFileStartIndex = 0;
            while ((thingFileStartIndex = html.IndexOf(thingDetailsFilesTagOffset2, thingFileStartIndex)) > -1)
            {
                thingFileStartIndex += thingDetailsFilesTagOffset.Length;
                String thingFileSubstring = html.Substring(thingFileStartIndex);
                String fileUrl = "http://www.thingiverse.com" + getStringForTags(urlStart, urlTagEnd, thingFileSubstring);
                String fileName = getStringForTags(thingTitleTagStart, textTagStart, thingFileSubstring);
                String fileImageUrl = getStringForTags(imageUrlTagStart, urlTagEnd, thingFileSubstring);
                String fileSize = getStringForTags(divTagStart, divTagEnd, getStringForTags(textTagEnd, null, thingFileSubstring)).Replace("\n", "").Replace("\t", "");
                ThingFiles.Add(fileUrl, new String[]{fileName, fileSize, fileImageUrl});
            }

            return new Thing(ThingTitle, ThingCreatedBy, ThingCreatorImageUrl, ThingDate, ThingDescription, ThingCreatorUrl, ThingImageUrl, ThingLargeImageUrl, ThingInstructions, ThingFiles);
        }

        private static String getStringForTags(String startTag, String endTag, String html)
        {
            int startIndex = -1;
            int length = 0;
            if ((startIndex = html.IndexOf(startTag)) > -1)
            {
                startIndex += startTag.Length;
                if (endTag != null)
                {
                    length = html.IndexOf(endTag, startIndex) - startIndex;
                    return html.Substring(startIndex, length);
                }
                else
                {
                    return html.Substring(startIndex);
                }
            }
            return "";
        }

        public static int getNewThingsLastPageIndex(String html)
        {
            return Convert.ToInt32(getStringForTags(newThingsLastPageIndexstartTag, urlTagEnd, getStringForTags(lastPageIndexstartTagOffset, null, html)));
        }

        public static int getPopularThingsLastPageIndex(String html)
        {
            return Convert.ToInt32(getStringForTags(popularThingsLastPageIndexstartTag, urlTagEnd, getStringForTags(lastPageIndexstartTagOffset, null, html)));
        }

        private static ThingResultListItem getResultListItemFromHtmlSnippet(String htmlSnippet)
        {
            //thingImageUrl
            String thingImageUrl = getStringForTags(imageUrlTagStart, urlTagEnd, htmlSnippet);

            //thingUrl
            String thingUrl = getStringForTags(thingUrlTagStart, urlTagEnd, htmlSnippet);

            //thingTitle
            String thingTitle = getStringForTags(thingTitleTagStart, urlTagEnd, htmlSnippet);

            //thingCreatorUrl
            String thingCreatorUrl = getStringForTags(thingCreatorUrlTagStart, urlTagEnd, htmlSnippet);

            //thingCreatedBy
            String thingCreatedBy = "by " + getStringForTags(textTagStart, textTagEnd, getStringForTags(thingCreatorUrlTagStart, null, htmlSnippet));

            //thingTime
            String thingTime =  getStringForTags(thingTimeTagStart, divTagEnd, htmlSnippet).Replace("\t", "").Replace("\n", "");

            return new ThingResultListItem(thingTitle, thingCreatedBy, thingTime, thingUrl, thingCreatorUrl, thingImageUrl);
        }

        public static String getLargeImageUrl(String html)
        {
            return getStringForTags(imageUrlTagStart, urlTagEnd, getStringForTags(largeImageTagOffset, null, html));
        }
    }
}
