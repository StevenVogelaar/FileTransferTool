using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace CoreLibrary
{
    class Pinger
    {

        private IPAddress[] _ipAddresses;
        private List<IPAddress> _responseAddresses;
        private Ping _pinger;
        private int _completed;
        private AutoResetEvent _resetEvent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddresses">Array of IP addresses that are to be pinged.</param>
        public Pinger(IPAddress[] ipAddresses)
        {
            _ipAddresses = ipAddresses;
            _responseAddresses = new List<IPAddress>();
            _pinger = new Ping();
            _pinger.PingCompleted += pinger_PingCompleted;
            _completed = 0;
            _resetEvent = new AutoResetEvent(false);
        }


        public IPAddress[] PingAll()
        {
            if (_ipAddresses.Length == 0) return new IPAddress[0];

            foreach (IPAddress ip in _ipAddresses)
            {
                _pinger.SendAsync(ip, null);
            }

            Console.WriteLine("Waiting for ping responses...");
            _resetEvent.WaitOne();
            Console.WriteLine("Responses all received");
            return _responseAddresses.ToArray();
        }

        public void pinger_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            Console.WriteLine("recevied ping response");

            if (e.Reply.Status == IPStatus.Success)
            {
                _responseAddresses.Add(e.Reply.Address);
            }
            _completed++;

            if (_completed == _ipAddresses.Length) _resetEvent.Set();
        }

    }
}
