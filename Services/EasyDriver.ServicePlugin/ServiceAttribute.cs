using System;

namespace EasyDriver.ServicePlugin
{
    public class ServiceAttribute : Attribute
    {
        public bool IsUnique { get; protected set; }
        public int InitializePiority { get; protected set; }

        public ServiceAttribute(int initializePiority, bool isUnique)
        {
            InitializePiority = initializePiority;
            IsUnique = isUnique;
        }
    }
}
