using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace WebTestingService
{
    //reference: https://www.c-sharpcorner.com/uploadfile/f9935e/invoking-a-web-service-dynamically-using-system-net-and-soap/
    public static class DynamicServiceInvoker
    {
        /// <summary>
        /// empty SOAP envelope
        /// </summary>
        static string _soapEnvelope = @"<soap:Envelope
                                    xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'
                                    xmlns:xsd='http://www.w3.org/2001/XMLSchema'
                                    xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
                                <soap:Body></soap:Body></soap:Envelope>";

        /// <summary>
        /// Makes service synchronous invocation for a certain method and gets response
        /// </summary>
        /// <returns>http response</returns>
        public static string Invoke(this ServiceMethod method, string url, string contractName)
        {
            WebResponse response = null;
            string strResponse = "";

            //Create the request
            HttpWebRequest req = CreateWebRequest(url, contractName, method.Name);

            //write the soap envelope to request stream
            using (Stream stm = req.GetRequestStream())
            {
                using (StreamWriter stmw = new StreamWriter(stm))
                {
                    stmw.Write(CreateSoapEnvelope(method.Name, method.Parameters));
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
        static string CreateSoapEnvelope(string methodName, List<MethodParameter> parameters)
        {
            string MethodCall = "<" + methodName + @" xmlns=""http://tempuri.org/"">";
            string StrParameters = string.Empty;
            foreach (var param in parameters)
            {
                StrParameters = StrParameters + "<" + param.Name + ">" + param.Value + "</" + param.Name + ">";
            }
            MethodCall = MethodCall + StrParameters + "</" + methodName + ">";

            //insert the method name and parameters into the empty envelope
            StringBuilder sb = new StringBuilder(_soapEnvelope);
            sb.Insert(sb.ToString().IndexOf("</soap:Body>"), MethodCall);

            return sb.ToString();
        }

        /// <summary>
        /// Creates an http web request using the service url
        /// </summary>
        /// <returns></returns>
        static HttpWebRequest CreateWebRequest(string url, string contractName, string methodName)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

            //add SOAPAction to header
            webRequest.Headers.Add("SOAPAction", "\"http://tempuri.org/" + contractName + "/" + methodName + "\"");
            /*else
                webRequest.Headers.Add("SOAPAction", "\"http://tempuri.org/" + ServiceMethod + "\"");
            */

            webRequest.Headers.Add("To", url);
            webRequest.ContentType = "text/xml";
            webRequest.Accept = "text/html,application/xhtml+xml,application/xml;";
            webRequest.Method = "POST";

            return webRequest;
        }
    }



}
