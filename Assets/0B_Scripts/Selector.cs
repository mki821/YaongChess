using UnityEngine;
using UnityEngine.InputSystem;

public class Selector : MonoBehaviour
{
    [SerializeField] private ChessBoard _chessBoard;
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private LayerMask _chessBoardLayer;
    [SerializeField] private LayerMask _selectableChessBoardLayer;

    private Transform _currentBoard;
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
            if(_currentBoard != null) {
                _currentBoard.transform.parent = hit.transform;
                _currentBoard.localPosition = new Vector3(0, _currentBoard.localPosition.y, 0);
                _chessBoard.DeselectAll();
            }
        }
        else if(Physics.Raycast(_cam.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, Mathf.Infinity, _chessBoardLayer)) {
            if(hit.transform.childCount > 0) {
                _currentBoard = hit.transform.GetChild(0);
                Piece piece = hit.transform.GetChild(0).GetComponent<ChessPiece>().piece;
                _chessBoard.Select(hit.transform.GetComponent<ChessTile>().pos, piece.type);
            }
            else
                _chessBoard.DeselectAll();
        }
        else {
            _chessBoard.DeselectAll();
        }
    }
}
