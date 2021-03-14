using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using WebTestingService.Utilities;

namespace WebTestingService
{
    //reference: https://www.c-sharpcorner.com/uploadfile/f9935e/invoking-a-web-service-dynamically-using-system-net-and-soap/
    public class SoapServiceClient
    {
        public string Url { get; set; }
        public LinkTypeEnum ServiceType { get; set; }
        public string ServiceMethod { get; set; }
        public List<MethodParameter> MethodParameters { get; set; }
        public string WCFContractName { get; set; }

        /// <summary>
        /// empty SOAP envelope
        /// </summary>
        string _soapEnvelope = @"<soap:Envelope
                                    xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'
                                    xmlns:xsd='http://www.w3.org/2001/XMLSchema'
                                    xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
                                <soap:Body></soap:Body></soap:Envelope>";

        /// <summary>
        /// Makes service synchronous invocation for a certain method and gets response
        /// </summary>
        /// <returns>http response</returns>
        public string InvokeService()
        {
            WebResponse response = null;
            string strResponse = "";

            //Create the request
            HttpWebRequest req = CreateWebRequest();

            //write the soap envelope to request stream
            using (Stream stm = req.GetRequestStream())
            {
                using (StreamWriter stmw = new StreamWriter(stm))
                {
                    stmw.Write(CreateSoapEnvelope());
                }
            }

            //get the response from the web service
            response = req.GetResponse();
            Stream str = response.GetResponseStream();
            StreamReader sr = new StreamReader(str);

            strResponse = sr.ReadToEnd();
            return strResponse;
        }

        /// <summary>
        /// Creates an envelope for the SOAP service call using the method name and parameters
        /// </summary>
        /// <returns>SOAP envelope</returns>
        string CreateSoapEnvelope()
        {
            string MethodCall = "<" + ServiceMethod + @" xmlns=""http://tempuri.org/"">";
            string StrParameters = string.Empty;
            foreach (var param in MethodParameters)
            {
                StrParameters = StrParameters + "<" + param.Name + ">" + param.Value + "</" + param.Name + ">";
            }
            MethodCall = MethodCall + StrParameters + "</" + ServiceMethod + ">";

            //insert the method name and parameters into the empty envelope
            StringBuilder sb = new StringBuilder(_soapEnvelope);
            sb.Insert(sb.ToString().IndexOf("</soap:Body>"), MethodCall);

            return sb.ToString();
        }

        /// <summary>
        /// Creates an http web request using the service url
        /// </summary>
        /// <returns></returns>
        HttpWebRequest CreateWebRequest()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Url);

            //add SOAPAction to header
            if (ServiceType == LinkTypeEnum.WCF_SOAP_Endpoint)
                webRequest.Headers.Add("SOAPAction", "\"http://tempuri.org/" + WCFContractName + "/" + ServiceMethod + "\"");
            else
                webRequest.Headers.Add("SOAPAction", "\"http://tempuri.org/" + ServiceMethod + "\"");

            webRequest.Headers.Add("To", Url);
            webRequest.ContentType = "text/xml";
            webRequest.Accept = "text/html,application/xhtml+xml,application/xml;";
            webRequest.Method = "POST";

            return webRequest;
        }
    }



}
