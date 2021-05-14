using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScada.Core
{
    public class LoginEventArgs : EventArgs
    {
        public string Username { get; }
        public string Role { get; }
        public DateTime LoginTime { get; }

        public LoginEventArgs(string userName, string role, DateTime loginTime)
        {
            Username = userName;
            Role = role;
            LoginTime = loginTime;
        }
    }
}
