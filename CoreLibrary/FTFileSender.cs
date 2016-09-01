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
        private GetFilePath _getFilePath;

        public FTFileSender(Socket socket, GetFilePath getFilePath)
        {
            _socket = socket;
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
            String path = _getFilePath(file);

            if (path != null)
            {
                if (File.Exists(path))
                {
                    sendFile(path, ".\\");
                }
                else if (Directory.Exists(path))
                {
                    sendFolder(path);
                }
                else
                {
                    FTTConsole.AddError("Could not find file: " + file);
                    dispose();
                    return;
                }
            }
            else
            {
                FTTConsole.AddError("Null Path for file: " + file);
                dispose();
                return;
            }

            dispose();
        }

        /// <summary>
        /// Begins an operation to send the contents of a folder and all sub-folders over the socket.
        /// </summary>
        /// <param name="path"></param>
        private void sendFolder(String path)
        {


        }

        /// <summary>
        /// Sends single file over the socket.
        /// </summary>
        /// <param name="path">Local Path of file.</param>
        /// <param name="relativePath">Relative path from dest directory.</param>
        private void sendFile(String path, string relativePath)
        {

            FileStream fileStream = null;
            String fileName = path.Substring(path.LastIndexOf('\\'));
            try
            {

                // Try to open file.
                fileStream = File.OpenRead(path);
                // Get size of file.
                FileInfo fileInfo = new FileInfo(path);
                long size = fileInfo.Length;
                byte[] sizeInBytes = BitConverter.GetBytes(size);

                byte[] buffer = new byte[2048];

                // Set first 8 bytes to the file size.
                for (int i = 0; i < 8; i++)
                {
                    buffer[i] = sizeInBytes[i];
                }

                byte[] fName = Encoding.UTF8.GetBytes(relativePath + fileName);
                
                // Set rest of bytes in 2048 byte message to the file name with relative path.
                for (int i = 8; i < fName.Length + 8; i++)
                {
                    buffer[i] = fName[i - 8];
                }

                // Send header info.
                _socket.Send(buffer);

                // Now send the file.
                int readBytes = 0;
                buffer = new byte[2048];
                SocketError sError;
                while ((readBytes = fileStream.Read(buffer, 0, buffer.Length)) > 0){
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

        }

        private void dispose()
        {
            // Shutdown causes the other client to stop listening for files.
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
    }
}
