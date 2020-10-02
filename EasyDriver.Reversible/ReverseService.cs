using EasyDriver.ServiceContainer;
using EasyDriver.ServicePlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriver.Reversible
{
    public class ReverseService : EasyServicePlugin, IReverseService
    {
        public ReverseSession Session { get; set; }
        public event EventHandler HistoryChanged;

        public ReverseService(IServiceContainer serviceContainer) : base(serviceContainer)
        {
            Session = new ReverseSession("MainReverseSession");
            Session.HistoryChanged += Session_HistoryChanged;
        }

        private void Session_HistoryChanged(object sender, EventArgs e)
        {
            HistoryChanged?.Invoke(sender, e);
        }

        public Transaction Begin() => Session.Begin();

        public Transaction Begin(string operationName) => Session.Begin(operationName);

        public bool CanRedo() => Session.CanRedo();

        public bool CanUndo() => Session.CanUndo();

        public void ClearHistory() => Session.Clear();

        public void Redo() => Session.Redo();

        public void Redo(int count) => Session.Redo(count);

        public void Undo() => Session.Undo();

        public void Undo(int count) => Session.Undo(count);

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
        }
    }
}
