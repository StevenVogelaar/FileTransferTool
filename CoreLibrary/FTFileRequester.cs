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
        private List<FTTFileInfo> _files;
        private Thread _senderThread;
        private String _dest;

        public FTFileRequester(List<FTTFileInfo> files, Socket socket, String destDirectory)
        {
            _files = files;
            _socket = socket;
            _socket.ReceiveTimeout = 5000;
            _socket.SendTimeout = 5000;
            _dest = destDirectory;

            _senderThread = new Thread(sendRequest);
            _senderThread.IsBackground = true;
            _senderThread.Start();
        }

        /// <summary>
        /// Send the files request to the other client.
        /// </summary>
        private void sendRequest()
        {
            try
            {
                byte[] buffer = new byte[4096];
                String[] fileNames = new String[_files.Count];
                FTTFileInfo[] filesArray = _files.ToArray();
                for (int i = 0; i < fileNames.Length; i++)
                {
                    fileNames[i] = filesArray[i].Name;
                }

                String combined = "";

                // Combine names into single string
                for (int i = 0; i < fileNames.Length; i++)
                {
                    if (i > 0)
                    {
                        combined = combined + "/" + fileNames[i];
                    }
                    else combined = combined + fileNames[i];
                }

                byte[] combinedBytes = Encoding.UTF8.GetBytes(combined);


                for (int i = 0; i < combined.Length && i < buffer.Length; i++)
                {
                    buffer[i] = combinedBytes[i];
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
            string fName = "";

            try
            {

                byte[] buffer = new byte[2048];
                int received = 0;
                long fileSize;

                // Try to receive file name and size of file to be received.
                if ((received = _socket.Receive(buffer)) <= 0)
                {
                    return;
                }


                // Get first 8 bytes which is the file size.
                byte[] sizeInBytes = new byte[8];
                for (int i = 0; i < 8; i++)
                {
                    sizeInBytes[i] = buffer[i];
                }

                // Convert first 8 bytes into long.
                fileSize = BitConverter.ToInt64(sizeInBytes, 0);

                // Get the filename which is in the rest of the 2048 bytes.
                fName = Encoding.UTF8.GetString(buffer, 8, 2039);
                fName = fName.Replace("\0", String.Empty);

                FTTConsole.AddDebug("Filename received: " + fName);

                // Get the file path without the file.
                String path = _dest + "\\" + fName.Substring(0,fName.LastIndexOf('\\'));
                // Create Directories for the path.
                Directory.CreateDirectory(path);
 
                // Create the output file.
                fileOut = new FileStream(_dest + "\\" + fName, FileMode.Create);

                // Write to the output file.
                long bytesReceived = 0;

                // First try to receive either the max chunk size or the filesize if it is smaller than the max chunk size.
                int nextReceive = (int)Math.Min(2048, fileSize);

                while (bytesReceived < fileSize && (received = _socket.Receive(buffer, nextReceive, SocketFlags.None )) > 0 )
                {
                    fileOut.Write(buffer, 0, received);
                    bytesReceived += received;

                    // This is so that it doesnt receive the header of the next file.
                    nextReceive = (int)Math.Min(fileSize - bytesReceived, 2048);
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
                return;
            }
            catch (Exception e)
            {

                FTTConsole.AddError("Error receiving file.");
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
                return;
            }

            FTTConsole.AddInfo("File received: " + fName);

            // Cleanup this instance.
            if (fileOut != null)
            {
                fileOut.Close();
            }

            // Try to receive another file if there is one.
            receiveFile();
        }


        private void dispose()
        {
            FTTConsole.AddDebug("Shutting down connection in requester");
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
    }
}
