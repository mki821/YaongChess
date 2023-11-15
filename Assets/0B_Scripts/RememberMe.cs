using UnityEngine;

public class RememberMe : MonoBehaviour
{
    public static RememberMe Instance = null;

    public Team team;

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
    }

    private void SetTeam(LitJson.JsonData jsondata) {
        if ((int)jsondata == 0) team = Team.White;
        else team = Team.Black;
    }
}
