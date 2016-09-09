using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.ComponentModel;

namespace CoreLibrary
{
    /// <summary>
    /// Listens for connections from other clients for requesting files.
    /// </summary>
    class FTFileSender
    {

        public delegate void OperationFinishedHandler(object sender, EventArgs e);
        public event OperationFinishedHandler OperationFinished;

        public delegate String GetFilePath(String fileName);

        private Socket _socket;
        private GetFilePath _getFilePath;
        private BackgroundWorker _senderWorker;

        public FTFileSender(Socket socket, GetFilePath getFilePath)
        {
            _socket = socket;;
            _getFilePath = getFilePath;


            _senderWorker = new BackgroundWorker();
            _senderWorker.DoWork += _senderWorker_DoWork;
            _senderWorker.WorkerReportsProgress = false;
            _senderWorker.WorkerSupportsCancellation = true;

            _senderWorker.RunWorkerAsync();

        }


        private void _senderWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            listen();
        }


        /// <summary>
        /// Will attempt to cancel current upload operations.
        /// </summary>
        public void Cancel()
        {
            _senderWorker.CancelAsync();
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
                        sendFile(path, "");
                    }
                    else if (Directory.Exists(path))
                    {
                        sendFolder(path, "");
                    }
                    else
                    {
                        FTTConsole.AddError("Could not find file: " + file);
                    }
                }
                else
                {
                    FTTConsole.AddError("Null Path for file: " + file);
                }
            }

            dispose();
        }

        /// <summary>
        /// Begins an operation to send the contents of a folder and all sub-folders over the socket.
        /// </summary>
        /// <param name="directoryPath">The main path for the folder that is being downloaded.</param>
        /// <param name="relativePath">Path to the current subfolder.</param>
        private void sendFolder(String directoryPath, String relativePath)
        {

            String[] files = Directory.GetFiles(directoryPath);
            String directoryName = directoryPath.Substring(directoryPath.LastIndexOf('/'));

            // Send each file in current directory.
            foreach (String f in files)
            {
                string linuxString = f.Replace('\\', '/');
                sendFile(linuxString, relativePath +  directoryName);
            }

            // Recurse for each subdirectory.
            String[] subDirectories = Directory.GetDirectories(directoryPath);
            foreach (string d in subDirectories)
            {
                string linuxString = d.Replace('\\', '/');
                sendFolder(linuxString, relativePath + directoryName);
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
            String fileName = path.Substring(path.LastIndexOf('/'));
            try
            {

                // Try to open file.
                fileStream = File.OpenRead(path);
                // Get size of file.
                FileInfo fileInfo = new FileInfo(path);
                long size = fileInfo.Length;
                byte[] sizeInBytes = BitConverter.GetBytes(size);

                byte[] buffer = new byte[FTConnectionManager.PACKET_SIZE];

                // Set first 8 bytes to the file size.
                for (int i = 0; i < 8; i++)
                {
                    buffer[i] = sizeInBytes[i];
                }

                byte[] fName = Encoding.UTF8.GetBytes(relativePath + fileName);
                
                // Set rest of bytes in FTConnectionManager.PACKET_SIZE byte message to the file name with relative path.
                for (int i = 8; i < fName.Length + 8; i++)
                {
                    buffer[i] = fName[i - 8];
                }

                // Send header info.
                _socket.Send(buffer);

                // Now send the file.
                int readBytes = 0;
                buffer = new byte[FTConnectionManager.PACKET_SIZE];
                SocketError sError;
                while ((readBytes = fileStream.Read(buffer, 0, buffer.Length)) > 0 && _socket.Connected){
                    int value = _socket.Send(buffer, 0, readBytes, SocketFlags.None, out sError);
                    
                    // Check to see if operation was canceled.
                    if (_senderWorker.CancellationPending)
                    {
                        throw new OperationCanceledException("Operation has been canceled");
                    }
                }

                FTTConsole.AddInfo("File sent: " + fileName);
            }
            catch (ArgumentException e)
            {
                FTTConsole.AddError("Null Path for file: " + fileName);
            }
            catch (OperationCanceledException e)
            {
                FTTConsole.AddInfo("Upload opertations have been canceled.");
                return;
            }
            catch (Exception e)
            {
                FTTConsole.AddError("Error sending file: " + e.Message);
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
        }

        private void dispose()
        {
            // Shutdown causes the other client to stop listening for files.
            
			_socket.Shutdown(SocketShutdown.Send);

			byte[] buffer = new byte[FTConnectionManager.PACKET_SIZE];
			int received = 0;
			_socket.ReceiveTimeout = 1;

            try
            {
                while ((received = _socket.Receive(buffer)) > 0)
                {
                    String message = Encoding.UTF8.GetString(buffer).Replace("\0", String.Empty);
                    if (message.Equals("FIN")) break;
                    Thread.Sleep(500);
                }
            }
            catch (SocketException e)
            {
                FTTConsole.AddDebug("Issue with receiving FIN message from client.");
            }
				
            _socket.Close();
            _socket.Dispose();

			FTTConsole.AddDebug("Connection shut down.");
            
            if (OperationFinished != null)
            {
                OperationFinished.Invoke(this, EventArgs.Empty);
            }
        }

		bool SocketConnected(Socket s)
		{
			bool part1 = s.Poll(1000, SelectMode.SelectWrite);
			bool part2 = (s.Available == 0);
			if ((part1 && part2 ) || !s.Connected)
				return false;
			else
				return true;
		}

		public static bool IsConnected(Socket client)
		{
			// This is how you can determine whether a socket is still connected.
			bool blockingState = client.Blocking;

			try
			{
				byte[] tmp = new byte[1];

				client.Blocking = false;
				client.Send(tmp, 0, 0);
				return true;
			}
			catch (SocketException e)
			{
				// 10035 == WSAEWOULDBLOCK
				if (e.NativeErrorCode.Equals(10035))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			finally
			{
				client.Blocking = blockingState;
			}
		}

        public class CanceledOperation : Exception
        {

        }
    }
}
