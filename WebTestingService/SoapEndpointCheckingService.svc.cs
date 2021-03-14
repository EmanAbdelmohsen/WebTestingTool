using System.Collections.Generic;
using WebTestingService.Utilities;

namespace WebTestingService
{
    public class SoapEndpointCheckingService : ISoapEndpointCheckingService
    {
        public ServiceResult<List<string>> GetAllServiceContracts(string serviceUrl)
        {
            throw new System.NotImplementedException();
        }

        public ServiceResult<List<ServiceMethod>> GetAllContactMethods(string serviceUrl, string contractName)
        {
            throw new System.NotImplementedException();
        }

        public ServiceResult InvokeServiceMethod(string serviceUrl, string contractName, string methodName, List<MethodParameter> parameters)
        {
            throw new System.NotImplementedException();
        }
    }
}
