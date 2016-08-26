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
    /// <summary>
    /// Listens for connections from other clients for requesting files.
    /// </summary>
    class FTFileSender
    {
        public delegate void FileReceivedCallback();
        public delegate String GetFilePath(String fileName);

        private Socket _socket;
        private Thread _listenerThread;
        private Thread _senderThread;
        private FileReceivedCallback _callback;
        private GetFilePath _getFilePath;

        public FTFileSender(Socket socket, FileReceivedCallback callback, GetFilePath getFilePath)
        {
            _socket = socket;
            _callback = callback;
            _getFilePath = getFilePath;

            _listenerThread = new Thread(listen);
            _listenerThread.IsBackground = true;
            _listenerThread.Start();
        }


        private void listen()
        {

            String requestedfile = "";

            try
            {
                byte[] buffer = new byte[2048];
                int received = _socket.Receive(buffer);
                String message = Encoding.UTF8.GetString(buffer);

                requestedfile = message.Replace("\0", String.Empty);

                FTTConsole.AddDebug("File requested: " + message);
                //Console.WriteLine("File Requested = " + message);

            }
            catch (Exception e)
            {

                FTTConsole.AddError("Error retreiving message sent from client.");
                Console.WriteLine(e.Message + "\n" + e.StackTrace);

                dispose();
                return;
            }

            Console.WriteLine("Being send file.");
            beginSendFile(requestedfile);
        }


        private void beginSendFile(String file)
        {
            _senderThread = new Thread(sendFile);
            _senderThread.IsBackground = true;
            _senderThread.Start(file);
        }

        private void sendFile(object fileNameArg)
        {

            FileStream fileStream = null;
            String fileName = null;
            try
            {

                fileName = (String)fileNameArg;

                // Try to open file.
                fileStream = File.OpenRead(_getFilePath(fileName));
                FileStream writeStream = new FileStream("./outfile.txt", FileMode.Create);


                byte[] fName = Encoding.UTF8.GetBytes(fileName);
                byte[] buffer = new byte[2048];

                // Send the name of the file in the first 2048 byte message.
                for (int i = 0; i < fName.Length; i++)
                {
                    buffer[i] = fName[i];
                }
                _socket.Send(Encoding.UTF8.GetBytes(fileName));

                // Now send the file.
                int readBytes = 0;
                buffer = new byte[2048];
                SocketError sError;
                while ((readBytes = fileStream.Read(buffer, 0, buffer.Length)) > 0){
                    writeStream.Write(buffer, 0, readBytes);
                    _socket.Send(buffer, 0, readBytes, SocketFlags.None, out sError);
                    
                }

                FTTConsole.AddInfo("File sent: " + fileName);
            }
            catch (ArgumentException e)
            {
                FTTConsole.AddError("Null Path for file: " + fileName);
            }
            catch (Exception e)
            {
                FTTConsole.AddError("Error sending file: " + e.Message);
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }

            if (fileStream != null)
            {
                fileStream.Close();
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
