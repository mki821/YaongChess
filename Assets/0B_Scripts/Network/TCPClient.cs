using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPClient : MonoBehaviour
{
    [SerializeField] private ChessBoard _chessBoard;

    private Queue<string> commandQueue = new Queue<string>();
    private TcpClient tc;
    private NetworkStream stream;

    private void Start() {
        tc = new TcpClient("127.0.0.1", 5500);
        stream = tc.GetStream();

        Thread thread = new Thread(ReceiveBuffer);
        thread.Start();
    }

    private void Update() {
        if(commandQueue.Count > 0) {
            ReceiveChessInfo(commandQueue.Dequeue());
        }
    }

    class ChessInfo {
        public int[] selectTile;
        public bool isMove;
        public bool isAttack;
        public int[] moveTile;
        public Team team;
        public Type type;
    }

    public void ReceiveChessInfo(string jsondata) {
        Debug.Log(jsondata);
        ChessInfo decode = LitJson.JsonMapper.ToObject<ChessInfo>(jsondata);

        if(decode.isMove) {
            _chessBoard.Move(new Vector2Int(decode.selectTile[0], decode.selectTile[1]), new Vector2Int(decode.moveTile[0], decode.moveTile[1]), decode.isAttack);
        }
        else
            _chessBoard.Select(new Vector2Int(decode.selectTile[0], decode.selectTile[1]), decode.type, decode.team);
    }

    public void SendChessInfo(Type type, Vector2Int selectTile, bool isMove, Vector2Int moveTile, bool isAttack = false) {
        string jsondata = LitJson.JsonMapper.ToJson(new ChessInfo() {
            selectTile = new int[] { selectTile.x, selectTile.y },
            isMove = isMove,
            isAttack = isAttack,
            moveTile = new int[] { moveTile.x, moveTile.y },
            team = _chessBoard.team,
            type = type
        });
        SendBuffer(jsondata);
    }

    private void SendBuffer(string msg) {
        msg += "//ENDBUFFER//";
        byte[] buffer = Encoding.ASCII.GetBytes(msg);

        print("send!");
        stream.Write(buffer, 0, buffer.Length);
    }

    private void ReceiveBuffer() {
        Debug.Log("StartReceive");
        while(true) {
            byte[] outBuffer = new byte[1024];
            string output ="";
            Debug.Log("wait");
            while(!output.Contains("//ENDBUFFER//")) {
                if(stream.DataAvailable) {
                    int nbytes = stream.Read(outBuffer, 0, outBuffer.Length);
                    output += Encoding.ASCII.GetString(outBuffer, 0, nbytes);
                    print($"read! {nbytes}");
                }
            }
            commandQueue.Enqueue(output.Replace("//ENDBUFFER//", ""));
        }
    }
}
