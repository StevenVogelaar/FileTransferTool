using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransferTool.CoreLibrary
{
    public class FTTConsoleMessage
    {

        public FTTConsole.MessageType Type { get; set; }
        public String Msg { get; set; }

        public FTTConsoleMessage(String message, FTTConsole.MessageType type)
        {
            Msg = message;
            Type = type;
        }
    }


}
