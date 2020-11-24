using EasyDriver.ServicePlugin;
using System;

namespace EasyDriver.Service.Reversible
{
    /// <summary>
    /// Cung cấp chức năng đảo ngược hoặc làm lại một hoạt động nào đó
    /// </summary>
    [Service(0, true)]
    public class ReversibleService : EasyServicePluginBase, IReversibleService
    {
        public ReversibleSession Session { get; set; }

        public ReversibleService() : base()
        {
            Session = new ReversibleSession("Default");
            Session.HistoryChanged += OnHistoryChanged;
        }

        public Transaction Begin() => Session.Begin();

        public Transaction Begin(string operationName) => Session.Begin(operationName);

        public void Undo() => Session.Undo();

        public void Undo(int count) => Session.Undo(count);

        public void Redo() => Session.Redo();

        public void Redo(int count) => Session.Redo(count);

        public bool CanUndo() => Session.CanUndo();

        public bool CanRedo() => Session.CanRedo();

        public void ClearHistory() => Session.Clear();

        private void OnHistoryChanged(object sender, EventArgs e)
        {
            HistoryChanged?.Invoke(sender, e);
        }

        public event EventHandler HistoryChanged;

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
        }
    }
}
