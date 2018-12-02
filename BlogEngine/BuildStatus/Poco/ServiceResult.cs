using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildStatus.Poco
{
    public class ServiceResult
    {
        public string FriendlyMessage { get; set; }
        public string DeveloperMessage { get; set; }
        public bool IsError { get; set; }
        public byte[] ResponseBody { get; set; }
    }
}
