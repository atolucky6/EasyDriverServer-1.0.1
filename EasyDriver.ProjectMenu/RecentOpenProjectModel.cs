using Newtonsoft.Json;
using System;
using System.IO;

namespace EasyDriver.ProjectMenu
{
    public class RecentOpenProjectModel
    {
        public bool Pined { get; set; }
        [JsonIgnore]
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(Path))
                    return string.Empty;
                return System.IO.Path.GetFileNameWithoutExtension(Path);
            }
        }
        public string Path { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
