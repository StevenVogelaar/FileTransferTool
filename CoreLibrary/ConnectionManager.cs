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
    class ConnectionManager
    {

        private ConnectionListener _connectionListener;
        private List<Connection> _connectionList;
        private String ipBase;

        public ConnectionManager()
        {
            _connectionListener = new ConnectionListener();
            _connectionList = new List<Connection>();
        }



        /// <summary>
        /// Checks for valid localhosts on the network and attempts to connect to them. Will only attempt to connect to hosts that are not currently connected.
        /// </summary>
        public void RefreshConnections()
        {

            ipBase = getIPBase();

            // Discover new hosts
            for (int i = 1; i < 255; i++)
            {
                String ip = ipBase + i.ToString();

                Ping p = new Ping();
                p.PingCompleted += new PingCompletedEventHandler(ping_PingCompleted);
                p.SendAsync(ip, 100, ip);
            }
        }


        private void ping_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            Console.WriteLine("IP: " + e.Reply.Address + " Status: " + e.Reply.Status);
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


        private IPAddress LocalIPAddress()
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


    }
}
