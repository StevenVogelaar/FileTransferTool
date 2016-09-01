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


            Receive();
        }

        private void Receive()
        {

            receiveFile();

            dispose();
        }

        private FileStream createFile(String path)
        {
            return new FileStream(_dest + "\\" + path, FileMode.Create);
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
                long fileSize;

                // Try to receive file name and number of chunks that are too be received for the file.
                received = _socket.Receive(buffer);

                // Get first 8 bytes which is the file size.
                byte[] sizeInBytes = new byte[8];
                for (int i = 0; i < 8; i++)
                {
                    sizeInBytes[i] = buffer[i];
                }

                // Convert first 8 bytes into long.
                fileSize = BitConverter.ToInt64(sizeInBytes, 0);


                // Get the filename which is in the rest of the 2048 bytes.
                String fName = Encoding.UTF8.GetString(buffer, 8, 2039);
                fName = fName.Replace("\0", String.Empty);

                FTTConsole.AddDebug("Filename received: " + fName);

                // Get the file path without the file.
                String path = fName.Substring(0,fName.LastIndexOf('\\'));
                // Create Directories for the path.
                Directory.CreateDirectory(path);
 
                // Create the output file.
                fileOut = new FileStream(_dest + "\\" + fName, FileMode.Create);


                // Write to the output file.
                long bytesReceived = 0;
                while ((received = _socket.Receive(buffer)) > 0 && bytesReceived < fileSize)
                {
                    fileOut.Write(buffer, 0, received);
                    fileSize += received;
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

        }


        private void dispose()
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
    }
}
