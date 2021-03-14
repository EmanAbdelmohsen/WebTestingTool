using System.ServiceModel;
using WebTestingService.Utilities;

namespace WebTestingService
{
    [ServiceContract]
    public interface ILinkCheckingService
    {
        [OperationContract]
        LinkTypeEnum DefineLinkType(string link);

        [OperationContract]
        ServiceResult ValidateLinkStatus(string link, LinkTypeEnum linkType);
    }
}
