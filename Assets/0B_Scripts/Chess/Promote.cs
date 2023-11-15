using UnityEngine;
using UnityEngine.UIElements;

public class Promote : MonoBehaviour
{
    [SerializeField] private Selector _selecotr;

    private UIDocument _uiDocument;
    private VisualElement _promote;
    private Vector2Int pos;

    private void Awake() {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void Start() {
        var root = _uiDocument.rootVisualElement;

        _promote = root.Q<VisualElement>("promote");

        _promote.Q<VisualElement>("bishop").RegisterCallback<ClickEvent>(e =>_selecotr.Promote(Type.Bishop, pos));
    }

    public void ShowPromote(Vector2Int pos, Vector2 screenPos) {
        this.pos = pos;

        _promote.style.display = DisplayStyle.Flex;

        _promote.style.left = screenPos.x;
        _promote.style.top = screenPos.y;
    }
    
    public void SelectComplete() => _promote.style.display = DisplayStyle.None;
}
