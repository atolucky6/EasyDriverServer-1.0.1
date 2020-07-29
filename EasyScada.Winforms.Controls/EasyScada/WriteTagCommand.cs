using System;
using System.ComponentModel;
using System.Threading.Tasks;
using EasyScada.Winforms.Connector;

namespace EasyScada.Winforms.Controls
{
    public class WriteTagCommand : ISupportConnector, ISupportTag
    {
        #region Public members

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        [DefaultValue(true)]
        public bool Enabled { get; set; } = true;

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public string WriteValue { get; set; }

        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public int WriteDelay { get; set; }

        #endregion

        #region Events

        public event EventHandler<TagWritingEventArgs> TagWriting;
        public event EventHandler<TagWritedEventArgs> TagWrited;

        #endregion

        #region Constructors

        public WriteTagCommand()
        {

        }

        #endregion

        #region Public methods

        public void Execute()
        {
            if (Enabled)
                OnWriteTag(WriteValue);
        }

        public override string ToString()
        {
            return PathToTag;
        }

        #endregion

        #region Private methods

        protected virtual async void OnWriteTag(string writeValue)
        {
            if (Connector != null && Connector.IsStarted && LinkedTag != null && LinkedTag.Value != writeValue && writeValue.IsNumber())
            {
                await Task.Delay(WriteDelay);

                TagWriting?.Invoke(this, new TagWritingEventArgs(LinkedTag, writeValue));
                Quality writeQuality = Quality.Uncertain;

                writeQuality = await LinkedTag.WriteAsync(writeValue);

                TagWrited?.Invoke(this, new TagWritedEventArgs(LinkedTag, writeQuality, writeValue));
            }
        }

        #endregion

        #region ISupportConnector

        EasyDriverConnector easyDriverConnector;
        [Description("Select driver connector for control")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        public EasyDriverConnector Connector
        {
            get { return easyDriverConnector; }
            set
            {
                if (value != null)
                    easyDriverConnector = value;
            }
        }

        #endregion

        #region ISupportTag

        [Description("Select path to tag for control")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        [TypeConverter(typeof(EasyScadaTagPathConverter))]
        public string PathToTag { get; set; }

        ITag linkedTag;
        [Browsable(false)]
        public ITag LinkedTag
        {
            get
            {
                if (linkedTag == null)
                {
                    linkedTag = Connector?.GetTag(PathToTag);
                }
                else
                {
                    if (linkedTag.Path != PathToTag)
                        linkedTag = Connector?.GetTag(PathToTag);
                }
                return linkedTag;
            }
        }

        #endregion
    }

    /// <summary>
    /// Manages a collection of WriteTagCommand instances.
    /// </summary>
    public class WriteTagCommandCollection : TypedCollection<WriteTagCommand>
    {
        #region Public
        /// <summary>
        /// Gets the item with the provided name.
        /// </summary>
        /// <param name="name">Name to find.</param>
        /// <returns>Item with matching name.</returns>
        public override WriteTagCommand this[string name]
        {
            get
            { 
                return null;
            }
        }
        #endregion
    }
}
