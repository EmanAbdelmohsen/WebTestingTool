using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace WebTestingService
{
    public class BrokenLinkService : IBrokenLinkService
    {
        public List<string> GetBorkenLinksFromUrl(string url)
        {
            //get html document from given url
            HtmlWeb hw = new HtmlWeb();
            HtmlDocument doc = hw.Load(new Uri(url));

            //1- fetch all links from html
            var links = ExtractLinksFromHtmlDoc(doc);

            //2- iterate on links to get the status
            List<string> brokenLinks = new List<string>();

            foreach (string link in links)
            {
                int status = GetStatusCodeFromHttpResponse(link);

                //3- validate on status code to filter broken links
                if (status >= 400)
                {
                    brokenLinks.Add(link);
                }
            }

            return brokenLinks;
        }

        public List<string> GetBrokenLinksFromHtml(string htmlContent)
        {
            //get html document from text
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            //1- fetch all links from html
            var links = ExtractLinksFromHtmlDoc(doc);

            //2- iterate on links to get the status
            List<string> brokenLinks = new List<string>();

            foreach (string link in links)
            {
                int status = GetStatusCodeFromHttpResponse(link);

                //3- validate on status code to filter broken links
                if (status >= 400)
                {
                    brokenLinks.Add(link);
                }
            }

            return brokenLinks;
        }

        /// <summary>
        /// Gets all links from a given web page html source
        /// </summary>
        /// <param name="doc">html source document</param>
        /// <returns>list of links</returns>
        List<string> ExtractLinksFromHtmlDoc(HtmlDocument doc)
        {
            List<string> links = new List<string>();

            //fetch links nodes from the html document
            links = doc.DocumentNode
                       .SelectNodes("//a[@href]")
                       .Select(node => node.Attributes["href"].Value)
                       .ToList();

            return links;
        }

        /// <summary>
        /// Gets status code from an http response for a given url
        /// </summary>
        /// <param name="url">string url to make an http request for</param>
        /// <returns>status code</returns>
        int GetStatusCodeFromHttpResponse(string url)
        {
            //create http request out from the given url
            HttpWebRequest request;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }
            catch
            {
                return 404;
            }

            HttpWebResponse response = null;
            HttpStatusCode statusCode;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException we)
            {
                response = (HttpWebResponse)we.Response;
            }

            statusCode = response.StatusCode;

            return (int)statusCode;
        }
    }
}
