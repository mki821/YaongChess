using System;
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    public Piece piece;

    private void Awake() {
        piece.type = (Type)Enum.Parse(typeof(Type), gameObject.name.Replace("(Clone)", "").Substring(5));
        piece.team = (Team)Enum.Parse(typeof(Team), gameObject.name.Replace("(Clone)", "").Substring(0, 5));
    }
}
