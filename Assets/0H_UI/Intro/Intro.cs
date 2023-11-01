using UnityEngine;
using UnityEngine.UIElements;

public class Intro : MonoBehaviour
{
    private UIDocument _uiDocument;

    private void Awake() {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void Start() {
        var root = _uiDocument.rootVisualElement;

        root.Q<VisualElement>("btn-start").RegisterCallback<ClickEvent>(HandleStartGame);
    }

    private void HandleStartGame(ClickEvent evt)
    {
        NetworkManager.Instance.StartGame();
    }
}
