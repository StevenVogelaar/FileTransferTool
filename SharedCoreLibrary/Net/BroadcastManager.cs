using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Net.NetworkInformation;
using FileTransferTool.CoreLibrary.Net;
using FileTransferTool.CoreLibrary.Files;
using FileTransferTool.CoreLibrary.UI;


namespace FileTransferTool.CoreLibrary.Net
{
    class BroadcastManager : IDisposable
    {

        public const String MULTICAST_IP = "227.42.123.96";
        public const Int32 MULTICAST_PORT = 3896;

        public delegate void AvailableFilesReceivedHandler(object sender, AvailableFilesReceivedEventArgs e);
        public event AvailableFilesReceivedHandler AvailableFilesReceived;

        public static HashSet<IPAddress> ConnectedClients { get; private set; }

        private BroadcastListener _broadcastListener;
        private BroadcastSender _broadcastSender;
        private HashSet<IPAddress> _connectedClientSet;


        public BroadcastManager()
        {
            ConnectedClients = new HashSet<IPAddress>();
            _broadcastListener = new BroadcastListener();
            _broadcastSender = new BroadcastSender();
            _connectedClientSet = new HashSet<IPAddress>();
            _broadcastListener.Start();

            _broadcastListener.MessageReceived += broadcastListener_MessageReceived;
        }


        /// <summary>
        /// Sends broadcast request and pings previously connected clients. If there is no response from pings, will remove their files.
        /// </summary>
        public void RefreshConnections()
        {

            _broadcastSender.SendMessage(new Message()
            {
                RequestBroadcast = true,
                IPAddress = LocalIPAddress().ToString(),
                Msg = "Broadcast Request",
                SharedFiles = FTTFileInfo.ConvertFileHandler(Core.SharedFiles.CopyOfList(), LocalIPAddress().ToString())
            });
        }


        private void broadcastListener_MessageReceived(object sender, BroadcastListener.MessageReceivedEventArgs e)
        {
            // Allways add ip to set of connected IP addresses to be used for pinging.
            _connectedClientSet.Add(IPAddress.Parse(e.Msg.IPAddress));

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
