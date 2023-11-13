using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class RoomInfo {
    public int roomID;
    public string roomName;
    public int personnel;
}

public class RoomUI : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset _roomInfoTemplate;
    [SerializeField] private VisualTreeAsset _loadingPanelTemplate;

    private UIDocument _uiDocument;
    private VisualElement _roomList;
    private VisualElement _loadingPanel;

    private void Awake() {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void Start() {
        TCPClient.EventListener["refresh.room"] = RefreshRoom;
        TCPClient.EventListener["try.room"] = ConnectRoom;

        TCPClient.SendBuffer("room.refresh", null);

        var root = _uiDocument.rootVisualElement;

        _roomList = root.Q<VisualElement>("room-list");
        root.Q<VisualElement>("refresh").RegisterCallback<ClickEvent>(e => TCPClient.SendBuffer("room.refresh", null));
    }

    public void RefreshRoom(LitJson.JsonData jsondata) {
        _roomList.Clear();

        var roomInfos = LitJson.JsonMapper.ToObject<RoomInfo[]>((string)jsondata);

        foreach(RoomInfo item in roomInfos) {
            var template = _roomInfoTemplate.Instantiate().Q<VisualElement>("room");

            template.Q<Label>("room-name").text = item.roomName;
            template.Q<Label>("room-host").text = item.roomID.ToString();
            template.Q<Label>("room-personnel").text = $"{item.personnel} / 2";

            template.RegisterCallback<ClickEvent>(e =>TryConnectRoom(item.roomID));

            _roomList.Add(template);
        }
    }

    private void MakeRoom() {
        //
    }

    private void TryConnectRoom(int roomID) {
        TCPClient.SendBuffer("room.connect", roomID);

        if(_loadingPanel == null)
            _loadingPanel = _loadingPanelTemplate.Instantiate().Q<VisualElement>("loading-panel");
    }

    private void ConnectRoom(LitJson.JsonData jsondata) {
        if(jsondata.ToString() == "True") {
            SceneManager.LoadScene(2);
        }
        else {
            _loadingPanel.Clear();
        }
    }
}
