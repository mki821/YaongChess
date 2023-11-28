using System;
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    public Piece piece;

    private void Awake() {
        if(gameObject.name.Contains("(Clone)")) gameObject.name.Replace("(Clone)", "");

        piece.type = (Type)Enum.Parse(typeof(Type), gameObject.name.Substring(5));
        piece.team = (Team)Enum.Parse(typeof(Team), gameObject.name.Substring(0, 5));
    }
}
