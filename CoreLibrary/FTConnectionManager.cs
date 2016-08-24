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

        private Socket _connectionReceiver;
        //private List<ConnectionListener> _listeners;
        private IPEndPoint _endPoint;
        private Thread _listenThread;


        public FTConnectionManager()
        {
            _endPoint = new IPEndPoint(ConnectionManager.LocalIPAddress(), ConnectionManager.FILETRANSFER_PORT);
            //_listeners = new List<ConnectionListener>();

            _listenThread = new Thread(listenForRequests);
            _listenThread.IsBackground = true;
            _listenThread.Start();
        }



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
            // Create connection listen which then runs on a background thread.
            new FTFileSender(socket,listener_dataReceivedCallback);
        }

        private void listener_dataReceivedCallback()
        {

        }


    }
}
