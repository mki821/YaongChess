using UnityEngine;
using UnityEngine.InputSystem;

public class Intro : MonoBehaviour
{
    private void Update() {
        if(Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame) {
            NetworkManager.Instance.StartGame();
        }
    }
}
