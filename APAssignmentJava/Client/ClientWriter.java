package Client;

import Client.Client;

import java.io.PrintWriter;
import java.util.Scanner;

public class ClientWriter implements Runnable{

    private PrintWriter out;

    public ClientWriter (PrintWriter out) {
        this.out = out;
    }

    @Override
    public void run() {

        Scanner scanner = new Scanner(System.in);
        String destination;

        while (true) {

            destination = scanner.nextLine();

            if (Client.getPlayers().contains(destination)) {
                System.out.println("Passing ball to "+destination);
                out.println(destination);
                Client.setChoosing(false);
                return;
            } else {
                System.out.println("Type an id that exists in the list of players");
            }
        }


    }
}
