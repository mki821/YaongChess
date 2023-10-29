using TMPro.EditorUtilities;
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

    private void Awake() {
        _cam = Camera.main;
    }

    private void Start() {
        _inputReader.OnMouseUp += Select;
    }

    private void OnDestroy() {
        _inputReader.OnMouseUp -= Select;
    }

    private void Select() {
        RaycastHit hit;
        if(Physics.Raycast(_cam.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, Mathf.Infinity, _selectableChessBoardLayer)) {
            if(_currentBoard != -Vector2Int.one) {
                _chessBoard.Move(_currentBoard, hit.transform.GetComponent<ChessTile>().pos, false);
                _currentBoard = -Vector2Int.one;
            }
        }
        else if(Physics.Raycast(_cam.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, Mathf.Infinity, _chessBoardLayer)) {
            if(hit.transform.childCount > 0) {
                _currentBoard = hit.transform.GetComponent<ChessTile>().pos;
                Piece piece = hit.transform.GetChild(0).GetComponent<ChessPiece>().piece;
                if(piece.team == _chessBoard.team) {
                    _chessBoard.Select(hit.transform.GetComponent<ChessTile>().pos, piece.type);
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
                    _chessBoard.Move(_currentBoard, hit.transform.GetComponent<ChessTile>().pos, true);
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
