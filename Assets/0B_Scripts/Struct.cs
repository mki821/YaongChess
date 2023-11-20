using System.Net.Sockets;
using System.Threading;
using LitJson;

public enum Type {
    None,
    Pawn,
    Rook,
    Bishop,
    Knight,
    Queen,
    King
}

public enum Team { None, Black, White }

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

class Box {
    public string command;
    public object data;

    public Box(string command, object data) {
        this.command = command;
        this.data = data;
    }

    public string ToJson() => JsonMapper.ToJson(this);
}

class Basic {
    public object obj1;
    public object obj2;
}

class ChessInfo {
    public int[] selectTile;
    public bool isMove;
    public bool isAttack;
    public bool promote = false;
    public int[] moveTile;
    public Team team;
    public Type type;
}