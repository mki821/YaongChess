using UnityEngine;

public class Room : MonoBehaviour
{
    private void Update() {
        if(Input.GetKeyDown(KeyCode.L)) {
            TCPClient.SendBuffer("room.make", "akdsg");
        }
        else if(Input.GetKeyDown(KeyCode.P)) {
            TCPClient.SendBuffer("room.refresh", null);
        }
    }
}
