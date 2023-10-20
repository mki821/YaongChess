using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class Selector : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private LayerMask _chessBoardLayer;
    [SerializeField] private LayerMask _selectableChessBoardLayer;

    private Transform _currentPiece;
    private Camera _cam;

    private void Awake() {
        _cam = Camera.main;
    }

    private void Start() {
        _inputReader.OnMouseUp += Select;
    }

    private void Select() {
        RaycastHit hit;
        if(Physics.Raycast(_cam.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, Mathf.Infinity, _selectableChessBoardLayer)) {
            if(_currentPiece != null) {
                _currentPiece.transform.parent = hit.transform;
            }
        }
        else if(Physics.Raycast(_cam.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, Mathf.Infinity, _chessBoardLayer)) {
            if(hit.transform.childCount > 0) {
                Debug.Log(hit.transform.name);
                _currentPiece = hit.transform.GetChild(0);
            }
        }
    }
}
