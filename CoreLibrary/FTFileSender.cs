using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CoreLibrary
{
    /// <summary>
    /// Listens for connections from other clients for requesting files.
    /// </summary>
    class FTFileSender
    {
        public delegate void DataReceivedCallback();

        private Socket _socket;
        private Thread _listenerThread;
        private Thread _senderThread;
        private DataReceivedCallback _callback;

        public FTFileSender(Socket socket, DataReceivedCallback callback)
        {
            _socket = socket;
            _callback = callback;

            _listenerThread = new Thread(listen);
            _listenerThread.IsBackground = true;
            _listenerThread.Start();

            _listenerThread = new Thread(listen);
            _listenerThread.IsBackground = true;
            _listenerThread.Start();
        }


        private void listen()
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[2048];
                    int received = _socket.Receive(buffer);
                    String message = buffer.ToString();

                    FTTConsole.AddDebug("File requested: " + message);
                    Console.WriteLine("File Requested = " + message);

                    beginSendFile(message);
                }
            }
            catch (Exception e)
            {
                FTTConsole.AddError("Error retreiving message sent from client.");
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }
        }


        private void beginSendFile(String file)
        {
            _senderThread = new Thread(sendFile);
            _senderThread.IsBackground = true;
            _senderThread.Start(file);
        }

        private void sendFile(object fileNameArg)
        {
            try {

                String fileName = (String)fileNameArg;
                _socket.Send(Encoding.UTF8.GetBytes(fileName));
            }
            catch (Exception e)
            {
                FTTConsole.AddError("Error sending file.");
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }
        }


    }
}
