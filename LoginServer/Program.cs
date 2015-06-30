using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using MySql.Data.MySqlClient;

namespace LoginServer
{
    class LoginServer
    {
        static LoginServer singleton;
        private Socket serverSocket = null;
        //mysql connection string
        ArrayList _connections = new ArrayList();
        ArrayList _buffer = new ArrayList();
        ArrayList _byteBuffer = new ArrayList();

        static void Main(string[] args)
        {
            LoginServer serverInstance = new LoginServer();
            serverInstance.SetupServer();

            while (true)
            {
                serverInstance.HoldListening();
                Console.Clear();
                Console.WriteLine("Connections: " + serverInstance._connections.Count);
            }

        }
        private void SetupServer()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipLocal = new IPEndPoint(IPAddress.Any, 32211);
            serverSocket.Bind(ipLocal);
            serverSocket.Listen(100);
            singleton = this;
            Console.WriteLine("Server started on: " + ipLocal.ToString());
        }
        private void HoldListening()
        {
            ArrayList list = new ArrayList();
            list.Add(serverSocket);
            Socket.Select(list, null, null, 1000);
            for (int i = 0; i < list.Count; i++)
            {
                Socket newSocket = ((Socket)list[i]).Accept();
                _connections.Add(newSocket);
                _byteBuffer.Add(new ArrayList());
                Console.WriteLine("New Connection from: " + newSocket.LocalEndPoint.ToString());
            }
            ReadData();
        }
        private void ReadData()
        {
            if (_connections.Count > 0)
            {
                ArrayList connections = new ArrayList(_connections);
                Socket.Select(connections, null, null, 1000);
                foreach (Socket socket in connections)
                {
                    byte[] receivedBytes = new byte[512];
                    ArrayList buffer = (ArrayList)_byteBuffer[_connections.IndexOf(socket)];
                    int read = socket.Receive(receivedBytes);
                    for (int i = 0; i < read; i++)
                    {
                        buffer.Add(receivedBytes[i]);
                    }

                    while (true && buffer.Count > 0)
                    {
                        int length = (byte)buffer[0];
                        Console.WriteLine(length + " | " + buffer.Count + " | " + buffer[0].ToString());
                        if (length < buffer.Count)
                        {
                            ArrayList thisMsgBytes = new ArrayList(buffer);
                            thisMsgBytes.RemoveRange(length + 1, thisMsgBytes.Count - (length + 1));
                            thisMsgBytes.RemoveRange(0, 1);
                            if (thisMsgBytes.Count != length)
                            {
                                Console.WriteLine("ERROR 1");
                            }
                            buffer.RemoveRange(0, length + 1);
                            byte[] readBytes = (byte[])thisMsgBytes.ToArray(typeof(byte));
                            MessageData readMsg = MessageData.FromByteArray(readBytes);
                            _buffer.Add(readMsg);
                            Console.WriteLine("Message of type {0}: {1}", readMsg.type, readMsg.stringData);
                            HandleReceivedPacket(readMsg);
                            buffer.Clear();
                            if (singleton != this)
                            {
                                Console.WriteLine("ERROR 2");
                            }
                        }
                    }
                }
            }
        }

        private void HandleReceivedPacket(MessageData data)
        {
            switch (data.type)
            {
                case (100):
                    Console.WriteLine("Wuhuu, ne 100");
                    break;
                default:
                    Console.WriteLine("Received Package: Type=[" + data.type + "] and data=[" + data.stringData + "]");
                    break;
            }
        }
    }
}
