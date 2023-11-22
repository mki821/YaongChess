using UnityEngine;
using UnityEngine.InputSystem;

public class Intro : MonoBehaviour
{
    private void Awake() {
        int width = Screen.width;
        int height = Screen.height;
        if(width >= 2560 && height >= 1440) Screen.SetResolution(2560, 1440, true);
        else if(width >= 1920 && height >= 1080) Screen.SetResolution(1920, 1080, true);
        else Screen.SetResolution(1280, 720, true);

        SoundManager.Instance.SetBGM(0);
    }

    private void Update() {
        if(Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame) {
            NetworkManager.Instance.StartGame();
        }
    }
}
