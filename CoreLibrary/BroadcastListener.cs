using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.Serialization.Json;
using System.IO;

namespace CoreLibrary
{
    class BroadcastListener : IDisposable
    {

        public delegate void MessageReceivedHandler(object sender, MessageReceivedEventArgs e);
        public event MessageReceivedHandler MessageReceived;

        private List<UdpClient> _udpClients;
        private IPEndPoint _ipEndPoint;
        private List<Thread> _threads;

        public BroadcastListener()
        {
            _udpClients = new List<UdpClient>();
            _threads = new List<Thread>();
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
                _threads.Add(thread);
            }
        }


        /// <summary>
        /// Listens for broadcasts. Call only on new threads.
        /// </summary>
        /// <param name="udpClientObj"></param>
        private void listenForRequests(Object udpClientObj)
        {

            UdpClient udpClient = (UdpClient)udpClientObj;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Message));
            MemoryStream stream = new MemoryStream();
            
            bool loop = true;

            while (loop)
            {
                try
                {
                    
                    udpClient.JoinMulticastGroup(IPAddress.Parse(ConnectionManager.MULTICAST_IP));
                    udpClient.MulticastLoopback = false;

                    String msg = "";
                    ASCIIEncoding ascii = new ASCIIEncoding();

                    // Listen for broadcast messages
                    while (true)
                    {
                        FTTConsole.AddDebug(udpClient.Client.LocalEndPoint + ": Waiting for messages...");
                        Byte[] data = udpClient.Receive(ref _ipEndPoint);
                        msg = ascii.GetString(data);
                        FTTConsole.AddDebug(udpClient.Client.LocalEndPoint + ": Received Message: " + msg);
                        //Console.WriteLine(msg);

                        try
                        {
                            // Deserialize.
                            stream.SetLength(0);
                            stream.Write(data, 0, data.Length);
                            stream.Position = 0;
                            Message message = (Message)serializer.ReadObject(stream);

                            // Set ip for each FTTFileInfo in the message
                            foreach (FTTFileInfo f in message.SharedFiles)
                            {
                                f.IP = message.IPAddress;
                            }

                            if (MessageReceived != null)
                            {
                                MessageReceived.Invoke(this, new MessageReceivedEventArgs() { Msg = message });
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.StackTrace);
                            FTTConsole.AddDebug("Error trying to prarse json message: " + e.Message);
                        }
                    }
                }
                catch (Exception e)
                {
                    FTTConsole.AddError("Error occured when trying to listen for broadcasts: " + e.Message);
                    Console.WriteLine(e.Message + "\n" + e.StackTrace);
                     
                }
                finally{

                    // If an exception is thrown, close the client and stop listening.
                    udpClient.Close();
                    loop = false;
                    FTTConsole.AddDebug("Stopped listening on: " + udpClient.Client.LocalEndPoint);
                }
            }
        }

        public void Dispose()
        {

            foreach(Thread t in _threads)
            {
                if (t != null) t.Abort();
            }

            foreach(UdpClient c in _udpClients)
            {
                if (c != null) c.Close();
            }
        }

        public class MessageReceivedEventArgs : EventArgs
        {
            public Message Msg { get; set; }
        }

    }
}
