using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Server
{
    class Server
    {
        private List<Connection> connections = new List<Connection>();
        private readonly object listLock = new object();

        public static void Main()
        {
            new Server();   
        }

        public Server()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            Socket socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(new IPEndPoint(ip, 27960));
            socket.Listen(10);

            Console.WriteLine("Waiting for connections...");
            while (true)
            {
                Socket client = socket.Accept();
                lock (listLock)
                {
                    connections.Add(new Connection(client, HandleMessage));
                    Console.WriteLine(connections.Count);
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void HandleMessage(string message, Connection source)
        {
            if (message.Equals("disconnected"))
            {
                lock (listLock)
                {
                    connections.Remove(source);
                }
                Broadcast(source.getIdentifier() + " " + message);
                Console.WriteLine(source.getIdentifier() + " " + message);
                Console.WriteLine(connections.Count);
            }
            else
            {
                Broadcast(source.getIdentifier() + ": " + message);
                Console.WriteLine(source.getIdentifier() + ": " + message);
            }
        }

        private void Broadcast(string message)
        {
            lock (listLock)
            {
                foreach (Connection c in connections)
                {
                    c.SendMessage(message);
                }
            }
        }
    }
}
