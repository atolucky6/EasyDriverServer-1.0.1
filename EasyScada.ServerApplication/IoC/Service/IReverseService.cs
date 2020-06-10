using EasyScada.ServerApplication.Reversible;
using System;

namespace EasyScada.ServerApplication
{
    public interface IReverseService
    {
        Transaction Begin();
        Transaction Begin(string operationName);
        void Undo();
        void Undo(int count);
        void Redo();
        void Redo(int count);
        bool CanUndo();
        bool CanRedo();
        void ClearHistory();
        event EventHandler HistoryChanged;
    }

    public class ReverseService : IReverseService
    {
        private readonly ReverseSession session;
        public event EventHandler HistoryChanged;

        public ReverseService()
        {
            session = new ReverseSession("MainReverseSession");
            session.HistoryChanged += Session_HistoryChanged;
        }

        private void Session_HistoryChanged(object sender, EventArgs e)
        {
            HistoryChanged?.Invoke(sender, e);
        }

        public Transaction Begin() => session.Begin();

        public Transaction Begin(string operationName) => session.Begin(operationName);

        public bool CanRedo() => session.CanRedo();

        public bool CanUndo() => session.CanUndo();

        public void ClearHistory() => session.Clear();

        public void Redo() => session.Redo();

        public void Redo(int count) => session.Redo(count);

        public void Undo() => session.Undo();

        public void Undo(int count) => session.Undo(count);
    }
}
