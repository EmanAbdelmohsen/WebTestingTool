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
            List<string> brokenLinks = new List<string>();

            //determine if the url is a web page or a wcf service
            if (url.Contains(".svc"))
            {
                //connect to a wcf service using SOAP 

                //create SOAP envelope
            }

            else
            {
                //get html document from given url
                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(new Uri(url));

                //1- fetch all links from html
                var links = ExtractLinksFromHtmlDoc(doc);

                //2- iterate on links to get the status
                foreach (string link in links)
                {
                    int status = GetStatusCodeFromHttpResponse(link);

                    //3- validate on status code to filter broken links
                    if (status >= 400)
                    {
                        brokenLinks.Add(link);
                    }
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

        public string invoke(string url)
        {
            /*Uri mexAddress = new Uri("http://neptune.fulton.ad.asu.edu/WSRepository/Services/BasicThreeSvc/Service.svc?wsdl");
            // For MEX endpoints use a MEX address and a
            // mexMode of .MetadataExchange
            MetadataExchangeClientMode mexMode = MetadataExchangeClientMode.HttpGet;
            string contractName = "IService";
            string operationName = "Add";
            object[] operationParameters = new object[] { 1, 2 };

            // Get the metadata file from the service.
            MetadataExchangeClient mexClient = new MetadataExchangeClient(mexAddress, mexMode);
            mexClient.ResolveMetadataReferences = true;
            MetadataSet metaSet = mexClient.GetMetadata();

            // Import all contracts and endpoints
            WsdlImporter importer = new WsdlImporter(metaSet);
            Collection<ContractDescription> contracts = importer.ImportAllContracts();
            ServiceEndpointCollection allEndpoints = importer.ImportAllEndpoints();

            // Generate type information for each contract
            ServiceContractGenerator generator = new ServiceContractGenerator();
            var endpointsForContracts = new Dictionary<string, IEnumerable<ServiceEndpoint>>();

            foreach (ContractDescription contract in contracts)
            {
                generator.GenerateServiceContractType(contract);
                // Keep a list of each contract's endpoints
                endpointsForContracts[contract.Name] = allEndpoints.Where(
                     se => se.Contract.Name == contract.Name).ToList();
            }
            return "";
            */

            /*var ser = new SoapServiceClient
                       {
                           Url = url,
                           ServiceMethod = "",
                           MethodParameters = new List<MethodParameter> { },// { new MethodParameter { Name = "value", Value = "2" } },
                           ServiceType = ServiceTypeEnum.WCF,
                           WCFContractName = ""
                       };
                       return ser.InvokeService();
                       */

            /*var ser = new ServiceDetail
            {
                ContractName = "IService",
                WSDLUri = new Uri("http://neptune.fulton.ad.asu.edu/WSRepository/Services/BasicThreeSvc/Service.svc?wsdl"),
                ServiceUri = new Uri("http://neptune.fulton.ad.asu.edu/WSRepository/Services/BasicThreeSvc/Service.svc"),
                MethodName = "PiValue"
            };
            GenericService s = new GenericService();
            s.Call(ser, new List<object>());
            return "";*/

            var ins = new MyServiceClient();
            ins.Invoke("http://neptune.fulton.ad.asu.edu/WSRepository/Services/BasicThreeSvc/Service.svc?wsdl");
            return "";
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
