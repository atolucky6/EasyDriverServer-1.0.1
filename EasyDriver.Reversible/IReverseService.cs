using EasyDriver.ServicePlugin;
using System;

namespace EasyDriver.Reversible
{
    public interface IReverseService : IEasyServicePlugin
    {
        ReverseSession Session { get; }
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
}
