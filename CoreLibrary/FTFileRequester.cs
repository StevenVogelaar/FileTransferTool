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
    class FTFileRequester
    {

        public delegate void OperationFinishedHandler(object sender, EventArgs e);
        public event OperationFinishedHandler OperationFinished;

        // Flag variables.
        private String _currentFileAlias;
        private String _currentFolderAlias;
        private bool _isInDirectory;
        private long _currentFolderDownloaded;
        private bool _error;

        private Socket _socket;
        private List<FTTFileInfo> _files;
        private BackgroundWorker _senderWorker;
        private String _dest;
        private FTDownloadCallbacks _callbacks;
        private String _ip;
        private List<DirectoryInfo> _directoryDownloads;

        public FTFileRequester(List<FTTFileInfo> files, Socket socket, String destDirectory, FTDownloadCallbacks callbacks, String ip)
        {
            _ip = ip;
            _files = files;
            _socket = socket;
            _socket.ReceiveTimeout = 30000;
            _socket.SendTimeout = 30000;
            _dest = destDirectory;
            _callbacks = callbacks;
            _callbacks.DownloadStarted();
            _error = false;

            _directoryDownloads = new List<DirectoryInfo>();

            _senderWorker = new BackgroundWorker();
            _senderWorker.WorkerReportsProgress = true;
            _senderWorker.WorkerSupportsCancellation = true;
            _senderWorker.DoWork += senderWorker_DoWork;
            _senderWorker.ProgressChanged += senderWorker_ProgressChanged;
            _senderWorker.RunWorkerCompleted += senderWorker_WorkCompleted;
            
            _senderWorker.RunWorkerAsync();

        }


        /// <summary>
        /// Will attempt to cancel current download operations.
        /// </summary>
        public void Cancel()
        {
            _senderWorker.CancelAsync();
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
                    fileNames[i] = filesArray[i].Alias;
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
            return new FileStream(_dest + "/" + path, FileMode.Create);
        }

        /// <summary>
        /// Attempts to receive a file.
        /// </summary>
        private void receiveFile()
        {
            FileStream fileOut = null;
            string aliasWithPath = "";

            try
            {

                byte[] buffer = new byte[FTConnectionManager.PACKET_SIZE];
                int received = 0;
                long fileSize;
				int totalReceived = 0;

				while (true){
                	// Try to receive file name and size of file to be received.
					if ((received = _socket.Receive(buffer, received, FTConnectionManager.PACKET_SIZE - totalReceived, SocketFlags.None)) <= 0)
                	{
                   	 	return;
                	}

					totalReceived += received;
					if (totalReceived == FTConnectionManager.PACKET_SIZE) break;

				}

                // Get first 8 bytes which is the file size.
                byte[] sizeInBytes = new byte[8];
                for (int i = 0; i < 8; i++)
                {
                    sizeInBytes[i] = buffer[i];
                }

                // Convert first 8 bytes into long.
                fileSize = BitConverter.ToInt64(sizeInBytes, 0);

                // Get the file alias which is in the rest of the PACKET_SIZE bytes.
                aliasWithPath = Encoding.UTF8.GetString(buffer, 8, FTConnectionManager.PACKET_SIZE - 9);
                aliasWithPath = aliasWithPath.Replace("\0", String.Empty);
                // currentFileAlias is used to send progress on a file to the callback class.
                _currentFileAlias = aliasWithPath.Substring(aliasWithPath.LastIndexOf('/') + 1);

                FTTConsole.AddDebug("File alias received: " + aliasWithPath);

                // Get the file path without the file.
				int tempthing = aliasWithPath.LastIndexOf('/');
                String path = _dest  + aliasWithPath.Substring(0,aliasWithPath.LastIndexOf('/'));
                // Create Directories for the path.
                Directory.CreateDirectory(path);


                // Set isInDirectory flag if the file path has not '\'. i.e. \subfolder\ is not \.
                if (aliasWithPath.LastIndexOf('/') != 0)
                {

                    _isInDirectory = true;
                    _currentFolderAlias = aliasWithPath.Split('/')[1];


                    bool found = false;
                    foreach (DirectoryInfo directory in _directoryDownloads)
                    {
                        if (directory.Alias.Equals(_currentFolderAlias))
                        {
                            found = true;
                            _currentFolderDownloaded = directory.Downloaded;
                            break;
                        }
                    }

                    if (!found)
                    {
                        // Set downloaded bytes to 0 for current folder download.
                        _directoryDownloads.Add(new DirectoryInfo() { Alias = _currentFolderAlias, Downloaded = 0 });
                        _currentFolderDownloaded = 0;
                    }

                    FTTConsole.AddDebug("current folder alias: " + _currentFolderAlias);
                }
                else _isInDirectory = false;
 
                // Create the output file.
				String tempytemp = _dest  + aliasWithPath;
                fileOut = new FileStream(_dest  + aliasWithPath, FileMode.Create);

                // Write to the output file.
                long bytesReceived = 0;

                // First try to receive either the max chunk size or the filesize if it is smaller than the max chunk size.
                int nextReceive = (int)Math.Min(FTConnectionManager.PACKET_SIZE, fileSize);
                int count = 1;

                // Receive and write the file.
                while (bytesReceived < fileSize && (received = _socket.Receive(buffer, nextReceive, SocketFlags.None )) > 0 )
                {
                    fileOut.Write(buffer, 0, received);
                    bytesReceived += received;
                    _currentFolderDownloaded += received;

                    // Report progress.
                    if (count % 400 == 0)
                    {
                        if (!_isInDirectory)
                        {
                            _senderWorker.ReportProgress((int)((bytesReceived / (double)fileSize) * 100));
                        }
                        else
                        {
                            _senderWorker.ReportProgress(-1, _currentFolderDownloaded);
                        }

                        // Check for cancelation.
                        if (_senderWorker.CancellationPending)
                        {
                            throw new OperationCanceledException("Operation Canceled");
                        }
                    }

                    count++;

                    // This is so that it doesnt receive the header of the next file.
                    nextReceive = (int)Math.Min(fileSize - bytesReceived, FTConnectionManager.PACKET_SIZE);
                }

				Console.WriteLine("Received: " + bytesReceived);
                if (_isInDirectory)
                {
                    foreach (DirectoryInfo directory in _directoryDownloads)
                    {
                        directory.Downloaded = _currentFolderDownloaded;
                    }
                }

                if (_isInDirectory) {
                    _senderWorker.ReportProgress(-1, _currentFolderDownloaded);
                }
                else
                {
                    _senderWorker.ReportProgress(100);
                }
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode == SocketError.TimedOut)
                {
                    FTTConsole.AddError("Error receiving file: Connection timed out.");
                    _error = true;
                }

                FTTConsole.AddError("Error receiving file: Socket Exception");
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
                deleteFile(_dest + "/" + aliasWithPath);
                _error = true;

                return;
            }
            catch (OperationCanceledException e)
            {
                FTTConsole.AddInfo("Download opertations have been canceled.");

                fileOut.Close();
                fileOut.Dispose();

                deleteFile(_dest + "/" + aliasWithPath);

                return;
            }
            catch (Exception e)
            {

                FTTConsole.AddError("Error receiving file.");
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
                deleteFile(_dest + "/" + aliasWithPath);
                _error = true;

                return;
            }
            finally
            {
                // Cleanup this instance.
                if (fileOut != null)
                {
                    fileOut.Close();
                    fileOut.Dispose();
                }
            }

            FTTConsole.AddInfo("File received: " + aliasWithPath);


            // Try to receive another file if there is one.
            receiveFile();
        }

        private void deleteFile(String path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception e)
            {
                FTTConsole.AddError("Could not delete canceled file. Please delete manualy.");
            }
        }


        private void senderWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            sendRequest();
        }

        private void senderWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage < 0)
            {
                // File is part of a directory download. So only report the ammount that has been downloaded.
                _callbacks.FolderDownloadProgress(_currentFolderAlias, (long)e.UserState, _ip);
            }
            else
            {
                _callbacks.DownloadProgress(_currentFileAlias, e.ProgressPercentage, _ip);
            }
        }

        private void senderWorker_WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            _callbacks.DownloadCompleted(_ip, _error);
        }


        private void dispose()
        {
            FTTConsole.AddDebug("Shutting down connection in requester");
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                _socket.Dispose();
            }
            catch (Exception e)
            {
                FTTConsole.AddError("Could not shutdown the remote connection.");
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }

            if (OperationFinished != null)
            {
                OperationFinished.Invoke(this, EventArgs.Empty);
            }
        }


        public class CanceledOperation : Exception
        {

        }

       
        public class DirectoryInfo
        {
            public String Alias { get; set; }
            public long Downloaded { get; set; }
        }

    }
}
