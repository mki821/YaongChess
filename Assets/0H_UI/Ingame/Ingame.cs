using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Ingame : MonoBehaviour
{
    private UIDocument _uiDocument;
    private VisualElement _endingPanel;
    private Label _endingCountdown;
    private VisualElement _teamIcon;
    private Label _ingameTime;
    private Label _ingameTurn;
    private Team _curTeam = Team.White;
    private int _curSecond = 0;
    private float t = 0;

    private void Awake() {
        SoundManager.Instance.SetBGM(3);
        _uiDocument = GetComponent<UIDocument>();

        TCPClient.EventListener["end.game"] = EndGame;
    }

    private void Start() {
        var root = _uiDocument.rootVisualElement;

        _endingPanel = root.Q<VisualElement>("end-panel");
        _endingCountdown = _endingPanel.Q<Label>("ending-count");
        _teamIcon = root.Q<VisualElement>("what-team");
        _ingameTime = root.Q<Label>("ingame-time");
        _ingameTurn = root.Q<Label>("turn-text");
    }

    private void Update() {
        t += Time.deltaTime;

        if(t >= 1) {
            ++_curSecond;
            _ingameTime.text = $"{_curSecond / 60:D2}:{_curSecond % 60:D2}";
            t = 0;
        }
    }

    private void EndGame(LitJson.JsonData jsondata) {
        _endingPanel.style.display =  DisplayStyle.Flex;

        if((int)jsondata == (int)RememberMe.Instance.team) _endingPanel.Q<Label>("end-text").text = "승리";
        else _endingPanel.Q<Label>("end-text").text = "패배";

        StartCoroutine(GotoMainCountdown());
    }

    private IEnumerator GotoMainCountdown() {
        float t = 10;
        while(t > 0) {
            yield return null;
            t -= Time.deltaTime;
            _endingCountdown.text = $"{Mathf.Ceil(t)}초 후 방 선택으로 돌아감";
        }
        TCPClient.SendBuffer("room.delete", null);
        SceneManager.LoadScene(1);
    }

    public void ChangeTeam() {
        if(_curTeam == Team.White) {
            _teamIcon.style.unityBackgroundImageTintColor = Color.black;
            _curTeam = Team.Black;
        }
        else {
            _teamIcon.style.unityBackgroundImageTintColor = Color.white;
            _curTeam = Team.White;
        }
    }

    public void Turn(int turn) {
        _ingameTurn.text = $"{turn:D2}턴 째";
    }
} 
