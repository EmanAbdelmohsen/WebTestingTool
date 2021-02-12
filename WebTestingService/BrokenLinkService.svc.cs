using System;
using System.Collections.Generic;

namespace WebTestingService
{
    public class BrokenLinkService : IBrokenLinkService
    {
        public List<string> GetBorkenLinksFromUrl(string url)
        {
            throw new NotImplementedException();
        }

        public List<string> GetBrokenLinksFromHtml(string htmlContent)
        {
            throw new NotImplementedException();
        }
    }
}
