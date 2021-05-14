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

        protected Color? _backColorIncomming = null;
        protected Color? _backColorOutgoing = null;
        protected Color? _backColorAcknowledged = null;

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

        public Color GetBackColorIncomming()
        {
            if (_backColorIncomming.HasValue)
            {
                return _backColorIncomming.Value;
            }
            else
            {
                try
                {
                    _backColorIncomming = ColorTranslator.FromHtml(BackColorIncoming); ;
                }
                catch { _backColorIncomming = null; }
            }
            return Color.Transparent;
        }

        public Color GetBackColorOutgoing()
        {
            if (_backColorOutgoing.HasValue)
            {
                return _backColorOutgoing.Value;
            }
            else
            {
                try
                {
                    _backColorOutgoing = ColorTranslator.FromHtml(BackColorOutgoing); ;
                }
                catch { _backColorOutgoing = null; }
            }
            return Color.Transparent;
        }

        public Color GetBackColorAcknowledged()
        {
            if (_backColorAcknowledged.HasValue)
            {
                return _backColorAcknowledged.Value;
            }
            else
            {
                try
                {
                    _backColorAcknowledged = ColorTranslator.FromHtml(BackColorAcknowledged); ;
                }
                catch { _backColorAcknowledged = null; }
            }
            return Color.Transparent;
        }
    }
}
