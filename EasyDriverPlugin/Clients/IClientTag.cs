using EasyDriverPlugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriverPlugin
{
    public interface IClientTag 
    {
        string Name { get; }
        string Path { get; }
        string Error { get; }
        string Value { get; }
        Quality Quality { get; }
        DateTime TimeStamp { get; }
        [JsonConverter(typeof(ListClientTagJsonConverter))]
        List<IClientTag> Childs { get; }
    }
}
