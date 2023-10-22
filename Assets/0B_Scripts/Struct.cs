public enum Type {
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
}