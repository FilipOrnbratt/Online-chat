using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Client
{
    class Connection
    {
        private Socket server;
        private Action<string> MessageHandler;
        public Connection(Action<string> MessageHandler)
        {
            this.MessageHandler = MessageHandler;
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            server = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            server.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 27960));
            Console.WriteLine("Connected to: " + server.RemoteEndPoint.ToString());
            new Thread(RecieveMessages).Start();
        }

        private void RecieveMessages()
        {
            string msg;
            byte[] bytes = new byte[1024];
            while (true)
            {
                try
                {
                    int result = server.Receive(bytes);
                    msg = Encoding.ASCII.GetString(bytes, 0, result);
                    MessageHandler(msg);
                    if (msg.IndexOf("<EOF>") > -1)
                    {
                        continue;
                    }
                }
                catch (SocketException e)
                {
                    break;
                }
            }
        }

        public void SendMessage(string message)
        {
            server.Send(Encoding.ASCII.GetBytes(message));
        }
    }
}
