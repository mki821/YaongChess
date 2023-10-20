using UnityEngine;
using UnityEngine.InputSystem;

public class ChessBoard : MonoBehaviour
{
    private GameObject[,] tiles = new GameObject[8,8];

    private void Awake() {
        int count = 0;
        for(int i = 0; i < 8; i++) {
            for(int j = 0; j < 8; j++) {
                tiles[i, j] = transform.GetChild(count).gameObject;
                count++;
            }
        }
    }

    private void Update() {
        if(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()).direction == Vector3.zero) {

        }
    }
}
