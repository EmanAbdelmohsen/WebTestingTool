using System.Collections.Generic;
using System.ServiceModel;
using WebTestingService.Utilities;

namespace WebTestingService
{
    [ServiceContract]
    public interface IWebPageLinkCheckingService
    {
        [OperationContract]
        ServiceResult<List<string>> GetAllLinksFromPageUrl(string url);

        [OperationContract]
        ServiceResult<List<string>> GetAllBrokenLinksFromPageUrl(string url);
    }
}
