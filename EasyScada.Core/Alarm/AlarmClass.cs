using System.ComponentModel;
using System.Drawing;

namespace EasyScada.Core
{
    public class AlarmClass : IUniqueNameItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IncomingText { get; set; }
        public string OutgoingText { get; set; }
        public string AcknowledgedText { get; set; }
        public string Description { get; set; }
        public string BackgroundColorIncoming { get; set; }
        public string BackgroundColorOutgoing { get; set; }
        public string BackgroundColorAcknowledged { get; set; }

        [Browsable(false)]
        public bool ReadOnly { get; set; }

        public AlarmClass()
        {
            IncomingText = "In";
            OutgoingText = "Out";
            AcknowledgedText = "Ack";
            BackgroundColorIncoming = "#FF0000";
            BackgroundColorOutgoing = "#FFFFFF";
            BackgroundColorAcknowledged = "#FFFFFF";
        }
    }
}
