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

        TcpListener _listener;
        Thread _thread;

        public ConnectionListener()
        {
            _thread = new Thread(listenForRequests);
            _thread.IsBackground = true;
            _thread.Start();
        }


        private void listenForRequests()
        {

            try
            {
                Int32 port = 2943;
                IPAddress localAddr = IPAddress.Parse("192.168.0.12");
                _listener = new TcpListener(port);

                _listener.Start();

                Byte[] bytes = new Byte[256];
                String data = null;

                // Wait for clients to connect
                while (true)
                {

                    Console.WriteLine("waiting for connection");
                    TcpClient client = _listener.AcceptTcpClient();
                    Console.WriteLine("Connected");

                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        byte[] msg = Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close();    
            }

            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                _listener.Stop();
            }

        }

    }
}
