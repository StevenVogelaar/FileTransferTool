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


namespace CoreLibrary
{
    class Sender : IDisposable
    {

        private Thread _thread;
        private UdpClient _broadcaster;
        private IPEndPoint _ipEndPoint;
        private DataContractJsonSerializer _serializer;
        private List<UdpClient> _additionalClients;

        public Sender()
        {
            _broadcaster = new UdpClient();
            _broadcaster.JoinMulticastGroup(IPAddress.Parse(ConnectionManager.MULTICAST_IP));
            _ipEndPoint = new IPEndPoint(IPAddress.Parse(ConnectionManager.MULTICAST_IP), ConnectionManager.MULTICAST_PORT);
            _additionalClients = new List<UdpClient>();
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

            try
            {
                byte[] data = getByteArray("Hallo world".ToCharArray());

                // Send data using the broadcaster using UDP multicasting.
                _broadcaster.SendAsync(data, data.Length, _ipEndPoint);

                // Send data to specific targets added to the additionalClient list.
                foreach (UdpClient c in _additionalClients)
                {
                    c.SendAsync(data, data.Length, _ipEndPoint);
                }

                Console.WriteLine("Sent message: " + "meh");
                MessageBox.Show("Sent message: " + "meh");
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message + "\n" + e.StackTrace + "\n");
            }
        }

     

        public void Dispose()
        {
            lock (_broadcaster)
            {
                _broadcaster.Close();
            }
        }
    }
}
