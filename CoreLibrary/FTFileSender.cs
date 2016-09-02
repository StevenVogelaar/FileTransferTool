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
            List<String> files = null;

            try
            {
                byte[] buffer = new byte[4096];
                int received = _socket.Receive(buffer);
                String message = Encoding.UTF8.GetString(buffer);

                message = message.Replace("\0", String.Empty);

                // Parse message into individual strings.
                files = parseMessage(message);

                FTTConsole.AddDebug("File requested: " + message);
            }
            catch (Exception e)
            {

                FTTConsole.AddError("Error retreiving message sent from client.");
                Console.WriteLine(e.Message + "\n" + e.StackTrace);

                dispose();
                return;
            }

            beginSendFile(files);
        }

        private List<String> parseMessage(String message)
        {
            String[] files = message.Split('/');
            return new List<String>(files);
        }


        private void beginSendFile(List<String> files)
        {

            foreach (String file in files)
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
                        sendFolder(path, ".\\");
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
            }

            dispose();
        }

        /// <summary>
        /// Begins an operation to send the contents of a folder and all sub-folders over the socket.
        /// </summary>
        /// <param name="directoryPath"></param>
        private void sendFolder(String directoryPath, String relativePath)
        {

            String[] files = Directory.GetFiles(directoryPath);
            String directoryName = directoryPath.Substring(directoryPath.LastIndexOf('\\'));

            // Send each file in current directory.
            foreach (String f in files)
            {
                sendFile(f, relativePath + directoryName);
            }

            // Recurse for each subdirectory.
            String[] subDirectories = Directory.GetDirectories(directoryPath);
            foreach (string d in subDirectories)
            {
                sendFolder(d, relativePath + directoryName);
            }
        }

        /// <summary>
        /// Sends single file over the socket.
        /// </summary>
        /// <param name="path">Local Path of file.</param>
        /// <param name="relativePath">Relative path from dest directory.</param>
        private void sendFile(String path, String relativePath)
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
                    int value = _socket.Send(buffer, 0, readBytes, SocketFlags.None, out sError);
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

            //Thread.Sleep(200);
        }

        private void dispose()
        {
            // Shutdown causes the other client to stop listening for files.
            FTTConsole.AddDebug("Connection shutting down.");
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
    }
}
