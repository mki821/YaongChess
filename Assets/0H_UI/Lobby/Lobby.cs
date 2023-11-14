using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Lobby : MonoBehaviour
{
    private int _team = 0;
    private bool _startable = false;
    private UIDocument _uiDocument;
    private VisualElement _background;

    private void Awake() {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void Start() {
        TCPClient.EventListener["set.team"] = SetTeam;
        TCPClient.EventListener["start.game"] = StartGame;
        TCPClient.EventListener["info.room"] = Startable;
        TCPClient.EventListener["disconnect.room"] = (LitJson.JsonData jsondata) => SceneManager.LoadScene(1);

        var root = _uiDocument.rootVisualElement;

        _background = root.Q<VisualElement>("content");

        root.Q<VisualElement>("btn-swap").RegisterCallback<ClickEvent>(e => Swap());
        root.Q<VisualElement>("btn-start").RegisterCallback<ClickEvent>(e => TCPClient.SendBuffer("start.game", null));

        if(RememberMe.Instance.team == Team.White)
            _background.style.backgroundColor = Color.white;
        else if(RememberMe.Instance.team == Team.Black)
            _background.style.backgroundColor = Color.black;
    }

    private void Startable(LitJson.JsonData jsondata) {
        if((int)jsondata == 2) _startable = true;
        else _startable = false;
    }

    private void StartGame(LitJson.JsonData jsondata) {
        TCPClient.SendBuffer("room.curInfo", null);

        if(_startable)
            SceneManager.LoadScene(3);
    }

    private void SetTeam(LitJson.JsonData jsondata) {
        _team = (int)jsondata;

        if(_team == 0)
            _background.style.backgroundColor = Color.white;
        else if(_team == 1)
            _background.style.backgroundColor = Color.black;
    }

    public void Swap() {
        TCPClient.SendBuffer("room.swap", null);
    }
}
