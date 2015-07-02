using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketConnector;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net;

namespace Server
{
    class Program
    {
        private static List<ClientData> clients;
        private static Socket listener;
        private static TextWriter writer;
        private static bool log = true;

        static void Main(string[] args)
        {
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clients = new List<ClientData>();

            IPEndPoint ip = new IPEndPoint(IPAddress.Any, 4242);
            listener.Bind(ip);

            Thread listenThread = new Thread(ListenThread);
            listenThread.Start();


            writer = new StreamWriter("log.txt");

            Console.WriteLine("Success... Address: " + ip);
            string tosend = "";
            while (true)
            {
                string cmd = Console.ReadLine();
                string[] splitted = cmd.Split(' ');
                switch (splitted[0].ToLower())
                {
                    case ("send"):
                        tosend = "";
                        for (int i = 1; i < splitted.Length; i++)
                        {
                            tosend += splitted[i] + " ";
                        }
                        Console.WriteLine("Sending '" + tosend + "'");
                        Packet p = new Packet(PacketType.Chat, "server");
                        p.data.Add("SERVER: " + tosend);
                        SendMessageToCLients(p);
                        break;
                    case ("clients"):
                        Console.WriteLine("Clients: " + clients.Count);
                        break;
                    case ("log"):
                        if (log)
                        {
                            log = false;
                            Console.WriteLine("Log Disabled!");
                        }
                        else
                        {
                            log = true;
                            Console.WriteLine("Log Enabled!");
                        }
                        break;
                    case ("exit"):
                    case ("stop"):
                        writer.Close();
                        Environment.Exit(0);
                        break;
                    case ("list"):
                        for (int i = 0; i < clients.Count; i++)
                        {
                            Console.WriteLine(i + ": " + clients[i].username);
                        }
                        break;
                    case ("wisper"):
                        if (splitted.Length >= 3)
                        {
                            int user = -1;
                            for (int i = 0; i < clients.Count; i++)
                            {
                                if (clients[i].username == splitted[1])
                                {
                                    user = i;
                                    break;
                                }
                            }
                            if (user == -1)
                            {
                                Console.WriteLine("Couldn't find user or user is not connected to the server");
                            }
                            else
                            {
                                tosend = "";
                                for (int i = 2; i < splitted.Length; i++)
                                {
                                    tosend += splitted[i] + " ";
                                }
                                Console.WriteLine("Sending '" + tosend + "'");
                                Packet p2 = new Packet(PacketType.Chat, "server");
                                p2.data.Add("SERVER: " + tosend);
                                SendMessageToCLients(p2);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Command not in format 'wisper [username] [string]'");
                        }
                        break;
                }
            }
        }

        static void ListenThread()
        {
            while (true)
            {
                listener.Listen(0);
                clients.Add(new ClientData(listener.Accept()));
            }
        }

        public static void Data_IN(object cSocket)
        {
            Socket clientSocket = ((ClientData)cSocket).clientSocket;

            byte[] Buffer;
            int readBytes;

            while (true)
            {
                try
                {
                    Buffer = new byte[clientSocket.SendBufferSize];
                    readBytes = clientSocket.Receive(Buffer);

                    if (readBytes > 0)
                    {
                        Packet p = new Packet(Buffer);
                        if (log)
                        {
                            writer.WriteLine("Type: " + p.packetType + " -> Data Count: " + p.data.Count);
                            Console.WriteLine("Type: " + p.packetType + " -> Data Count: " + p.data.Count);
                            foreach (string values in p.data)
                            {
                                Console.WriteLine("      " + values);
                            }
                        }
                        DataManager(cSocket, p);
                    }
                }
                catch (SocketException)
                {
                    Console.WriteLine("Client Disconnected.");
                    clients.Remove(((ClientData)cSocket));
                    Thread.CurrentThread.Abort();
                }
            }
        }

        public static void DataManager(object client, Packet p)
        {
            switch (p.packetType)
            {
                case PacketType.Registration:
                    for (int i = 0; i < p.data.Count; i++ )
                    {
                        Console.WriteLine(p.data[i]);
                    }
                    Console.WriteLine("------------------------");
                    ((ClientData)client).username = p.data[0];
                    break;
                case PacketType.getNews:
                    Packet news = new Packet(PacketType.getNews, "SERVER");
                    news.data.Add("There are no News!");
                    ((ClientData)client).clientSocket.Send(news.ToBytes());
                    break;
                case PacketType.Chat:
                    SendMessageToCLients(p);
                    break;


                case PacketType.CloseConnection:
                    var exitClient = GetClientByID(p);

                    CloseClientConnection(exitClient);
                    RemoveClientFromList(exitClient);
                    SendMessageToCLients(p);
                    AbortClientThread(exitClient);
                    break;
            }
        }

        public static void SendMessageToCLients(Packet p)
        {
            foreach (ClientData c in clients)
            {
                c.clientSocket.Send(p.ToBytes());
            }
        }
        private static ClientData GetClientByID(Packet p)
        {
            return (from client in clients
                    where client.id == p.senderID
                    select client)
                    .FirstOrDefault();

        }
        private static void CloseClientConnection(ClientData c)
        {
            c.clientSocket.Close();
        }
        private static void RemoveClientFromList(ClientData c)
        {
            clients.Remove(c);
        }
        private static void AbortClientThread(ClientData c)
        {
            c.clientThread.Abort();
        }
    }
}