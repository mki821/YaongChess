using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using LitJson;

public class Lobby : MonoBehaviour
{
    private int _team = 0;
    private bool _startable = false;
    private bool _host = true; //I will change
    private UIDocument _uiDocument;
    private VisualElement _background;
    private Label player1;
    private Label player2;

    private void Awake() {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void Start() {
        TCPClient.EventListener["set.team"] = SetTeam;
        TCPClient.EventListener["namenamename"] = SetName;
        TCPClient.EventListener["start.game"] = StartGame;
        TCPClient.EventListener["info.room"] = Startable;
        TCPClient.EventListener["namenamename"] = SetName;
        TCPClient.EventListener["disconnect.room"] = (JsonData jsondata) => SceneManager.LoadScene(1);

        var root = _uiDocument.rootVisualElement;

        _background = root.Q<VisualElement>("content");

        root.Q<VisualElement>("btn-swap").RegisterCallback<ClickEvent>(e => Swap());
        root.Q<VisualElement>("btn-start").RegisterCallback<ClickEvent>(e => TCPClient.SendBuffer("start.game", null));
        root.Q<VisualElement>("btn-exit").RegisterCallback<ClickEvent>(e => ExitRoom());
        player1 = root.Q<Label>("player1-name");
        player2 = root.Q<Label>("player2-name");
    }

    private void Startable(JsonData jsondata) {
        if((int)jsondata == 2) _startable = true;
        else _startable = false;

        if(_startable)
            SceneManager.LoadScene(3);
    }

    private void StartGame(JsonData jsondata) {
        if(_host)
            TCPClient.SendBuffer("room.curInfo", null);
    }

    private void SetTeam(JsonData jsondata) {
        _team = (int)jsondata;
        RememberMe.Instance.team = _team == 0 ? Team.White : Team.Black;
    }

    private void SetName(JsonData jsondata) {
        Basic data = JsonMapper.ToObject<Basic>(jsondata.ToJson());

        if(player1.text == "비어있습니다") player2.text = "비어있습니다";
        else if(player2.text == "비어있습니다") player1.text = "비어있습니다";

        if((int)data.obj2 == 2) player1.text = (string)data.obj1;
        else player2.text = (string)data.obj1;
    }

    public void Swap() {
        TCPClient.SendBuffer("room.swap", null);
    }

    public void ExitRoom() {
        TCPClient.SendBuffer("room.exit", null);
        SceneManager.LoadScene(1);
    }
}
