using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildStatus.Poco
{
    public class RequestEventArgs : EventArgs
    {
        public string Url { get; set; }
        public string ProjectName { get; set; }
        public string PersonalAccessToken { get; set; }
        public string DefinitionId { get; set; }
    }
}
