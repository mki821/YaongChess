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

        root.Q<TextField>("hostname").RegisterCallback<ChangeEvent<string>>(HandleSetHostName);
        root.Q<TextField>("port").RegisterCallback<ChangeEvent<string>>(HandleSetPort);
        root.Q<VisualElement>("btn-start").RegisterCallback<ClickEvent>(HandleStartGame);
    }

    private void HandleStartGame(ClickEvent evt)
    {
        NetworkManager.Instance.StartGame();
    }

    private void HandleSetHostName(ChangeEvent<string> evt)
    {
        NetworkManager.Instance.HostName = evt.newValue;
    }

    private void HandleSetPort(ChangeEvent<string> evt)
    {
        if (int.TryParse(evt.newValue, out int intValue))
            NetworkManager.Instance.Port = intValue;
    }
}
