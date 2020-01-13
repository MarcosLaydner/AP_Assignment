using System;
using System.Collections.Generic;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace Server
{
    class Server
    {

        private static HashSet<String> names = new HashSet<String>();
        private static Handler holder;
        private static ArrayList players = new ArrayList();
        private static long counter;

        static void Main(String[] args)
        {

            TcpListener listener = new TcpListener(IPAddress.Loopback, 59898);
            listener.Start();
            Console.WriteLine("Waiting for incoming connections...");
            while (true)
            {
                TcpClient tcpClient = listener.AcceptTcpClient();
                Handler temp = new Handler(tcpClient, "player"+counter);
                new Thread(temp.run).Start();
                counter++;
                Server.names.Add(temp.GetId());
                Server.players.Add(temp);
                if (holder == null)
                {
                    holder = temp;
                }
                UpdateState();

            }
        }

        public static void ListPlayers()
        {
            Console.WriteLine("-------------------------------------------");
            Console.WriteLine("Players currently connected:");
            foreach (String id in names)
                Console.WriteLine(id);
        
            Console.WriteLine("-------------------------------------------");
        }

        public static void UpdateState()
        {
            StreamWriter writer;
            foreach (Handler h in players)
            {
                writer = h.GetWriter();
                writer.WriteLine(h.GetId());
                writer.WriteLine(holder.GetId());
                writer.WriteLine(names.Count);
                SendPlayerList(writer);
            }
        }

        private static void SendPlayerList(StreamWriter writer)
        {
            foreach (String id in names)
            {
                writer.WriteLine(id);
            }
        }

        public static HashSet<String> GetNames()
        {
            return names;
        }

        public static Handler GetHolder()
        {
            return holder;
        }

        public static void SetHolder(Handler holder)
        {
            Server.holder = holder;
        }

        public static ArrayList GetPlayers()
        {
            return players;
        }
    }
}
