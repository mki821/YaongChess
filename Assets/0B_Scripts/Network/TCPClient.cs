using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LitJson;
using UnityEngine;
using UnityEngine.Events;

public class TCPClient : MonoBehaviour
{
    public static Dictionary<string, UnityAction<JsonData>> EventListener = new();

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
        tc = new TcpClient(NetworkManager.Instance.HostName, NetworkManager.Instance.Port);
        stream = tc.GetStream();

        Thread thread = new Thread(ReceiveBuffer);
        thread.Start();
    }

    private void Update() {
        if(commandQueue.Count > 0) {
            commandQueue.Dequeue()?.Invoke();;
        }
    }

    public static void SendBuffer(string type, object data) {
        byte[] buffer = Encoding.ASCII.GetBytes(new Box(type, data).ToJson() + "//ENDBUFFER//");

        instance.stream.Write(buffer, 0, buffer.Length);
    }

    private void ReceiveBuffer() {
        Debug.Log("StartReceive");
        while(true) {
            byte[] outBuffer = new byte[1024];
            StringBuilder output = new StringBuilder();
            Debug.Log("wait");
            while(!output.ToString().Contains("//ENDBUFFER//")) {
                if(stream.DataAvailable) {
                    int nbytes = stream.Read(outBuffer, 0, outBuffer.Length);
                    output.Append(Encoding.ASCII.GetString(outBuffer, 0, nbytes));
                    print($"read! {nbytes}");
                }
            }

            
            JsonData decode = JsonMapper.ToObject(output.ToString());

            UnityAction<JsonData> CallBack;
            if (!EventListener.TryGetValue((string)decode["command"], out CallBack)){
                Debug.LogError($"{decode["command"]} Trigger는 찾을 수 없습니다.");
            }
            else {
                commandQueue.Enqueue(() => CallBack.Invoke(decode["data"]));
                /*print($"Connected Server | PlayerID : {output.ToString().Replace("//ENDBUFFER//", "")}");
                NetworkManager.Instance.ID = int.Parse(output.ToString().Replace("//ENDBUFFER//", ""));
                _chessBoard.team = NetworkManager.Instance.ID == 1 ? Team.White : Team.Black;*/
            }
        }
    }
}
