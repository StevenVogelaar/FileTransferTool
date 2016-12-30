using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Net.NetworkInformation;
using FileTransferTool.CoreLibrary.Net;
using FileTransferTool.CoreLibrary.Files;
using FileTransferTool.CoreLibrary.UI;



namespace FileTransferTool.CoreLibrary.Net
{
    class BroadcastSender : IDisposable
    {
        private static ManualResetEvent _resetEvent;
        private Thread _thread;
        private UdpClient _broadcaster;
        private IPEndPoint _ipEndPoint;
        private List<UdpClient> _additionalClients;
        private Queue<Message> _messageQueue;

        public BroadcastSender()
        {
            _resetEvent = new ManualResetEvent(false);
            _messageQueue = new Queue<Message>();
            _broadcaster = new UdpClient();
			_broadcaster.JoinMulticastGroup (IPAddress.Parse (BroadcastManager.MULTICAST_IP));
            _broadcaster.MulticastLoopback = false;
            _ipEndPoint = new IPEndPoint(IPAddress.Parse(BroadcastManager.MULTICAST_IP), BroadcastManager.MULTICAST_PORT);
            _additionalClients = new List<UdpClient>();
            _thread = new Thread(Send);
            _thread.IsBackground = true;
            _thread.Start();
        }

        private static Byte[] getByteArray(Char[] message)
        {
            Byte[] Ret = new Byte[message.Length];
            for (int i = 0; i < message.Length; i++)
                Ret[i] = (Byte)message[i];
            return Ret;
        }


        /// <summary>
        /// Used to send a broadcast. Broadcast is sent on a seperate thread. Messages will be added to a queue and may not be sent immediatly.
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(Message message)
        {   
            lock (_messageQueue)
            {
                _messageQueue.Enqueue(message);
                _resetEvent.Set();
            }
        }


        /// <summary>
        /// Looping method that will broadcast message. Will stay alive on a background thread for duration of program. Triggered using an autoevent.
        /// </summary>
        /// <param name="args">Message object</param>
        private void Send()
        {

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Message));
          

            using (MemoryStream stream = new MemoryStream()) {

                StreamReader reader = new StreamReader(stream);

                while (true)
                {

                    _resetEvent.WaitOne();

                    try
                    {

                        Message message;
                        lock (_messageQueue)
                        {
                            // If there is no message in queue, return to start of loop and wait for autoEvent to trigger.
                            if (_messageQueue.Count > 0)
                            {
                                message = _messageQueue.Dequeue();
                            }
                            else
                            {
                                _resetEvent.Reset();
                                continue;
                            }
                        }

                        // Empty and write object to stream.
                        stream.SetLength(0);
                        serializer.WriteObject(stream, message);

                        // read object from stream.
                        stream.Seek(0, SeekOrigin.Begin);
                        String serMessage = reader.ReadToEnd();

                        byte[] data = getByteArray(serMessage.ToCharArray());

                        // Send data using the broadcaster using UDP multicasting.
                        _broadcaster.SendAsync(data, data.Length, _ipEndPoint);

                        // Send data to specific targets added to the additionalClient list.
                        foreach (UdpClient c in _additionalClients)
                        {
                            c.SendAsync(data, data.Length, _ipEndPoint);
                        }

                        FTTConsole.AddDebug("Sent Message: " + serMessage);
                    }
                    catch (Exception e)
                    {
                        FTTConsole.AddError("\n" + e.Message + "\n" + e.StackTrace + "\n");
                    }
                }
            }
        }

     

        public void Dispose()
        {
            _thread.Abort();

            lock (_broadcaster)
            {
                _broadcaster.Close();
            }

            foreach (UdpClient c in _additionalClients)
            {
                if (c != null) c.Close();
            }
        }
    }
}
