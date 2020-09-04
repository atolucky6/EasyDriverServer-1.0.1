using System;

namespace EasyScada.Core.Animate
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AnimateAttribute : Attribute
    {
        public string DisplayName { get; private set; }
        public bool Enabled { get; private set; }
        public bool AllowDiscreteTrigger { get; private set; }
        public bool AllowAnalogTrigger { get; private set; }
        public bool AllowExpressionTrigger { get; private set; }

        public AnimateAttribute(
            string displayName = null,
            bool enabled = true,
            bool allowDiscreteTrigger = true,
            bool allowAnalogTrigger = true,
            bool allowExpressionTrigger = true)
        {
            DisplayName = displayName;
            Enabled = enabled;
            AllowDiscreteTrigger = allowDiscreteTrigger;
            AllowAnalogTrigger = allowAnalogTrigger;
            AllowExpressionTrigger = allowExpressionTrigger;
        }
    }
}
