using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyDriverPlugin
{
    public class CommandExecutedEventArgs : EventArgs
    {
        public WriteCommand WriteCommandSource { get; private set; }
        public WriteResponse WriteResponse { get; private set; }

        public CommandExecutedEventArgs(WriteCommand command, WriteResponse response)
        {
            WriteCommandSource = command;
            WriteResponse = response;
        }
    }
}
