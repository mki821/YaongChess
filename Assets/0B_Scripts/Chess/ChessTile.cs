using UnityEngine;

public class ChessTile : MonoBehaviour
{
    public Vector2Int pos;
    public MeshRenderer meshRender;

    private void Awake() {
        meshRender = GetComponent<MeshRenderer>();
    }
}
