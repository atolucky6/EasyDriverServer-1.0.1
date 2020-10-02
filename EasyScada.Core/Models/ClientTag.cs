using System;
using System.Collections.Generic;

namespace EasyScada.Core
{
    class ClientTag
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Error { get; set; }
        public string Value { get; set; }
        public Quality Quality { get; set; }
        public DateTime TimeStamp { get; set; }

        public List<ClientTag> Childs { get; set; }
    }
}
