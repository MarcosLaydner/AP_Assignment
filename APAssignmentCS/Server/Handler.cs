using System;
using System.IO;
using System.Net.Sockets;

namespace Server
{
    class Handler
    {

        private TcpClient tcpClient;
        private String id;
        private StreamReader reader;
        private StreamWriter writer;

        public Handler(TcpClient tcpClient, String id)
        {

            this.tcpClient = tcpClient;
            this.id = id;
            reader = new StreamReader(tcpClient.GetStream());
            writer = new StreamWriter(tcpClient.GetStream());
            writer.AutoFlush = true;
        }

        public void run()
        {

            Console.WriteLine(id.ToUpper() + " Connected." + tcpClient);
            Server.ListPlayers();

            try
            {
                while (true) {
                    String message = reader.ReadLine();

                    if (Server.GetNames().Contains(message.ToLower()))
                    {
                        Handler newHolder = FindHandlerById(message.ToLower());
                        Server.SetHolder(newHolder);
                        Server.UpdateState();
                        Console.WriteLine(id.ToUpper() +
                                " has passed the ball to " + newHolder.GetId().ToUpper());
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("Error on " + id);

            }
            finally
            {
                try
                {
                    this.close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error on closing Handler for player " + id);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        private Handler FindHandlerById(String id)
        {
            foreach (Handler h in Server.GetPlayers())
            {
                if (h.GetId().Equals(id))
                {
                    return h;
                }
            }

            return null;
        }

        public StreamWriter GetWriter()
        {
            return writer;
        }

        public String GetId()
        {
            return id;
        }

        public void close()
        {

            Server.GetPlayers().Remove(this);
            Server.GetNames().Remove(this.id);

            writer.Close();
            reader.Close();
            tcpClient.Close();

            Console.WriteLine(id.ToUpper() + " Disconnected.");
            Server.ListPlayers();

            if (Server.GetHolder().Equals(this))
            {
                if (Server.GetPlayers().Count > 0)
                {
                    Handler newHolder = (Handler) Server.GetPlayers()[0];
                    Server.SetHolder(newHolder);
                    Console.WriteLine("Ball Holder disconnected, passing the ball to " + newHolder.GetId().ToUpper());
                }
                else
                {
                    Server.SetHolder(null);
                    Console.WriteLine("Ball Holder disconnected, and was the last player. Waiting for a player to enter");
                }
            }
            Server.UpdateState();
        }
    }
}
