using System;
using System.Collections.Generic;

namespace EasyDriver.Service.Reversible
{
    public class ReversibleSession : Transaction
    {
        #region Members

        Transaction previous;

        #endregion

        #region Constructors

        public ReversibleSession() : base(null, null)
        {
        }

        public ReversibleSession(string name) : base(null, name) { }

        #endregion

        #region Methods

        /// <summary>
        /// Bắt đầu một <see cref="Transaction"/> mới
        /// </summary>
        /// <param name="operationName">Tên của transaction</param>
        /// <returns></returns>
        public new Transaction Begin(string operationName)
        {
            if (Next != null)
                throw new InvalidOperationException("Không thể bắt đầu một transaction nếu như transaction trước đó chưa hoàn tất.");
            previous = Current;
            Current = this;
            return Transaction.Begin(operationName);
        }

        /// <summary>
        /// Bắt đầu một <see cref="Transaction"/> mới
        /// </summary>
        /// <param name="operationName">Tên của transaction</param>
        /// <returns></returns>
        public new Transaction Begin() => Begin(null);

        public override void AddChange(ReversibleChange change)
        {
            base.AddChange(change);
            RaiseHistoryChanged();
        }

        private void RaiseHistoryChanged() => HistoryChanged?.Invoke(this, EventArgs.Empty);

        protected override void OnNextTransactionFinished(Transaction nextTransaction)
        {
            base.OnNextTransactionFinished(nextTransaction);
            if (nextTransaction == Current)
            {
                Current = previous;
                previous = null;
            }
        }

        protected override void OnReversed(ReverseDirection direction)
        {
            base.OnReversed(direction);
            RaiseHistoryChanged();
        }

        public void Undo() => Undo(1);
        public void Undo(int count)
        {
            if (Index > 0)
            {
                Transaction currentTransaction = Current;
                Current = null;
                bool failed = false;
                while (count > 0 && Index > 0)
                {
                    int i = Index - 1;
                    if (failed = !Changes[i].Reverse())
                        break;
                    Index--;
                    count--;
                }
                if (!failed)
                    OnReversed(ReverseDirection.Undo);
                Current = currentTransaction;
            }
        }
        public bool CanUndo() => Index > 0;

        public void Redo() => Redo(1);
        public void Redo(int count)
        {
            if (Index < Changes.Count)
            {
                Transaction currentTransaction = Current;
                Current = null;
                bool failed = false;
                while (count > 0 && Index < Changes.Count)
                {
                    int i = Index;
                    if (failed = !Changes[i].Reverse())
                        break;
                    Index++;
                    count--;
                }
                if (!failed)
                    OnReversed(ReverseDirection.Redo);
                Current = currentTransaction;
            }
        }
        public bool CanRedo() => Index < Changes.Count;

        public string GetUndoText()
        {
            if (Index > 0)
                return Changes[Index - 1].Name;
            return null;
        }

        public IEnumerable<string> GetUndoTextList()
        {
            for (int i = 0; i < Index; i++)
                yield return Changes[Index - i - 1].Name;
        }

        public string GetRedoText()
        {
            if (Index < Changes.Count)
                return Changes[Index].Name;
            return null;
        }

        public IEnumerable<string> GetRedoTextList()
        {
            for (int i = 0; i < Changes.Count - Index; i++)
                yield return Changes[Index + i].Name;
        }

        public void Clear()
        {
            Index = 0;
            Changes.Clear();
            Current = null;
            previous = null;
        }

        #endregion

        #region Events

        public event EventHandler HistoryChanged;

        #endregion
    }
}
