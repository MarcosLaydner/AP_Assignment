package Server;

import java.io.PrintWriter;
import java.net.Socket;
import java.util.Scanner;

public class Handler implements Runnable, AutoCloseable {

    private Socket socket;
    private String id;
    private Scanner in;
    private PrintWriter out;

    Handler(Socket socket, String id) throws Exception {

        this.socket = socket;
        this.id = id;
        in = new Scanner(socket.getInputStream());
        out = new PrintWriter(socket.getOutputStream(), true);
    }

    @Override
    public void run() {

        System.out.println(id.toUpperCase()+ " Connected." + socket);
        Server.listPlayers();

        try {
            while (in.hasNextLine()) {
                String message = in.nextLine();

                    if (Server.getNames().contains(message.toLowerCase())) {
                        Handler newHolder = findHandlerById(message.toLowerCase());
                        Server.setHolder(newHolder);
                        Server.updateState();
                        System.out.println(id.toUpperCase() +
                                " has passed the ball to " + newHolder.getId().toUpperCase());
                    }

                }
        } catch (Exception e) {
            e.printStackTrace();
            System.out.println("Error on " + id);

        } finally {
            try {
                this.close();
            } catch (Exception e) {
                System.out.println("Error on closing Server.Server.Handler for player " + id);
                e.printStackTrace();
            }
        }
    }

    private Handler findHandlerById(String id) {
        for(Handler h: Server.getPlayers()) {
            if (h.getId().equals(id)) {
                return h;
            }
        }

        return null;
    }

    public PrintWriter getOut() {
        return out;
    }

    public String getId() {
        return id;
    }

    @Override
    public void close() throws Exception {

        Server.getPlayers().remove(this);
        Server.getNames().remove(this.id);

        out.close();
        in.close();
        socket.close();

        System.out.println(id.toUpperCase() + " Disconnected. " + socket);
        Server.listPlayers();

        if (Server.getHolder().equals(this)) {
            if (!Server.getPlayers().isEmpty()) {
                Handler newHolder = Server.getPlayers().get(0);
                Server.setHolder(newHolder);
                System.out.println("Ball Holder disconnected, passing the ball to " + newHolder.getId().toUpperCase());
            } else {
                Server.setHolder(null);
                System.out.println("Ball Holder disconnected, and was the last player. Waiting for a player to enter");
            }
        }
        Server.updateState();
    }
}
