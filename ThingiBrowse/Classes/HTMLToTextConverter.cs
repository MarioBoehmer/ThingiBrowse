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
using System.Text;

namespace ThingiBrowse
{
    public class HTMLToTextConverter
    {
        private static String urlStartTag = "<a";
        private static String urlEndTag = "</a>";
        private static String hyperlinkStartTag = "href=\"";
        private static String hyperlinkEndTag = "\"";

        public static String convertHtmlToText(String html)
        {
            StringBuilder textBuilder = new StringBuilder();
            int urlStartIndex = -1;
            int urlEndIndex = 0;
            String htmlSnippet = html;
            while ((urlStartIndex = htmlSnippet.IndexOf(urlStartTag)) > -1)
            {
                //append normal text
                String textInFrontOfHyperlink = htmlSnippet.Substring(0, urlStartIndex);
                textBuilder.Append(textInFrontOfHyperlink);
                //append hyperlink
                int hyperlinkStartIndex = htmlSnippet.IndexOf(hyperlinkStartTag, textInFrontOfHyperlink.Length) + hyperlinkStartTag.Length;
                int hyperlinkLength = htmlSnippet.IndexOf(hyperlinkEndTag, hyperlinkStartIndex) - hyperlinkStartIndex;
                textBuilder.Append(htmlSnippet.Substring(hyperlinkStartIndex, hyperlinkLength));
                urlEndIndex = htmlSnippet.IndexOf(urlEndTag) + urlEndTag.Length;
                htmlSnippet = htmlSnippet.Substring(urlEndIndex);
            }
            textBuilder.Append(htmlSnippet);
            return textBuilder.Replace("\t", "").Replace("\n", "").Replace("\r", "").Replace("<br />", "\n").ToString();
        }
    }
}
