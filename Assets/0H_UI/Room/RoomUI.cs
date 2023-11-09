using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

class RoomInfo {
    public int roomID;
    public string roomName;
    public int personnel;
}

public class RoomUI : MonoBehaviour
{
    private UIDocument _uiDocument;
    private VisualElement _roomList;

    private void Awake() {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void Start() {
        var root = _uiDocument.rootVisualElement;

        _roomList = root.Q<VisualElement>("room-list");
    }

    //public void RefreshRoom(Dictionary<int, RoomInfo> roomInfos) {

    //}
}
