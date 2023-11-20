using UnityEngine;
using UnityEngine.UIElements;

public class Promote : MonoBehaviour
{
    [SerializeField] private Selector _selector;

    private UIDocument _uiDocument;
    private VisualElement _promote;
    private Vector2Int pos;
    private Vector2Int targetPos;

    private void Awake() {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void Start() {
        var root = _uiDocument.rootVisualElement;

        _promote = root.Q<VisualElement>("promote");

        _promote.Q<VisualElement>("rook").RegisterCallback<ClickEvent>(e => P(Type.Rook));
        _promote.Q<VisualElement>("knight").RegisterCallback<ClickEvent>(e => P(Type.Knight));
        _promote.Q<VisualElement>("bishop").RegisterCallback<ClickEvent>(e => P(Type.Bishop));
        _promote.Q<VisualElement>("queen").RegisterCallback<ClickEvent>(e => P(Type.Queen));
    }

    private void P(Type type) {
        _selector.Promote(type, pos, targetPos);
        SelectComplete();
    }

    public void ShowPromote(Vector2Int pos, Vector2Int targetPos, Vector2 screenPos) {
        this.pos = pos;
        this.targetPos = targetPos;

        _promote.style.display = DisplayStyle.Flex;

        _promote.style.left = new StyleLength(screenPos.x - 215);
        _promote.style.top = new StyleLength(screenPos.y - 120);
    }

    public void SelectComplete() => _promote.style.display = DisplayStyle.None;
}
