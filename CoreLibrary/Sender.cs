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
    class Sender : IDisposable
    {

        Thread _thread;
        UdpClient _udpClient;
        IPEndPoint _ipEndPoint;

        public Sender()
        {
            _udpClient = new UdpClient();
            _udpClient.JoinMulticastGroup(IPAddress.Parse(ConnectionManager.MULTICAST_IP));
            _ipEndPoint = new IPEndPoint(IPAddress.Parse(ConnectionManager.MULTICAST_IP), ConnectionManager.MULTICAST_PORT);
        }

        private static Byte[] getByteArray(Char[] message)
        {
            Byte[] Ret = new Byte[message.Length];
            for (int i = 0; i < message.Length; i++)
                Ret[i] = (Byte)message[i];
            return Ret;
        }


        public void SendMessage(Message message)
        {
            _thread = new Thread(attemptSend);
            _thread.IsBackground = true;
            _thread.Start(message);
        }

        private void attemptSend(object messageObj)
        {
            Message message = (Message)messageObj;
            Console.WriteLine("dawfdawfaw");

            try
            {
                byte[] data = getByteArray(message.DataAsJSON().ToCharArray());
                _udpClient.Send(data,data.Length, _ipEndPoint);

                Console.WriteLine("Sent message: " + message.DataAsJSON());
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message + "\n" + e.StackTrace + "\n");
            }
        
        }

        public void Dispose()
        {
            lock (_udpClient)
            {
                _udpClient.Close();
            }
        }
    }
}
