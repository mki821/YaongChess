using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheckPiece : MonoBehaviour
{
    public List<GameObject> icons = new List<GameObject>();

    private void Awake() {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach(Transform child in allChildren) {
            if(child.name.Contains("Icon")) {
                icons.Add(child.gameObject);
                child.gameObject.SetActive(false);
            }
        }
    }

    private void Update() {
        if(Mouse.current.rightButton.wasPressedThisFrame)
            OnAndOffIcon(true);
        else if(Mouse.current.rightButton.wasReleasedThisFrame)
            OnAndOffIcon(false);
    }
    
    private void OnAndOffIcon(bool show) {
        foreach(GameObject icon in icons) {
            if(icon == null) icons.Remove(icon);
            else icon.SetActive(show);
        }
    }
}
