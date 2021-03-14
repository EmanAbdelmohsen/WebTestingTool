using System.Collections.Generic;
using WebTestingService.Utilities;

namespace WebTestingService
{
    public class WebPageLinkCheckingService : IWebPageLinkCheckingService
    {
        public ServiceResult<List<string>> GetAllLinksFromPageUrl(string url)
        {
            throw new System.NotImplementedException();
        }

        public ServiceResult<List<string>> GetAllBrokenLinksFromPageUrl(string url)
        {
            throw new System.NotImplementedException();
        }
    }
}
