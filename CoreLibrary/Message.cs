using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary
{
    class Message
    {

        public String Data { get; set; }

        public Message(String message)
        {
            Data = message;
        }

        public Message()
        {

        }


        /// <summary>
        /// Sends currently stored data. Intended to be run on another thread.
        /// </summary>
        public void Send()
        {

        }


        private void parseMessage(String message)
        {

        }

        public String DataAsJSON() {

            if (Data != null) return Data;
            else
            {
                return "Null";
            }
        }

    }
}
