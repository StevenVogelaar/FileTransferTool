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
    class FTConnectionManager : IDisposable
    {

        public delegate void ConnectionFailedHandler(object sender, ConnectionFailedEventArgs e);
        public event ConnectionFailedHandler ConnectionFailed;

        public const Int32 FILETRANSFER_PORT = 3897;
        //public const Int32 RECEIVING_PORT = 3898;
        public const int PACKET_SIZE = 1460;

        private Socket _connectionReceiver;
        private IPEndPoint _endPoint;
        private Thread _listenThread;
        private FTFileSender.GetFilePath _getFilePath;

        private List<FTFileRequester> _requesters;
        private List<FTFileSender> _senders;


        public FTConnectionManager(FTFileSender.GetFilePath getFilePath)
        {
            _endPoint = new IPEndPoint(BroadcastManager.LocalIPAddress(), FILETRANSFER_PORT);
            _requesters = new List<FTFileRequester>();
            _senders = new List<FTFileSender>();

            _getFilePath = getFilePath;
            _listenThread = new Thread(listenForRequests);
            _listenThread.IsBackground = true;
            _listenThread.Start();
        }


        /// <summary>
        /// Stops all download operations.
        /// </summary>
        public void CancelDownloads()
        {
            lock (_requesters)
            {
                foreach (FTFileRequester requester in _requesters)
                {
                    requester.Cancel();
                }
            }
        }

        /// <summary>
        /// Stops all upload operations.
        /// </summary>
        public void CanceUploads()
        {
            lock (_senders)
            {
                foreach (FTFileSender sender in _senders)
                {
                    sender.Cancel();
                }
            }
        }


        /// <summary>
        /// Checks for current upload operations.
        /// </summary>
        /// <returns>True if there are current upload operations.</returns>
        public bool CurrentUploadOperations()
        {
            if (_senders.Count > 0)
            {
                return true;
            }
            else return false;
        }


        /// <summary>
        /// Creates a FTFileRequester for a list of files from the same remote host.
        /// </summary>
        /// <param name="files">List of files that all have the same ip address.</param>
        public void DownloadFile(List<FTTFileInfo> files, String dest, FTDownloadCallbacks callbacks)
        {
            DownloadStarter downloadStarter = new DownloadStarter(files, dest, callbacks, _requesters, requester_OperationFinished, this);

            Thread thread = new Thread(downloadStarter.DownloadFile);
            thread.IsBackground = true;
            thread.Start();
        }


        /// <summary>
        /// Accepts incomming requests. These requests will be other clients trying to download files.
        /// </summary>
        private void listenForRequests()
        {
            // Initialize listener socket.
            try
            {
                _connectionReceiver = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _connectionReceiver.Bind(_endPoint);
                _connectionReceiver.Listen(10);
            }
            catch (Exception e)
            {
                FTTConsole.AddError("Error listening for file transfer connections.");
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }

            // Accept incomming connection requests.
            while (true)
            {
                try
                {
                    Socket socket = _connectionReceiver.Accept();
                    connectionAccepted(socket);
                }
                catch (Exception e)
                {
                    FTTConsole.AddError("Error accepting a file transfer connection.");
                    Console.WriteLine(e.Message + "\n" + e.StackTrace);
                }
            }
        }

        /// <summary>
        /// Thread safe method for removing senders from the list.
        /// </summary>
        /// <param name="sender"></param>
        private void removeSender(FTFileSender sender)
        {
            lock (_senders)
            {
                _senders.Remove(sender);
            }
        }

        /// <summary>
        /// Thread safe method for removing requesters from the list.
        /// </summary>
        /// <param name="requester"></param>
        private void removeRequester(FTFileRequester requester)
        {
            lock (_requesters)
            {
                _requesters.Remove(requester);
            }
        }

        private void connectionAccepted(Socket socket)
        {
            // Create FTFileSender which then runs on a background thread.
            FTFileSender sender = new FTFileSender(socket, _getFilePath);
            sender.OperationFinished += sender_OperationFinished;

            lock (_senders)
            {
                _senders.Add(sender);
            }
        }


        /// <summary>
        /// Calls remover method with the event invoker.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void requester_OperationFinished(object sender, EventArgs e)
        {
            removeRequester((FTFileRequester)sender);
        }

        /// <summary>
        /// Calls remover method with the event invoker.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sender_OperationFinished(object sender, EventArgs e)
        {
            removeSender((FTFileSender)sender);
        }


        public void Dispose()
        {

            lock (_requesters)
            {
                foreach (FTFileRequester requester in _requesters)
                {
                    requester.Cancel();
                }

                _requesters.Clear();
            }

            lock (_senders)
            {
                foreach (FTFileSender sender in _senders)
                {
                    sender.Cancel();
                }

                _senders.Clear();
            }

            _connectionReceiver.Close();
            _connectionReceiver.Dispose();

            // Give chance for requesters/receivers to shutdown.
            Thread.Sleep(500);
        }

        public void invokeConnectionFailed(String ip)
        {
            if (ConnectionFailed != null)
            {
                ConnectionFailed.Invoke(this, new ConnectionFailedEventArgs() { IP = ip });
            }
        }


        public class DownloadStarter
        {
            public delegate void requester_OperationFinished(object sender, EventArgs e);

            private List<FTTFileInfo> _files;
            private String _dest;
            private FTDownloadCallbacks _callbacks;
            private List<FTFileRequester> _requesters;
            private requester_OperationFinished _callback;
            private FTConnectionManager _manager;

            public DownloadStarter(List<FTTFileInfo> files, String dest, FTDownloadCallbacks callbacks, List<FTFileRequester> requesters, requester_OperationFinished callback, FTConnectionManager manager)
            {
                _files = files;
                _dest = dest;
                _callbacks = callbacks;
                _requesters = requesters;
                _callback = callback;
                _manager = manager;
            }

            public void DownloadFile()
            {
                // Attempt to open connection with remote host.
                try
                {
					Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					Console.WriteLine("AWKAWKFWKAWF " + IPAddress.Parse(_files.ElementAt(0).IP));
                    socket.Connect(IPAddress.Parse(_files.ElementAt(0).IP), FILETRANSFER_PORT);

                    // Create new fileRequester. It will run the request automaticaly.
                    foreach (FTTFileInfo f in _files)
                    {
                        FTTConsole.AddDebug("Sending request for files: " + f.Name);
                    }

                    // Create requester
                    FTFileRequester requester = new FTFileRequester(_files, socket, _dest, _callbacks, _files.ElementAt(0).IP);
                    requester.OperationFinished += delegate (object sender, EventArgs e){ _callback.Invoke(sender, e); };

                    lock (_requesters)
                    {
                        _requesters.Add(requester);
                    }
                }
                catch (Exception e)
                {
                    FTTConsole.AddError("Error trying to connect to remote host: " + _files.ElementAt(0).IP);
                    Console.WriteLine(e.Message + "\n" + e.StackTrace);

                    _manager.invokeConnectionFailed(_files.ElementAt(0).IP);
                }
            }
        }

        public class ConnectionFailedEventArgs : EventArgs
        {
            public String IP { get; set; }
        }
    }
}
