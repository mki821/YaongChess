using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LitJson;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class TCPClient : MonoBehaviour
{
    public static Dictionary<string, UnityAction<JsonData>> EventListener = new();

    [SerializeField] private string IP = "172.31.3.146";
    [SerializeField] private int port = 5500;

    private Queue<Action> commandQueue = new Queue<Action>();
    private TcpClient tc;
    private NetworkStream stream;

    private static TCPClient instance;

    private void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        try {
            tc = new TcpClient(IP, port);
            stream = tc.GetStream();

            Thread thread = new Thread(ReceiveBuffer);
            thread.Start();
        }
        catch {
            RoomUI.ErrorPanel();
            return;
        }
    }

    private void OnDestroy() {
        if(instance == this) tc.Close();
    }

    private void Update() {
        if(commandQueue.Count > 0) {
            commandQueue.Dequeue()?.Invoke();;
        }
    }

    public static void SendBuffer(string type, object data) {
        byte[] buffer = Encoding.UTF8.GetBytes(new Box(type, data).ToJson() + "\\ENDBUFFER\\");

        instance.stream.Write(buffer, 0, buffer.Length);
    }

    private void ReceiveBuffer() {
        Debug.Log("StartReceive");
        while(true) {
            byte[] outBuffer = new byte[1024];
            StringBuilder output = new StringBuilder();
            Debug.Log("wait");
            while(!output.ToString().Contains("\\ENDBUFFER\\")) {
                if(stream.DataAvailable) {
                    int nbytes = stream.Read(outBuffer, 0, outBuffer.Length);
                    output.Append(Encoding.UTF8.GetString(outBuffer, 0, nbytes));
                    print($"read! {nbytes}");
                }
            }
            Debug.Log(output);

            string[] datas = output.ToString().Split("\\ENDBUFFER\\");

            for(int i = 0; i < datas.Length - 1; ++i) {
                JsonData decode = JsonMapper.ToObject(datas[i]);

                UnityAction<JsonData> CallBack;
                if (!EventListener.TryGetValue((string)decode["command"], out CallBack)){
                    Debug.LogError($"{decode["command"]} Trigger는 찾을 수 없습니다.");
                }
                else {
                    commandQueue.Enqueue(() => CallBack?.Invoke(decode["data"]));
                }
            }  
        }
    }
}
