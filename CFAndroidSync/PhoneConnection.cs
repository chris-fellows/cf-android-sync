using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Accessibility;

namespace CFAndroidSync
{
    internal class PhoneConnection
    {
        private Thread _thread;        

        public void Start()
        {
            _thread = new Thread(Run);
            _thread.Start();
        }

        public void Stop()
        {
            _thread.Abort();
        }

        public void Run()
        {
            int port = 11000;

            UdpClient udpServer = new UdpClient(port);

            while (true)
            {
                var remoteEP = new IPEndPoint(IPAddress.Any, port);
                var data = udpServer.Receive(ref remoteEP); // listen on port 11000
                var message = Encoding.UTF8.GetString(data);
                Console.Write("receive data from " + remoteEP.ToString());
                udpServer.Send(new byte[] { 1 }, 1, remoteEP); // reply back
            }
        }
    }
}
