using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System.Threading;
using System.Collections.Generic;

public class Listener: MonoBehaviour
{
    [SerializeField] private bool host = false;

    private List<NetworkClient> clients = new List<NetworkClient>();
    private TcpListener listener;

    private void Awake() {
        if(host) {
            Thread thread = new(ListnerThread);
            thread.Start();
        }
    }

    private void OnDestroy() {
        if(host) {
            listener.Stop();
        }
    }

    private void ListnerThread() {
        listener = new TcpListener(IPAddress.Any, 5500);
        int count = 0;

        try {
            listener.Start();
            while(true) {
                TcpClient tc = listener.AcceptTcpClient();

                Thread th = new Thread(() => Send(tc));
                th.Start();

                clients.Add(new NetworkClient() {
                    name = count++.ToString(),
                    client = tc,
                    stream = tc.GetStream(),
                    thread = th
                });
            }
        }
        catch(SocketException ex) {
            Debug.LogError(ex.Message);
        }
    }

    private void Send(TcpClient tc) {
        byte[] buffer = new byte[1024];

        while(true) {
            try{
                NetworkStream stream = tc.GetStream();

                int bytes;
                print($"{tc} / wait...");
                while((bytes = stream.Read(buffer, 0, buffer.Length)) > 0) {
                    foreach(NetworkClient item in clients) {
                        item.stream.Write(buffer, 0, bytes);
                    }
                    //print($"{tc} / {Encoding.Default.GetString(buffer)}({bytes})");
                }
            }
            catch { break; }
        }
    }
}
