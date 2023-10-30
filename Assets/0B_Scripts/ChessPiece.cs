using System;
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    public Piece piece;

    private void Awake() {
        piece.type = (Type)Enum.Parse(typeof(Type), gameObject.name.Substring(5));
    }
}
