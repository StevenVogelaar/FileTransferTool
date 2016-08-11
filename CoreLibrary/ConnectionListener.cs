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
    class ConnectionListener
    {

        //TcpListener _listener;
        Thread _thread;

        UdpClient _udpClient;

        public ConnectionListener()
        {
            _thread = new Thread(listenForRequests);
            _thread.IsBackground = true;
        }

        public void Start()
        {
            _thread.Start();
        }


        private void listenForRequests()
        {

            try
            {
                _udpClient = new UdpClient(ConnectionManager.MULTICAST_PORT, AddressFamily.InterNetwork);
                _udpClient.JoinMulticastGroup(IPAddress.Parse(ConnectionManager.MULTICAST_IP));
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, ConnectionManager.MULTICAST_PORT);
                String msg = "";
                ASCIIEncoding ascii = new ASCIIEncoding();

                while (true)
                {

                    Console.WriteLine("Waiting for message");
                    Byte[] data = _udpClient.Receive(ref ipEndPoint);
                    msg = ascii.GetString(data);

                    Console.WriteLine("Recived: " + msg);
                    if (msg.Equals("quit")) break;
                }

            }
            catch (Exception e)
            {

                Console.WriteLine(e.StackTrace);

            }
            finally
            {
                if (_udpClient != null)
                {
                    _udpClient.Close();
                }
            }

        }

    }
}
