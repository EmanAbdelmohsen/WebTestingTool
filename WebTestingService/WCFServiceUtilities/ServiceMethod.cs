using System.Collections.Generic;

namespace WebTestingService
{
    public class ServiceMethod
    {
        public string Name { get; set; }
        public string ReturnType { get; set; }
        public List<MethodParameter> Parameters { get; set; }
    }

    public class MethodParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
