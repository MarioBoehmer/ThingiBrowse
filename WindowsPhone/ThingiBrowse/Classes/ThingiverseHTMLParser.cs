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
        private static String SEARCH_LAST_PAGE_INDEX_END_TAG = "?";
	private static String THING_TAG = "<div class=\"thing-float\">";
	private static String SEARCH_THING_TAG = "<td><a href=\"http://www.thingiverse.com/thing:";
	private static String IMAGE_URL_TAG_START = "<img src=\"";
	private static String URL_TAG_START = "<a href=\"";
	private static String URL_TAG_END = "\"";
	private static String THING_URL_TAG_START = "<div class=\"thing-name\"><a href=\"";
	private static String SEARCH_THING_URL_TAG_START = URL_TAG_START;
	private static String TEXT_TAG_START = "\">";
	private static String TEXT_TAG_END = "</a>";
	private static String PARAGRAPH_TAG_START = "<p>";
	private static String PARAGRAPH_TAG_END = "</p>";
	private static String THING_CREATOR_URL_TAG_START = "Created by <a href=\"";
	private static String THING_TIME_TAG_START = "<div class=\"thing-time\">";
	private static String SEARCH_THING_TIME_TAG_START = "Published: ";
	private static String SEARCH_THING_TIME_TAG_END = " on";
	private static String DIV_TAG_START = "<div>";
	private static String DIV_TAG_END = "</div>";
	private static String LAST_PAGE_INDEX_START_TAG_OFFSET = "\"Next\"";
	private static String THINGS_LAST_PAGE_INDEX_START_TAG = "/page:";
	private static String THING_DETAILS_TITLE_TAG_OFFSET = "<div id=\"thing-meta\">";
	private static String THING_DETAILS_TITLE_TAG_START = "<h1>";
	private static String THING_DETAILS_TITLE_TAG_END = "</h1>";
	private static String THING_DETAILS_CREATED_BY_TAG_OFFSET = "<div class=\"byline\">";
	private static String THING_DETAILS_CREATOR_IMAGE_URL_OFFSET = "<div id=\"thing-creator\">";
	private static String THING_DETAILS_CREATED_DATE_START = "Created on ";
	private static String THING_DETAILS_CREATED_DATE_END = "</";
	private static String THING_DETAILS_DESCRIPTION_TAG_START = "<div id=\"thing-description\">";
	private static String THING_DETAILS_IMAGE_URL_TAG_OFFSET = "<div id=\"thing-gallery-main\">";
	private static String THING_DETAILS_ADDITIONAL_IMAGE_URL_TAG_OFFSET = "<div class=\"thing-image-thumb\">";
	private static String THING_DETAILS_ADDITIONAL_IMAGE_URLS_TAG_START = "<div id=\"thing-gallery-thumbs\">";
	private static String THING_DETAILS_ADDITIONAL_IMAGE_URLS_TAG_END = "<div id=\"thing-info\">";
	private static String THING_DETAILS_INSTRUCTIONS_TAG_OFFSET = "<h4>Instructions</h4>";
	private static String THING_DETAILS_FILES_TAG_OFFSET = "<div class=\"thing-status\">";
	private static String THING_DETAILS_FILES_TAG_SECOND_OFFSET = "<div class=\"thing-file\"";
	private static String THING_TITLE_TAG_START = "title=\"";
	private static String LARGE_IMAGE_TAG_OFFSET = "<div class=\"main-content\"";
	private static String MEDIUM_IMAGE_TAG_OFFSET = "<b>card</b>";
	private static String THINGIVERSE_BASE_URL = "http://www.thingiverse.com";

        public static List<ThingResultListItem> getThingResultListItems(String html)
        {
            List<ThingResultListItem> resultListItems = new List<ThingResultListItem>();
            List<int> indices = new List<int>();
            int startIndex = 0;
            while ((startIndex = html.IndexOf(THING_TAG, startIndex)) > 0)
            {
                startIndex = startIndex + THING_TAG.Length;
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

        public static List<ThingResultListItem> getThingResultListItemsForSearch(String html)
        {
            List<ThingResultListItem> resultListItems = new List<ThingResultListItem>();
            List<int> indices = new List<int>();
            int startIndex = 0;
            while ((startIndex = html.IndexOf(SEARCH_THING_TAG, startIndex)) > 0)
            {
                startIndex = startIndex + SEARCH_THING_TAG.Length;
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
                resultListItems.Add(getResultListItemForSearchFromHtmlSnippet(thingHtml));

            }
            return resultListItems;
        }

        public static Thing getThing(String html)
        {
            Dictionary<String, String[]> thingFiles = new Dictionary<String, String[]>();
            List<String> thingAllImageUrls = new List<String>();

            // ThingTitle
            String thingTitle = getStringForTags(THING_DETAILS_TITLE_TAG_START,
                    THING_DETAILS_TITLE_TAG_END,
                    getStringForTags(THING_DETAILS_TITLE_TAG_OFFSET, null, html));

            // ThingCreatedBy
            String thingCreatedBy = getStringForTags(
                    TEXT_TAG_START,
                    TEXT_TAG_END,
                    getStringForTags(THING_DETAILS_CREATED_BY_TAG_OFFSET, null,
                            html));

            // ThingCreatorImageUrl
            String thingCreatorImageUrl = getStringForTags(
                    IMAGE_URL_TAG_START,
                    URL_TAG_END,
                    getStringForTags(THING_DETAILS_CREATOR_IMAGE_URL_OFFSET,
                            THING_DETAILS_CREATED_BY_TAG_OFFSET, html));

            // ThingCreatorUrl
            String thingCreatorUrl = adjustRelativeUrl(
                    getStringForTags(
                            URL_TAG_START,
                            URL_TAG_END,
                            getStringForTags(THING_DETAILS_CREATED_BY_TAG_OFFSET,
                                    null, html)), THINGIVERSE_BASE_URL);

            // ThingDate
            String thingDate = getStringForTags(THING_DETAILS_CREATED_DATE_START,
                    THING_DETAILS_CREATED_DATE_END, html).Replace("\t", "");

            // ThingDescription
            String thingDescription = HTMLToTextConverter.convertHtmlToText(getStringForTags(
                    THING_DETAILS_DESCRIPTION_TAG_START, DIV_TAG_END, html)
                    .Replace("\t", ""));

            // ThingImageUrl
            String thingImageUrl = getStringForTags(
                    IMAGE_URL_TAG_START,
                    URL_TAG_END,
                    getStringForTags(THING_DETAILS_IMAGE_URL_TAG_OFFSET, null, html));

            // ThingLargeImageUrl
            String thingLargeImageUrl = adjustRelativeUrl(
                    getStringForTags(
                            URL_TAG_START,
                            URL_TAG_END,
                            getStringForTags(THING_DETAILS_IMAGE_URL_TAG_OFFSET,
                                    null, html)), THINGIVERSE_BASE_URL);

            // ThingAllImageUrls
            thingAllImageUrls.Add(thingLargeImageUrl);
            int thingImageUrlsStartIndex = 0;
            String additionalImagesHtmlSnippet = getStringForTags(
                    THING_DETAILS_ADDITIONAL_IMAGE_URLS_TAG_START,
                    THING_DETAILS_ADDITIONAL_IMAGE_URLS_TAG_END, html);
            while ((thingImageUrlsStartIndex = additionalImagesHtmlSnippet.IndexOf(
                    THING_DETAILS_ADDITIONAL_IMAGE_URL_TAG_OFFSET,
                    thingImageUrlsStartIndex)) > -1)
            {
                thingImageUrlsStartIndex += THING_DETAILS_ADDITIONAL_IMAGE_URL_TAG_OFFSET
                        .Length;
                String thingAdditionalImageSubstring = additionalImagesHtmlSnippet
                        .Substring(thingImageUrlsStartIndex);
                String thingAdditionalImage = adjustRelativeUrl(
                        getStringForTags(URL_TAG_START, URL_TAG_END,
                                thingAdditionalImageSubstring),
                        THINGIVERSE_BASE_URL);
                thingAllImageUrls.Add(thingAdditionalImage);
            }

            // ThingInstructions
            String thingInstructions = HTMLToTextConverter.convertHtmlToText(getStringForTags(
                    PARAGRAPH_TAG_START,
                    PARAGRAPH_TAG_END,
                    getStringForTags(THING_DETAILS_INSTRUCTIONS_TAG_OFFSET, null,
                            html)).Replace("\t", ""));

            // ThingFiles
            int thingFileStartIndex = 0;
            while ((thingFileStartIndex = html.IndexOf(
                    THING_DETAILS_FILES_TAG_SECOND_OFFSET, thingFileStartIndex)) > -1)
            {
                thingFileStartIndex += THING_DETAILS_FILES_TAG_OFFSET.Length;
                String thingFileSubstring = html.Substring(thingFileStartIndex);
                String fileUrl = adjustRelativeUrl(
                        getStringForTags(URL_TAG_START, URL_TAG_END,
                                thingFileSubstring), THINGIVERSE_BASE_URL);
                String fileName = getStringForTags(THING_TITLE_TAG_START,
                        TEXT_TAG_START, thingFileSubstring);
                String fileImageUrl = getStringForTags(IMAGE_URL_TAG_START,
                        URL_TAG_END, thingFileSubstring);
                String fileSize = getStringForTags(DIV_TAG_START, DIV_TAG_END,
                        getStringForTags(TEXT_TAG_END, null, thingFileSubstring))
                        .Replace("\n", "").Replace("\t", "");
                thingFiles.Add(fileUrl, new String[] { fileName, fileSize,
					fileImageUrl });
            }

            return new Thing(thingTitle, thingCreatedBy, thingCreatorImageUrl,
                    thingDate, thingDescription, thingCreatorUrl, thingImageUrl,
                    thingLargeImageUrl, thingInstructions, thingFiles,
                    thingAllImageUrls);
        }

        private static String getStringForTags(String startTag, String endTag, String html)
        {
            int startIndex = -1;
            int endIndex = 0;
            if ((startIndex = html.IndexOf(startTag)) > -1)
            {
                startIndex += startTag.Length;
                if (endTag != null)
                {
                    endIndex = html.IndexOf(endTag, startIndex) - startIndex;
                    return html.Substring(startIndex, endIndex).Replace("[", "%5B");
                }
                else
                {
                    return html.Substring(startIndex).Replace("[", "%5B");
                }
            }
            return "";
        }

        public static int getThingsLastPageIndex(String html)
        {
            return Convert.ToInt32(getStringForTags(THINGS_LAST_PAGE_INDEX_START_TAG,
                            URL_TAG_END, getStringForTags(LAST_PAGE_INDEX_START_TAG_OFFSET,
                                    null, html)));
        }

        public static int getThingsLastPageIndexForSearch(String html)
        {
            return Convert.ToInt32(getStringForTags(THINGS_LAST_PAGE_INDEX_START_TAG,
                            SEARCH_LAST_PAGE_INDEX_END_TAG, getStringForTags(LAST_PAGE_INDEX_START_TAG_OFFSET,
                                    null, html)));
        }

        private static ThingResultListItem getResultListItemFromHtmlSnippet(
            String htmlSnippet)
        {
            // thingImageUrl
            String thingImageUrl = getStringForTags(IMAGE_URL_TAG_START,
                    URL_TAG_END, htmlSnippet);

            // thingUrl
            String thingUrl = adjustRelativeUrl(
                    getStringForTags(THING_URL_TAG_START, URL_TAG_END, htmlSnippet),
                    THINGIVERSE_BASE_URL);

            // thingTitle
            String thingTitle = getStringForTags(THING_TITLE_TAG_START,
                    URL_TAG_END, htmlSnippet);

            // thingCreatorUrl
            String thingCreatorUrl = adjustRelativeUrl(
                    getStringForTags(THING_CREATOR_URL_TAG_START, URL_TAG_END,
                            htmlSnippet), THINGIVERSE_BASE_URL);

            // thingCreatedBy
            String thingCreatedBy = "by "
                    + getStringForTags(
                            TEXT_TAG_START,
                            TEXT_TAG_END,
                            getStringForTags(THING_CREATOR_URL_TAG_START, null,
                                    htmlSnippet));

            // thingTime
            String thingTime = getStringForTags(THING_TIME_TAG_START, DIV_TAG_END,
                    htmlSnippet).Replace("\t", "").Replace("\n", "");

            return new ThingResultListItem(thingTitle, thingCreatedBy, thingTime,
                    thingUrl, thingCreatorUrl, thingImageUrl);
        }

        private static ThingResultListItem getResultListItemForSearchFromHtmlSnippet(
                String htmlSnippet)
        {
            // thingImageUrl
            String thingImageUrl = getStringForTags(IMAGE_URL_TAG_START,
                    URL_TAG_END, htmlSnippet);

            // thingUrl
            String thingUrl = adjustRelativeUrl(
                    getStringForTags(SEARCH_THING_URL_TAG_START, URL_TAG_END,
                            htmlSnippet), THINGIVERSE_BASE_URL);

            String thingTitleAndCreator = getStringForTags(thingUrl + "\">",
                    "</a>", htmlSnippet);

            String[] titleCreatorDivider = {" by "};
            String[] titleCreatorArray = thingTitleAndCreator
                    .Split(titleCreatorDivider, StringSplitOptions.None);

            // thingCreatedBy
            String thingCreatedBy = "";

            if (titleCreatorArray.Length == 2)
            {
                thingCreatedBy = "by " + titleCreatorArray[1];
            }

            // thingTitle
            String thingTitle = titleCreatorArray[0];

            // thingTime
            String thingTime = getStringForTags(SEARCH_THING_TIME_TAG_START,
                    SEARCH_THING_TIME_TAG_END, htmlSnippet);

            return new ThingResultListItem(thingTitle, thingCreatedBy, thingTime,
                    thingUrl, null, thingImageUrl);
        }

        public static String getLargeImageUrl(String html)
        {
            return getStringForTags(IMAGE_URL_TAG_START, URL_TAG_END,
                    getStringForTags(LARGE_IMAGE_TAG_OFFSET, null, html));
        }

        public static String getMediumImageUrl(String html)
        {
            return getStringForTags(IMAGE_URL_TAG_START, URL_TAG_END,
                    getStringForTags(MEDIUM_IMAGE_TAG_OFFSET, null, html));
        }

        private static String adjustRelativeUrl(String url, String baseUrl)
        {
            if (url.StartsWith("/"))
            {
                return baseUrl + url;
            }
            return url;
        }
    }
}
