using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance = null;

    private string _hostName = "127.0.0.1";
    private int _port = 5500;


    public int Port { get => _port; set => _port = value; }
    public string HostName { get =>_hostName; set => _hostName = value; }
    public int ID { get; set; }

    private void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    public void StartGame() {
        SceneManager.LoadScene(1);
    }
}
