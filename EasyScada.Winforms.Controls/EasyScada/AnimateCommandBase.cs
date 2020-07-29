using EasyScada.Winforms.Connector;
using System;
using System.ComponentModel;

namespace EasyScada.Winforms.Controls
{
    public abstract class AnimateCommandBase : ISupportConnector, ISupportTag
    {
        public virtual object BaseControl { get; set; }

        public virtual string PropertyName { get; set; }
        public virtual object AnimateValue { get; set; }

        public virtual string CompareValue { get; set; }

        public virtual decimal LowValue { get; set; }
        public virtual decimal HighValue { get; set; }
        public virtual decimal CurrentValue { get; set; }

        public virtual AnimateMode AnimateMode { get; protected set; }
        public virtual AnimatePiority AnimatePiority { get; set; }
        public virtual CompareMode CompareMode { get; set; }
        public virtual CompareValueMode CompareValueMode { get; set; }

        public virtual string PathToTag { get; set; }
        public virtual string PathToHighTag { get; set; }
        public virtual string PathToLowTag { get; set; }
        public virtual string PathToDirectTag { get; set; }

        private ITag _linkedTag;
        public virtual ITag LinkedLowTag
        {
            get
            {
                if (_linkedTag == null || _linkedTag.Path != PathToTag)
                    _linkedTag = Connector?.GetTag(PathToTag);
                return _linkedTag;
            }
        }

        private ITag _linkedHighTag;
        public virtual ITag LinkedHighTag
        {
            get
            {
                if (_linkedHighTag == null || _linkedHighTag.Path != PathToHighTag)
                    _linkedHighTag = Connector?.GetTag(PathToHighTag);
                return _linkedHighTag;
            }
        }

        private ITag _linkedLowTag;
        public virtual ITag LinkedTag
        {
            get
            {
                if (_linkedLowTag == null || _linkedLowTag.Path != PathToLowTag)
                    _linkedLowTag = Connector?.GetTag(PathToLowTag);
                return _linkedLowTag;
            }
        }

        private ITag _linkedDirectTag;
        public virtual ITag LinkedDirectTag
        {
            get
            {
                if (_linkedDirectTag == null || _linkedDirectTag.Path != PathToDirectTag)
                    _linkedDirectTag = Connector?.GetTag(PathToDirectTag);
                return _linkedDirectTag;
            }
        }

        private EasyDriverConnector _connector;
        public virtual EasyDriverConnector Connector
        {
            get { return _connector; }
            set
            {
                if (value != null && _connector == null)
                {
                    _connector = value;
                    if (_connector.IsStarted)
                    {
                        OnConnectorChanged(value, EventArgs.Empty);
                    }
                    else
                    {
                        _connector.Started += OnConnectorChanged;
                    }
                }
            }
        }

        public AnimateCommandBase()
        {

        }

        public abstract void Execute();

        protected virtual void OnConnectorChanged(object sender, EventArgs e)
        {
            try
            {
                Execute();
            }
            catch { }
        }

        protected virtual bool TryGetCompareValue(out decimal result)
        {
            result = 0;
            if (CompareValueMode == CompareValueMode.Const)
                return decimal.TryParse(CompareValue, out result);
            else
                return decimal.TryParse(LinkedHighTag?.Value, out result);
        }

        protected virtual object GetAnimateValue()
        {
            if (AnimateMode == AnimateMode.Direct)
                return LinkedDirectTag?.Value;
            return AnimateValue;
        }

        /// <summary>
        /// The method to get a <see cref="PropertyDescriptor"/> of the control by property name
        /// </summary>
        /// <param name="control"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        protected virtual PropertyDescriptor GetPropertyByName(object control, string propName)
        {
            try
            {
                PropertyDescriptor prop;
                prop = TypeDescriptor.GetProperties(control)[propName];
                return prop;
            }
            catch { return null; }
        }

        /// <summary>
        /// Set the value for property of the control
        /// </summary>
        /// <param name="control"></param>
        /// <param name="value"></param>
        /// <param name="propName"></param>
        protected virtual void SetValue(object control, object value, string propName)
        {
            control.GetPropertyByName(propName)?.SetValue(control, value);
        }
    }

    /// <summary>
    /// Manages a collection of WriteTagCommand instances.
    /// </summary>
    public class AnimateCommandCollection : TypedCollection<AnimateCommandBase>
    {
        #region Public
        /// <summary>
        /// Gets the item with the provided name.
        /// </summary>
        /// <param name="name">Name to find.</param>
        /// <returns>Item with matching name.</returns>
        public override AnimateCommandBase this[string name]
        {
            get
            {
                return null;
            }
        }
        #endregion
    }
}
