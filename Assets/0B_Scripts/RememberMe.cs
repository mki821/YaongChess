using UnityEngine;

public class RememberMe : MonoBehaviour
{
    public static RememberMe Instance = null;

    public Team team;
    new public string name = "플레이어";

    private void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        TCPClient.EventListener["set.team"] = SetTeam;
        TCPClient.EventListener["wyn.lobby"] = SendName;
        TCPClient.EventListener["namenamename"] = null;
    }

    private void SetTeam(LitJson.JsonData jsondata) {
        if ((int)jsondata == 0) team = Team.White;
        else team = Team.Black;

        TCPClient.SendBuffer("wyn.lobby", null);
    }

    private void SendName(LitJson.JsonData jsondata) {
        TCPClient.SendBuffer("namenamename", new Basic() {
            obj1 = name,
            obj2 = team
        });
    }
}
