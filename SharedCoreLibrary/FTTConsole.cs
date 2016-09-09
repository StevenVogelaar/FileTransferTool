using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CoreLibrary
{
    public static class FTTConsole
    {

        public enum MessageType
        {
            Info,
            Error,
            Debug
        }

        public delegate void ConsoleMessageHandler(ConsoleMessageEventArgs e);
        public static event ConsoleMessageHandler ConsoleMessage;

        private static List<FTTConsoleMessage> _out;

        private static bool _debugMode;

        /// <summary>
        /// Must be called before console can be used.
        /// </summary>
        public static void Init()
        {
            _out = new List<FTTConsoleMessage>();
            _debugMode = true;
        }

        /// <summary>
        /// Adds a debug message to Out and fires message event.
        /// </summary>
        /// <param name="message"></param>
        public static void AddDebug(String message)
        {
            if (_out == null || !_debugMode) return;

            FTTConsoleMessage temp = new FTTConsoleMessage(message, MessageType.Debug);
            invokeConsoleMessage(temp);
            _out.Add(temp); 
        }

        /// <summary>
        /// Adds a standard info message to Out and fires message event.
        /// </summary>
        /// <param name="message"></param>
        public static void AddInfo(String message)
        {
            if (_out == null) return;

            FTTConsoleMessage temp = new FTTConsoleMessage(message, MessageType.Info);
            invokeConsoleMessage(temp);
            _out.Add(temp);
        }

        /// <summary>
        /// Adds an error message to Out and fires message event.
        /// </summary>
        /// <param name="message"></param>
        public static void AddError(String message)
        {
            if (_out == null) return;

            FTTConsoleMessage temp = new FTTConsoleMessage(message, MessageType.Error);
            invokeConsoleMessage(temp);
            _out.Add(temp);
        }

        private static void invokeConsoleMessage(FTTConsoleMessage message)
        {
            
            if (ConsoleMessage != null)
            {
                ConsoleMessage.Invoke(new ConsoleMessageEventArgs() { Message = message });
            }
        }

       

        public class ConsoleMessageEventArgs : EventArgs
        {
            public FTTConsoleMessage Message { get; set; }
        }

    }
}
