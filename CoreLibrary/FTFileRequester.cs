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
    class FTFileRequester
    {

        private Socket _socket;
        private String _fileName;
        private Thread _senderThread;
        private Thread _receiverThreader;

        public FTFileRequester(String fileName, Socket socket)
        {
            _fileName = fileName;
            _socket = socket;

            _senderThread = new Thread(sendRequest);
            _senderThread.IsBackground = true;
            _senderThread.Start();

            _receiverThreader = new Thread(receiveFile);
            _receiverThreader.IsBackground = true;
            _senderThread.Start();
        }


        private void sendRequest()
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(_fileName);
                _socket.Send(buffer);
            }
            catch (Exception e)
            {
                FTTConsole.AddError("Error sending request for file.");
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }
        }

        private void receiveFile()
        {
            try
            {
                byte[] buffer = new byte[2048];
                int received;

                received = _socket.Receive(buffer);

                Console.WriteLine("File Received: " + received);
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode == SocketError.TimedOut)
                {
                    FTTConsole.AddError("Error receiving file: Connection timed out.");
                }

                FTTConsole.AddError("Error receiving file.");
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }
        }
    }
}
