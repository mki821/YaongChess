using UnityEngine;
using UnityEngine.InputSystem;

public class Intro : MonoBehaviour
{
    private void Awake() {
        Screen.SetResolution(1920, 1080, true);
    }

    private void Update() {
        if(Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame) {
            NetworkManager.Instance.StartGame();
        }
    }
}
