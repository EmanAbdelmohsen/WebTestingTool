using System.Collections.Generic;
using System.ServiceModel;

namespace WebTestingService
{
    [ServiceContract]
    public interface IBrokenLinkService
    {
        [OperationContract]
        //[WebGet]
        string invoke(string url);

        [OperationContract]
        //[WebGet]
        List<string> GetBorkenLinksFromUrl(string url);

        [OperationContract]
        //[WebGet]
        List<string> GetBrokenLinksFromHtml(string htmlContent);
    }
}
