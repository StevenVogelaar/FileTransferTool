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
    class FTConnectionManager
    {

        public const Int32 FILETRANSFER_PORT = 3897;
        public const Int32 RECEIVING_PORT = 3898;

        private Socket _connectionReceiver;
        private IPEndPoint _endPoint;
        private Thread _listenThread;
        private FTFileSender.GetFilePath _getFilePath;


        public FTConnectionManager(FTFileSender.GetFilePath getFilePath)
        {
            _endPoint = new IPEndPoint(BroadcastManager.LocalIPAddress(), FILETRANSFER_PORT);
            //_listeners = new List<ConnectionListener>();

            _getFilePath = getFilePath;

            _listenThread = new Thread(listenForRequests);
            _listenThread.IsBackground = true;
            _listenThread.Start();
        }


        /// <summary>
        /// Creates a FTFileRequester for a list of files from the same remote host.
        /// </summary>
        /// <param name="files">List of files that all have the same ip address.</param>
        public void DownloadFile(List<FTTFileInfo> files, String dest)
        {
            // Switch this to run on a seperate thread!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // Attempt to open connection with remote host.
            try
            {
                Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(new IPEndPoint(BroadcastManager.LocalIPAddress(), RECEIVING_PORT));
                socket.Connect(new IPEndPoint(IPAddress.Parse(files.ElementAt(0).IP), FILETRANSFER_PORT));

                // Create new fileRequester. It will run the request automaticaly.
                foreach (FTTFileInfo f in files)
                {
                    FTTConsole.AddDebug("Sending request for files: " + f.Name);
                }

                new FTFileRequester(files, socket, dest);
            }
            catch (Exception e)
            {
                FTTConsole.AddError("Error trying to connect to remmote host: " + files.ElementAt(0).IP);
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
            }
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


        private void connectionAccepted(Socket socket)
        {
            // Create FTFileSender which then runs on a background thread.
            new FTFileSender(socket, _getFilePath);
        }

        private void listener_fileReceivedCallback()
        {
            
        }


    }
}
