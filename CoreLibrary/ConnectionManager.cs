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

        public delegate void AvailableFilesReceivedHandler(object sender, AvailableFilesReceivedEventArgs e);
        public event AvailableFilesReceivedHandler AvailableFilesReceived;


        private BroadcastListener _broadcastListener;
        private BroadcastSender _broadcastSender;
        private List<Connection> _connectionList;
        //private Queue<Message> _messageQueue;

        SynchronizationContext context;


        public ConnectionManager()
        {
            _broadcastListener = new BroadcastListener();
            _broadcastSender = new BroadcastSender();
            _connectionList = new List<Connection>();
            _broadcastListener.Start();

            _broadcastListener.MessageReceived += broadcastListener_MessageReceived;
            context = SynchronizationContext.Current;
        }


        /// <summary>
        /// Checks for valid localhosts on the network and attempts to connect to them. Will only attempt to connect to hosts that are not currently connected.
        /// </summary>
        public void RefreshConnections()
        {
            _broadcastSender.SendMessage(new Message() {RequestBroadcast = true, IPAddress = LocalIPAddress().ToString(), Msg = "Broadcast Request",
                SharedFiles =  FTTFileInfo.ConvertFileHandler(Core.SharedFiles.CopyOfList(), LocalIPAddress().ToString())});
        }


        private void broadcastListener_MessageReceived(object sender, BroadcastListener.MessageReceivedEventArgs e)
        {
            Message message = e.Msg;

            // Check of response broadcast was requested.
            if (message.RequestBroadcast)
            {
                broadcastInfo();
            }

            if (AvailableFilesReceived != null)
            {
                AvailableFilesReceived.Invoke(this, new AvailableFilesReceivedEventArgs() { SourceIP = message.IPAddress, Files = message.SharedFiles });
            }
        }

        /// <summary>
        /// Sends a broadcast to all other clients which includes this clients shared files.
        /// </summary>
        private void broadcastInfo()
        {
            _broadcastSender.SendMessage(new Message() {IPAddress = LocalIPAddress().ToString(), RequestBroadcast = false, Msg = "BroadcastInfo",
                SharedFiles = FTTFileInfo.ConvertFileHandler(Core.SharedFiles.CopyOfList(), LocalIPAddress().ToString())});
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
            if (!NetworkInterface.GetIsNetworkAvailable())
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
            _broadcastSender.Dispose();
            _broadcastListener.Dispose();
        }


        public class AvailableFilesReceivedEventArgs : EventArgs
        {
            public FTTFileInfo[] Files { get; set; }
            public String SourceIP { get; set; }
        }
    }
}
