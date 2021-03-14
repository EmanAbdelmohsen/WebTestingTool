using System.Collections.Generic;
using System.ServiceModel;
using WebTestingService.Utilities;

namespace WebTestingService
{
    [ServiceContract]
    public interface ISoapEndpointCheckingService
    {
        [OperationContract]
        ServiceResult<List<string>> GetAllServiceContracts(string serviceUrl);

        [OperationContract]
        ServiceResult<List<ServiceMethod>> GetAllContactMethods(string serviceUrl, string contractName);

        [OperationContract]
        ServiceResult InvokeServiceMethod(string serviceUrl, string contractName, string methodName, List<MethodParameter> parameters);
    }
}
