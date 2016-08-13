using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Net.NetworkInformation;

namespace CoreLibrary
{
    class ConnectionManager : IDisposable
    {

        public const String MULTICAST_IP = "227.42.123.96";
        public const Int32 MULTICAST_PORT = 3896;


        private ConnectionListener _connectionListener;
        private Sender _sender;
        private List<Connection> _connectionList;


        public ConnectionManager()
        {
            _connectionListener = new ConnectionListener();
            _sender = new Sender();
            _connectionList = new List<Connection>();
            _connectionListener.Start();
        }



        /// <summary>
        /// Checks for valid localhosts on the network and attempts to connect to them. Will only attempt to connect to hosts that are not currently connected.
        /// </summary>
        public void RefreshConnections()
        {

            _sender.SendMessage(new Message() {Success = false, IPAddress = LocalIPAddress().ToString(), Msg = "Hello this is a test message" });
            
        }

        private void start()
        {
            
        }


        public void Connect()
        {
            Connection connection = new Connection("192.168.0.12");
            _connectionList.Add(connection);
            connection.Connect();
        }


        /// <summary>
        /// Returns the base local ip address i.e. (192.168.0)
        /// </summary>
        /// <returns></returns>
        private String getIPBase()
        {

            String localIP = LocalIPAddress().ToString();
            String temp = localIP.Substring(0, localIP.LastIndexOf('.')) + ".";

            return temp;
        }


        /// <summary>
        /// Returns the local IPV4 address of the computer.
        /// </summary>
        /// <returns></returns>
        public static IPAddress LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
