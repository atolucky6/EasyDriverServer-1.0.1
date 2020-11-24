using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyDriverPlugin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EasyDriverPlugin
{
    public class ClientTag : IClientTag
    {
        public string Value { get; set; }

        public Quality Quality { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string Error { get; set; }

        public List<IClientTag> Childs { get; set; }

        [JsonConstructor]
        public ClientTag()
        {
            Childs = new List<IClientTag>();
        }
    }
}
