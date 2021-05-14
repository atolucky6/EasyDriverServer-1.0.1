using System;

namespace EasyScada.Core
{
    public class LogoutEventArgs
    {
        public string Username { get; }
        public string Role { get; }
        public DateTime LogoutTime { get; }

        public LogoutEventArgs(string userName, string role, DateTime logoutTime)
        {
            Username = userName;
            Role = role;
            LogoutTime = logoutTime;
        }
    }
}
