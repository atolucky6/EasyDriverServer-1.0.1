using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public class EmailSetting : IUniqueNameItem
    {
        public string Name { get; set; }
        public string EmailsString { get; set; }
        public string CCString { get; set; }
        public bool Enabled { get; set; } = true;
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public bool EnableSSL { get; set; } = true;
        public string CredentialEmail { get; set; }
        public string CredentialPassword { get; set; }
        public int Timeout { get; set; } = 100000;
        public bool ReadOnly { get; set; }

        public EmailSetting()
        {

        }

        public IEnumerable<string> GetEmails()
        {
            foreach (var item in EmailsString?.Split(','))
            {
                yield return item;
            }
        }

        public IEnumerable<string> GetCC()
        {
            foreach (var item in CCString?.Split(','))
            {
                yield return item;
            }
        }
    }
}
