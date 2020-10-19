using System.ComponentModel;
using System.Drawing;

namespace EasyScada.Core
{
    public class AlarmClass : IUniqueNameItem
    {
        public bool Enabled { get; set; } = true;
        public string Name { get; set; }
        public string IncomingText { get; set; }
        public string OutgoingText { get; set; }
        public string AcknowledgedText { get; set; }
        public string Description { get; set; }
        public string BackColorIncoming { get; set; }
        public string BackColorOutgoing { get; set; }
        public string BackColorAcknowledged { get; set; }

        [Browsable(false)]
        public bool ReadOnly { get; set; }

        public AlarmClass()
        {
            IncomingText = "In";
            OutgoingText = "Out";
            AcknowledgedText = "Ack";
            BackColorIncoming = "#FF0000";
            BackColorOutgoing = "#FFFFFF";
            BackColorAcknowledged = "#FFFFFF";
        }

        public Color GetColorWinform(string colorString)
        {
            try
            {
                return ColorTranslator.FromHtml(colorString);
            }
            catch { return Color.Transparent; }
        }
    }
}
