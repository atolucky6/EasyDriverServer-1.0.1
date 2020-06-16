using System;
using System.Collections.Generic;

namespace EasyScada.Api.Interfaces
{
    [Serializable]
    public class Device
    {
        public string Name { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

        public DateTime LastRefreshTime { get; set; }

        public string Error { get; set; }

        public List<Tag> Tags { get; set; }

        public string Path { get; set; }

        public bool Checked { get; set; }
    }
}
