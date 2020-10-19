using System.Collections.Generic;

namespace EasyScada.Core
{
    public class SMSSetting : IUniqueNameItem
    {
        public string Name { get; set; }
        public string PhoneNumbers { get; set; }
        public bool Enabled { get; set; } = true;
        public bool ReadOnly { get; set; }
        public string ComPort { get; set; }
        public int Baudrate { get; set; } = 9600;
        public int DataBits { get; set; } = 7;
        public string Parity { get; set; } = "None";
        public string StopBits { get; set; } = "One";
        public int Timeout { get; set; } = 10000;

        public SMSSetting()
        {
        }

        public IEnumerable<string> GetPhoneNumbers()
        {
            foreach (var item in PhoneNumbers?.Split(','))
            {
                yield return item;
            }
        }
    }
}
