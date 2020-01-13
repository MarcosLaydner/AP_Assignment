package Server;

import java.io.PrintWriter;
import java.net.ServerSocket;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.Set;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class Server {

    private static Set<String> names = new HashSet<>();
    private static Handler holder;
    private static ArrayList<Handler> players = new ArrayList<>();
    private static long counter = 0;

    public static void main(String[] args) throws Exception {
        try (ServerSocket listener = new ServerSocket(59898)) {

            System.out.println("The ball game server is running...");
            ExecutorService pool = Executors.newCachedThreadPool();

            while (true) {
                try {
                    Handler temp = new Handler(listener.accept(), "player"+counter);
                    pool.execute(temp);
                    counter++;
                    Server.names.add(temp.getId());
                    Server.players.add(temp);
                    if (holder == null)
                        holder = temp;
                    updateState();
                } catch (Exception e) {
                    e.printStackTrace();
                }
            }
        }
    }

    public static void listPlayers() {
        System.out.println("-------------------------------------------");
        System.out.println("Players currently connected:");
        for (String id: names) {
            System.out.println(id);
        }
        System.out.println("-------------------------------------------");
    }

    public static void updateState() {
        PrintWriter out;
        for (Handler h: players) {
            out = h.getOut();
            out.println(h.getId());
            out.println(holder.getId());
            out.println(names.size());
            sendPlayerList(out);
        }
    }

    private static void sendPlayerList(PrintWriter out) {
        for (String id: names) {
            out.println(id);
        }
    }

    public static Set<String> getNames() {
        return names;
    }

    public static Handler getHolder() {
        return holder;
    }

    public static void setHolder(Handler holder) {
        Server.holder = holder;
    }

    public static ArrayList<Handler> getPlayers() {
        return players;
    }


}