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
        private int _receivingPort = 3898;

        private Socket _connectionReceiver;
        //private List<ConnectionListener> _listeners;
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
        /// Creates a FTFileRequester for a file.
        /// </summary>
        /// <param name="files"></param>
        public void DownloadFile(FTTFileInfo file, String dest)
        {
            // Switch this to run on a seperate thread!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // Attempt to open connection with remote host.
            try
            {
                Socket socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(new IPEndPoint(BroadcastManager.LocalIPAddress(), _receivingPort));
                socket.Connect(new IPEndPoint(IPAddress.Parse(file.IP), FILETRANSFER_PORT));

                // Increment port so there arnt conflicts
                _receivingPort++;
                if (_receivingPort > 7000)
                {
                    _receivingPort = 3898;
                }

                // Create new fileRequester. It will run the request automaticaly.
                FTTConsole.AddDebug("Sending request for file: " + file.Name);
                new FTFileRequester(file.Name, socket, dest);
            }
            catch (Exception e)
            {
                FTTConsole.AddError("Error trying to connect to remmote host: " + file.IP);
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
            new FTFileSender(socket,listener_fileReceivedCallback, _getFilePath);
        }

        private void listener_fileReceivedCallback()
        {
            Console.WriteLine("File received herez");
        }


    }
}
