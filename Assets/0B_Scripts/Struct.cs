using System.Net.Sockets;
using System.Threading;

public enum Type {
    None,
    Pawn,
    Rook,
    Bishop,
    Knight,
    Queen,
    King
}

public enum Team { Black, White }

[System.Serializable]
public class Piece {
    public Team team;
    public Type type;
    public bool firstMove = true;
}

public class NetworkClient {
    public string name;
    public TcpClient client;
    public NetworkStream stream;
    public Thread thread;
}