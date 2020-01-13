using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    public class Client
    {
        private static String id;
        private static HashSet<String> players;
        private static String holder;
        private static Boolean choosing;

        public static void Main(String[] args)
        {
            try
            {
                TcpClient tcpClient = new TcpClient("localhost", 59898); 

                StreamReader reader = new StreamReader(tcpClient.GetStream());
                StreamWriter writer = new StreamWriter(tcpClient.GetStream());
                writer.AutoFlush = true;

                while (true)
                {

                    UpdateState(reader);

                    Console.WriteLine("-------------------------------------------");
                    Console.WriteLine("Your id: " + id);
                    Console.WriteLine("Players Connected:");

                    foreach (String s in players)
                    {
                        Console.WriteLine("    " + s);
                    }

                    if (holder.Equals(id))
                    {

                        Console.WriteLine("You have the ball! Type the id of the player you wish to pass the ball to:");

                        if (!choosing)
                        {
                            choosing = true;
                            new Thread(WriterThread).Start(writer);
                        }

                    }
                    else
                    {
                        Console.WriteLine("Currently, " + holder + " has the ball.");
                        Console.WriteLine("-------------------------------------------");
                    }
                }

            } catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private static void WriterThread(object param)
        {
            StreamWriter writer = (StreamWriter) param;
            String destination;

            while (true)
            {

                destination = Console.ReadLine();

                if (players.Contains(destination))
                {
                    Console.WriteLine("Passing ball to " + destination);
                    writer.WriteLine(destination);
                    Client.SetChoosing(false);
                    return;
                }
                else
                {
                    Console.WriteLine("Type an id that exists in the list of players");
                }
            }
        }

        private static void UpdateState(StreamReader reader)
        {
            id = reader.ReadLine();
            holder = reader.ReadLine();
            int size = int.Parse(reader.ReadLine());
            players = new HashSet<String>();
            for (int i = 0; i < size; i++)
            {
                players.Add(reader.ReadLine());
            }
        }

        public static void SetChoosing(bool choosing)
        {
            Client.choosing = choosing;
        }

        public static HashSet<String> GetPlayers()
        {
            return players;
        }
    }
}
