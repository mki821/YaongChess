using UnityEngine;
using UnityEngine.InputSystem;

public class Selector : MonoBehaviour
{
    [SerializeField] private ChessBoard _chessBoard;
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private LayerMask _chessBoardLayer;
    [SerializeField] private LayerMask _selectableChessBoardLayer;
    [SerializeField] private LayerMask _attackableChessBoardLayer;

    private Vector2Int _currentBoard = -Vector2Int.one;
    private Camera _cam;
    private TCPClient _tcpClient;

    private void Awake() {
        _cam = Camera.main;
        _tcpClient = GetComponent<TCPClient>();
    }

    private void Start() {
        _inputReader.OnMouseUp += Select;
    }

    private void OnDestroy() {
        _inputReader.OnMouseUp -= Select;
    }

    private void Select() {
        if(_chessBoard.team == Team.None) return;

        if(_chessBoard.curTeam == _chessBoard.team) {
            RaycastHit hit;
            if(Physics.Raycast(_cam.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, Mathf.Infinity, _selectableChessBoardLayer)) {
                if(_currentBoard != -Vector2Int.one) {
                    Vector2Int pos = hit.transform.GetComponent<ChessTile>().pos;
                    SendChessInfo(Type.None, _currentBoard, true, pos, false);
                    _currentBoard = -Vector2Int.one;
                }
            }
            else if(Physics.Raycast(_cam.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, Mathf.Infinity, _chessBoardLayer)) {
                if(hit.transform.childCount > 0) {
                    Piece piece = hit.transform.GetChild(0).GetComponent<ChessPiece>().piece;
                    if(piece.team == _chessBoard.team) {
                        _currentBoard = hit.transform.GetComponent<ChessTile>().pos;
                        SendChessInfo(piece.type, _currentBoard, false, -Vector2Int.one);
                    }
                }
                else {
                    _currentBoard = -Vector2Int.one;
                    _chessBoard.DeselectAll();
                }
            }
            else if(Physics.Raycast(_cam.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, Mathf.Infinity, _attackableChessBoardLayer)) {
                if(hit.transform.childCount > 0) {
                    Piece piece = hit.transform.GetChild(0).GetComponent<ChessPiece>().piece;
                    if(piece.team != _chessBoard.team) {
                        Vector2Int pos = hit.transform.GetComponent<ChessTile>().pos;
                        SendChessInfo(piece.type, _currentBoard, true, pos, true);
                        _currentBoard = -Vector2Int.one;
                    }
                }
            }
            else {
                _currentBoard = -Vector2Int.one;
                _chessBoard.DeselectAll();
            }
        }
    }

    private void SendChessInfo(Type type, Vector2Int selectTile, bool isMove, Vector2Int moveTile, bool isAttack = false) {
        TCPClient.SendBuffer("chess", new ChessInfo() {
            selectTile = new int[] { selectTile.x, selectTile.y },
            isMove = isMove,
            isAttack = isAttack,
            moveTile = new int[] { moveTile.x, moveTile.y },
            team = _chessBoard.team,
            type = type
        });
    }
}
