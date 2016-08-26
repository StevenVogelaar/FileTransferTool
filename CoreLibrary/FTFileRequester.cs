using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace CoreLibrary
{
    class FTFileRequester
    {

        private Socket _socket;
        private String _file;
        private Thread _senderThread;
        private Thread _receiverThreader;
        private String _dest;
        private int _count;

        public FTFileRequester(String file, Socket socket, String destDirectory)
        {
            _count = 0;
            _file = file;
            _socket = socket;
            _socket.ReceiveTimeout = 5000;
            _socket.SendTimeout = 5000;
            _dest = destDirectory;

            _senderThread = new Thread(sendRequest);
            _senderThread.IsBackground = true;
            _senderThread.Start();
        }

        /// <summary>
        /// Send the file request to the other client.
        /// </summary>
        private void sendRequest()
        {
            try
            {
                byte[] buffer = new byte[2048];
                byte[] fName = Encoding.UTF8.GetBytes(_file);

                for (int ii = 0; ii < fName.Length; ii++)
                {
                    buffer[ii] = fName[ii];
                }

                _socket.Send(buffer); 
            }
            catch (SocketException e)
            {

                if (e.SocketErrorCode == SocketError.TimedOut)
                {
                    FTTConsole.AddError("Error receiving file: Connection timed out.");
                }
                else FTTConsole.AddError("Error sending request for file.");


                Console.WriteLine(e.Message + "\n" + e.StackTrace);

                dispose();
                return;
            }


            beginReceive();
        }

        private void beginReceive()
        {
            _receiverThreader = new Thread(receiveFile);
            _receiverThreader.IsBackground = true;
            _receiverThreader.Start();
        }

        /// <summary>
        /// Attempts to receive a file.
        /// </summary>
        private void receiveFile()
        {

            FileStream fileOut = null;

            try
            {

                byte[] buffer = new byte[2048];
                int received = 0;

                // Try to receive file name.
                received = _socket.Receive(buffer);
                String fName = Encoding.UTF8.GetString(buffer);
                fName = fName.Replace("\0", String.Empty);
                Console.WriteLine("Filename received: " + fName);

                fileOut = new FileStream(_dest + "\\" + fName, FileMode.Create);

                while (received > 0)
                {
                    received = _socket.Receive(buffer);
                    fileOut.Write(buffer, 0, buffer.Length);
                }   
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode == SocketError.TimedOut)
                {
                    FTTConsole.AddError("Error receiving file: Connection timed out.");
                }

                FTTConsole.AddError("Error receiving file: Socket Exception");
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }
            catch (Exception e)
            {

                FTTConsole.AddError("Error receiving file.");
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }

            FTTConsole.AddInfo("File received: " + _file);

            // Cleanup this instance.
            if (fileOut != null)
            {
                fileOut.Close();
            }
            dispose();
        }


        private void dispose()
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
    }
}
