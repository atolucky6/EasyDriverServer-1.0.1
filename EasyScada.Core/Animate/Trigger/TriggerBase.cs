using EasyScada.Core.Evaluate;
using System;
using System.ComponentModel;
using System.Reflection;

namespace EasyScada.Core
{
    public abstract class TriggerBase
    {
        #region Public properties

        private string tokenExpressionString;
        public virtual string TokenExpressionString
        {
            get { return tokenExpressionString; }
            set
            {
                if (value != tokenExpressionString)
                {
                    tokenExpressionString = value;
                    TokenExpression = new TokenExpression(value);
                }
            }
        }

        public virtual string TriggerTagPath { get; set; }
        public virtual bool Enabled { get; set; } = true;
        public virtual object Target { get; set; }
        public virtual AnimatePropertyWrapper AnimatePropertyWrapper { get; set; }
        public virtual object SetValue { get; set; }
        public virtual Evaluator Evaluator { get; protected set; }
        public virtual TokenExpression TokenExpression { get; private set; }
        public virtual string LastErrorMessage { get; protected set; }
        public virtual int Delay { get; set; }
        public CompareMode CompareMode { get; set; }
        public virtual string Description { get; set; }

        private ITag triggerTag;
        [Browsable(false)]
        public virtual ITag TriggerTag
        {
            get
            {
                if (triggerTag == null)
                    triggerTag = EasyDriverConnectorProvider.GetEasyDriverConnector().GetTag(TriggerTagPath);
                else
                {
                    if (triggerTag.Path != TriggerTagPath)
                        triggerTag = EasyDriverConnectorProvider.GetEasyDriverConnector().GetTag(TriggerTagPath);
                }
                return triggerTag;
            }
        }

        #endregion

        #region Constructors

        public TriggerBase()
        {
            Evaluator = new Evaluator();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Hàm thực thi trigger
        /// </summary>
        public virtual void Execute(object parameter = null)
        {
            try
            {
                if (CanExecute(parameter))
                    AnimatePropertyWrapper.UpdateValue();
            }
            catch (Exception ex)
            {
                LastErrorMessage = $"Error while execute trigger: {ex.Message}";
            }
        }

        /// <summary>
        /// Hàm kiểm tra điều kiện để thực hiện trigger
        /// </summary>
        /// <returns>Trả về True nếu cho phép thực hiện và ngược lại</returns>
        protected virtual bool CanExecute(object parameter = null)
        {
            if (Enabled && Target != null && AnimatePropertyWrapper != null)
            {
                if (string.IsNullOrEmpty(TokenExpressionString))
                    return false;
                if (!TokenExpression.AnyErrors)
                {
                    if (TokenExpression.Variables.Count > 0)
                        return false;

                    bool canExecute = false;
                    if (Evaluator.Evaluate(TokenExpression, out string value, out string error))
                        bool.TryParse(value, out canExecute);
                    LastErrorMessage = error;
                    return canExecute;
                }
            }
            return false;
        }

        public virtual string Compile()
        {
            CanExecute();
            return LastErrorMessage;
        }

        #endregion
    }
}
