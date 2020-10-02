using System;
using System.ComponentModel;

namespace EasyScada.Core
{
    public class LogColumn
    {
        public bool Enabled { get; set; } = true;
        [TypeConverter(typeof(TagPathConverter))]
        public string ColumnName { get; set; }
        [TypeConverter(typeof(TagPathConverter))]
        public string TagPath { get; set; }
        public string DefaultValue { get; set; }
        public LogColumnMode Mode { get; set; }
        public bool UseDefaultValueWhenQualityBad { get; set; }
        public string Description { get; set; }

        public LogColumn()
        {
            Connector = EasyDriverConnectorProvider.GetEasyDriverConnector();
            if (Connector.IsStarted)
                OnConnectorStarted(null, EventArgs.Empty);
            else
                Connector.Started += OnConnectorStarted;
        }

        private void OnConnectorStarted(object sender, EventArgs e)
        {
            if (sender != null)
                Connector.Started -= OnConnectorStarted;
            Tag = Connector.GetTag(TagPath);
        }

        [Browsable(false)]
        public IEasyDriverConnector Connector { get; protected set; }

        [Browsable(false)]
        public ITag Tag { get; internal set; }

        [Browsable(false)]
        public string Value
        {
            get
            {
                if (Tag == null)
                    return DefaultValue;
                if (Tag.Quality == Quality.Good)
                    return Tag.Value;
                else if (Tag.Quality == Quality.Bad)
                    return UseDefaultValueWhenQualityBad ? DefaultValue : Tag.Value;

                return DefaultValue;
            }
        }

        [Browsable(false)]
        public Quality Quality { get => Tag == null ? Quality.Uncertain : Tag.Quality; }

        public override string ToString()
        {
            return $"{ColumnName} - {TagPath}";
        }
    }
}
