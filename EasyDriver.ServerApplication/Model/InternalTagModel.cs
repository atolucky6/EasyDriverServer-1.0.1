using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.ServerApplication
{
    public class InternalTagModel
    {
        [BsonId]
        public string GUID { get; set; }
        public string Value { get; set; }
        public string LastUpdateTime { get; set; }
    }
}
