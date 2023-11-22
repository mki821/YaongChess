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

    private static UIDocument _uiDocument;
    private VisualElement _roomList;
    private VisualElement _loadingPanel;
    private VisualElement _roomMakePanel;

    private void Awake() {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void Start() {
        TCPClient.EventListener["success.room"] = GotoLobby;
        TCPClient.EventListener["refresh.room"] = RefreshRoom;
        TCPClient.EventListener["try.room"] = ConnectRoom;

        TCPClient.SendBuffer("room.refresh", null);

        var root = _uiDocument.rootVisualElement;

        _roomList = root.Q<VisualElement>("room-list");
        root.Q<VisualElement>("refresh").RegisterCallback<ClickEvent>(e => TCPClient.SendBuffer("room.refresh", null));
        root.Q<VisualElement>("btn-maker").RegisterCallback<ClickEvent>(e => MakeRoom());
        root.Q<TextField>("input-name").RegisterValueChangedCallback(e => SetName(root.Q<TextField>("input-name").text));

        _roomMakePanel = root.Q<VisualElement>("panel");
        _loadingPanel = root.Q<VisualElement>("loading-panel");
    }

    private void GotoLobby(LitJson.JsonData jsondata) {
        SceneManager.LoadScene(2);
    }

    private void SetName(string name) => RememberMe.Instance.name = name;

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
        _roomMakePanel.style.display = DisplayStyle.Flex;
        _roomMakePanel.Q<VisualElement>("btn-make").RegisterCallback<ClickEvent>(e => TCPClient.SendBuffer("room.make", _roomMakePanel.Q<TextField>("room-name").text));
    }

    private void TryConnectRoom(int roomID) {
        TCPClient.SendBuffer("room.connect", roomID);
        _loadingPanel.style.display = DisplayStyle.Flex;
    }

    private void ConnectRoom(LitJson.JsonData jsondata) {
        if((string)jsondata == "True") SceneManager.LoadScene(2);
        else _loadingPanel.style.display = DisplayStyle.None;
    }

    public static void ErrorPanel() {
        _uiDocument.rootVisualElement.Q<VisualElement>("error-panel").style.display = DisplayStyle.Flex;
    }
}
