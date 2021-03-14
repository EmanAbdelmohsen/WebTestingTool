using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using WebTestingService.Utilities;

namespace WebTestingService
{
    //reference: https://www.c-sharpcorner.com/UploadFile/yusufkaratoprak/dynamically-wcf-usage-in-client/
    public class MyServiceClient
    {
        public ServiceResult Invoke(string url)
        {
            var client = new WebClient();

            //use the service url of the wsdl file
            //url = url.Contains("?wsdl") ? url : url + "?wsdl";

            //Import WSDL
            WsdlImporter importer = ImportWSDL(new Uri(url));

            if (importer != null)
            {
                //Extract Service and Data Contract Descriptions
                Collection<ContractDescription> svcCtrDesc = importer.ImportAllContracts();

                //Compile the description to assembly
                CodeCompileUnit ccu = GetServiceAndDataContractCompileUnitFromWSDL(svcCtrDesc);
                CompilerResults results = GenerateContractsAssemblyInMemory(new CodeCompileUnit[] { ccu });

                if (!results.Errors.HasErrors)
                {
                    var assembly = results.CompiledAssembly;
                    if (assembly != null)
                    {
                        //Extract all end points available on the WSDL
                        IDictionary<string, IEnumerable<ServiceEndpoint>> allEP = GetEndPointsForServiceContracts(importer, svcCtrDesc);
                        IEnumerable<ServiceEndpoint> currentSvcEP;
                        if (allEP.TryGetValue("IService", out currentSvcEP))
                        {
                            //Find the endpoint of the service to which the proxy needs to contact
                            var svcEP = currentSvcEP.First(x => x.ListenUri.AbsoluteUri == url.Replace("?wsdl", ""));

                            //Generate proxy
                            var proxy = GetProxy("IService", svcEP, assembly);

                            if (proxy != null)
                            {
                                var methods = proxy.GetType().GetMethods();
                                /*var method = service.GetType().GetMethod(methodName);
                                if (method == null)
                                {
                                    var message = "Method: " + methodName + " is invalid. Valid Methods are: ";
                                    message = methods.Aggregate(message, (current, methodInfo) => current + methodInfo.Name + "; ");
                                    throw new Exception(message);
                                }*/

                                //success service call
                                return new ServiceResult();
                            }

                            return new ServiceResult(4, "Service invocation error. Service provided: " + "");
                        }
                        return new ServiceResult(6, "Service invocation error. Service provided: " + "");
                    }
                    return new ServiceResult(5, "Service invocation error. No assembly found.");

                }
                //return error result with errors
                string errorMsg = "Compile Error Occurred calling web service. Compiler errors: " + results.Errors.Count + ".  ";
                foreach (var error in results.Errors)
                {
                    errorMsg += "," + error.ToString();
                }
                return new ServiceResult(3, errorMsg);
            }

            return new ServiceResult(1, "wsdl file unavailable");
        }

        private WsdlImporter ImportWSDL(Uri wsdlLoc)
        {
            MetadataExchangeClient mexC = new MetadataExchangeClient(wsdlLoc, MetadataExchangeClientMode.HttpGet)
            {
                ResolveMetadataReferences = true
            };
            MetadataSet metaSet = mexC.GetMetadata();
            return new WsdlImporter(metaSet);
        }

        private Dictionary<string, IEnumerable<ServiceEndpoint>> GetEndPointsForServiceContracts(WsdlImporter imptr, Collection<ContractDescription> svcCtrDescs)
        {
            ServiceEndpointCollection allEP = imptr.ImportAllEndpoints();
            var ctrEP = new Dictionary<string, IEnumerable<ServiceEndpoint>>();
            foreach (ContractDescription svcCtrDesc in svcCtrDescs)
            {
                List<ServiceEndpoint> eps = allEP.Where(x => x.Contract.Name == svcCtrDesc.Name).ToList();
                ctrEP.Add(svcCtrDesc.Name, eps);
            }
            return ctrEP;
        }

        private object GetProxy(string ctrName, ServiceEndpoint svcEP, Assembly assembly)
        {
            Type prxyT = assembly.GetTypes().First(t => t.IsClass && t.GetInterface(ctrName) != null && t.GetInterface(typeof(ICommunicationObject).Name) != null);
            object proxy = assembly.CreateInstance(prxyT.Name, false, BindingFlags.CreateInstance,
                                    null, new object[] { svcEP.Binding, svcEP.Address }, CultureInfo.CurrentCulture, null);
            return proxy;
        }

        private CodeCompileUnit GetServiceAndDataContractCompileUnitFromWSDL(Collection<ContractDescription> svcCtrDescs)
        {
            ServiceContractGenerator svcCtrGen = new ServiceContractGenerator();
            foreach (ContractDescription ctrDesc in svcCtrDescs)
            {
                svcCtrGen.GenerateServiceContractType(ctrDesc);
            }
            return svcCtrGen.TargetCompileUnit;
        }

        private CompilerResults GenerateContractsAssemblyInMemory(params CodeCompileUnit[] codeCompileUnits)
        {
            // Generate a code file for the contracts 
            CodeGeneratorOptions opts = new CodeGeneratorOptions
            {
                BracingStyle = "C"
            };
            CodeDomProvider pro = CodeDomProvider.CreateProvider("C#");
            // Compile the code file to an in-memory assembly
            // Don't forget to add all WCF-related assemblies as references
            CompilerParameters prms = new CompilerParameters(new string[] { "System.dll", "System.ServiceModel.dll",
                "System.Runtime.Serialization.dll"})
            {
                GenerateInMemory = true,
                GenerateExecutable = false
            };
            return pro.CompileAssemblyFromDom(prms, codeCompileUnits);
        }
    }

}
