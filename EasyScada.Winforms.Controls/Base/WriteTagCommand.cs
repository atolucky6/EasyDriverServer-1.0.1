using EasyScada.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return TagPath;
        }

        #endregion

        #region Private methods

        protected virtual async void OnWriteTag(string writeValue)
        {
            if (Connector != null &&
                Connector.IsStarted &&
                LinkedTag != null &&
                LinkedTag.Value != writeValue &&
                IsNumber(writeValue))
            {
                await Task.Delay(WriteDelay);

                TagWriting?.Invoke(this, new TagWritingEventArgs(LinkedTag, writeValue));
                Quality writeQuality = Quality.Uncertain;

                var response = await LinkedTag.WriteAsync(writeValue);

                if (response.IsSuccess)
                    writeQuality = Quality.Good;
                else
                    writeQuality = Quality.Bad;

                TagWrited?.Invoke(this, new TagWritedEventArgs(LinkedTag, writeQuality, writeValue));
            }
        }

        /// <summary>
        /// Check the provided string is a number or not
        /// </summary>
        /// <param name="str"></param>
        /// <returns>True is a number; False is not a number</returns>
        private bool IsNumber(string str)
        {
            return decimal.TryParse(str, out decimal value);
        }

        #endregion

        #region ISupportConnector

        [Description("Select driver connector for control")]
        [Browsable(false), Category(DesignerCategory.EASYSCADA)]
        public IEasyDriverConnector Connector => EasyDriverConnectorProvider.GetEasyDriverConnector();

        #endregion

        #region ISupportTag

        [Description("Select path to tag for control")]
        [Browsable(true), Category(DesignerCategory.EASYSCADA)]
        [Editor(typeof(PathToTagPropertyEditor), typeof(UITypeEditor))]
        public string TagPath { get; set; }

        ITag linkedTag;
        [Browsable(false)]
        public ITag LinkedTag
        {
            get
            {
                if (linkedTag == null)
                {
                    linkedTag = Connector?.GetTag(TagPath);
                }
                else
                {
                    if (linkedTag.Path != TagPath)
                        linkedTag = Connector?.GetTag(TagPath);
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
