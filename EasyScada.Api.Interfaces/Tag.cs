using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Api.Interfaces
{
    [Serializable]
    public class Tag 
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string DataType { get; set; }

        public string Value { get; set; }

        public Quality Quality { get; set; }

        public int RefreshRate { get; set; }

        public int RefreshInterval { get; set; }

        public AccessPermission AccessPermission { get; set; }

        public string Error { get; set; }

        public DateTime TimeStamp { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

        public string Path { get; set; }

        public bool Checked { get; set; }
    }
}
