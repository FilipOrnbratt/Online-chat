using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Connection
    {
        private Socket client;
        private Action<string, Connection> MessageHandler;

        public Connection(Socket socket, Action<string, Connection> MessageHandler)
        {
            this.MessageHandler = MessageHandler;
            this.client = socket;
            Console.WriteLine(client.RemoteEndPoint + " connected");
            new Thread(RecieveMessages).Start();
        }

        private void close()
        {
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        private void RecieveMessages()
        {
            string msg;
            byte[] bytes = new byte[1024];
            while (true)
            {
                try
                {
                    int result = client.Receive(bytes);
                    msg = Encoding.ASCII.GetString(bytes, 0, result);
                    if (msg.IndexOf("<EOF>") > -1)
                    {
                        continue;
                    }
                    MessageHandler(msg, this);
                }
                catch (SocketException e)
                {
                    MessageHandler("disconnected", this);
                    break;
                }
            }
        }

        public void SendMessage(string message)
        {
            client.Send(Encoding.ASCII.GetBytes(message));
        }

        public string getIdentifier()
        {
            return client.RemoteEndPoint.ToString();
        }
    }
}
