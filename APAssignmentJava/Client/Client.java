package Client;

import java.io.PrintWriter;
import java.net.Socket;
import java.util.HashSet;
import java.util.Scanner;
import java.util.Set;

public class Client {

    private static String id;
    private static Set<String> players;
    private static String holder;
    private static boolean choosing = false;

    public static void main(String[] args) {
        try (Socket socket = new Socket("localhost", 59898)) {

            Scanner in = new Scanner(socket.getInputStream());
            PrintWriter out = new PrintWriter(socket.getOutputStream(), true);

            while (true) {

                updateState(in);

                System.out.println("-------------------------------------------");
                System.out.println("Your id: "+id);
                System.out.println("Players Connected:");

                for (String s: players) {
                    System.out.println("    "+s);
                }

                if (holder.equals(id)) {

                    System.out.println("You have the ball! Type the id of the player you wish to pass the ball to:");

                    if (!choosing) {
                        choosing = true;
                        new Thread(new ClientWriter(out)).start();
                    }

                } else {
                    System.out.println("Currently, "+holder+" has the ball.");
                    System.out.println("-------------------------------------------");
                }
            }

        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private static void updateState(Scanner in) {
        id = in.nextLine();
        holder = in.nextLine();
        int size = Integer.parseInt(in.nextLine());
        players = new HashSet<>();
        for(int i=0; i < size; i++) {
            players.add(in.nextLine());
        }
    }

    public static void setChoosing(boolean choosing) {
        Client.choosing = choosing;
    }

    public static Set<String> getPlayers() {
        return players;
    }
}