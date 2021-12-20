using System.Collections.Generic;

namespace TestGenerator.Templates
{
    public class MethodDto
    {
        public string name { get; set; }
        public string fullName { get; set; }
        public string type { get; set; }
    }

    public class CallDto
    {
        public string service { get; set; }
        public string method { get; set; }
    }

    public class ServiceDto
    {
        public string name { get; set; }
        public string type { get; set; }
    }

    public partial class Template
    {
        public string _sut;
        public Dictionary<MethodDto, List<CallDto>> _methods;
        public List<ServiceDto> _services;

        public Template(
            string sut,
            Dictionary<MethodDto, List<CallDto>> methods,
            List<ServiceDto> services)
        {
            _sut = sut;
            _methods = methods;
            _services = services;
        }
    }
}
