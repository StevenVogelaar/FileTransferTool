using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace FileTransferTool.CoreLibrary.Net
{
    class Connection : IDisposable
    {
        private TcpClient _client;
        private Thread _thread;
        private NetworkStream _stream;
        private String _ip;
        private Int32 _port;


        public Connection(String ip)
        {
            _thread = new Thread(initiateConnect);
            _thread.IsBackground = true;
            _port = 2943;
            _ip = ip;
        }

        private void initiateConnect()
        {

            try
            {

                _client = new TcpClient(_ip, _port);
                _stream = _client.GetStream();


                Byte[] data = Encoding.ASCII.GetBytes("Hello from client");
                _stream.Write(data, 0, data.Length);


                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = _stream.Read(data, 0, data.Length);
                responseData = Encoding.ASCII.GetString(data, 0, bytes);

                // Close everything.
                _stream.Close();
                _client.Close();

            }
            catch (ArgumentNullException e)
            {
                //Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
               // Console.WriteLine("SocketException: {0}", e);
            }
        }


        public void Connect()
        {
            _thread.Start();
        }

        public void Dispose()
        {
            try
            {
                _thread.Abort();
                
                _stream.Dispose();
            }
            catch (Exception e)
            {

            }
        }
    }
}
