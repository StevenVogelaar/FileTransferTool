using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace CoreLibrary
{
    class ConnectionListener
    {

        public delegate void MessageReceivedHandler(object sender, MessageReceivedEventArgs e);
        public event MessageReceivedHandler MessageReceived;

        List<UdpClient> _udpClients;
        IPEndPoint _ipEndPoint;

        public ConnectionListener()
        {
            _udpClients = new List<UdpClient>();
            _ipEndPoint = new IPEndPoint(IPAddress.Parse(ConnectionManager.MULTICAST_IP), ConnectionManager.MULTICAST_PORT);
        }

        public void Start()
        {

            IPAddress[] iplist = Dns.GetHostAddresses(Dns.GetHostName());
            //IPAddress[] iplist = new IPAddress[] { IPAddress.Parse("192.168.0.2") };


            // Initialize UDP Clients on each IPV4 address this PC has.
            foreach (IPAddress s in iplist)
            {
                if (s.AddressFamily == AddressFamily.InterNetwork)
                {
                    try
                    {
                        UdpClient udpClient = new UdpClient(new IPEndPoint(s, ConnectionManager.MULTICAST_PORT));
                        _udpClients.Add(udpClient);
                    }
                    catch (Exception e)
                    {
                        FTTConsole.AddError(e.Message);
                    }
                }
            }

            // Start listening for each UDP Client on seperate threads.
            foreach (UdpClient c in _udpClients)
            {
                Thread thread = new Thread(listenForRequests);
                thread.IsBackground = true;
                thread.Start(c);
            }
        }


        private void listenForRequests(Object udpClientObj)
        {

            try
            {
                UdpClient udpClient = (UdpClient)udpClientObj;
                udpClient.JoinMulticastGroup(IPAddress.Parse(ConnectionManager.MULTICAST_IP));
                
                String msg = "";
                ASCIIEncoding ascii = new ASCIIEncoding();

                
                while (true)
                {

                    FTTConsole.AddDebug(udpClient.Client.LocalEndPoint + ": Waiting for messages...");
                    Byte[] data = udpClient.Receive(ref _ipEndPoint);
                    msg = ascii.GetString(data);
                    FTTConsole.AddDebug(udpClient.Client.LocalEndPoint + ": Received Message: " + msg);

                    if (msg.Equals("quit")) break;
                }

                FTTConsole.AddDebug("Stopped listening on: " + udpClient.Client.LocalEndPoint);

            }
            catch (Exception e)
            {
                FTTConsole.AddError("Error Occured when trying to listen to stuff: " + e.Message);
                Console.WriteLine(e.Message + "\n" + e.StackTrace);

            }
            finally
            {
                foreach (UdpClient c in _udpClients)
                {
                    if (c != null)
                    {
                        c.Close();
                    }
                }
            }
        }


        public class MessageReceivedEventArgs : EventArgs
        {
            public Message Msg { get; set; }
        }

    }
}
