using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ShowCard : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private VisualTreeAsset _cardTemplate;
    [SerializeField] private List<UpgradeParts> _upgradeParts;

    private VisualElement _contentBox;

    private void Start() {
        var root = _uiDocument.rootVisualElement;

        _contentBox = root.Q<VisualElement>("content");
    }

    private void Update() {
        if(Keyboard.current.spaceKey.wasPressedThisFrame) {
            ShowCards();
        }
    }

    private void ShowCards() {
        for(int i = 0; i < 3; i++) {
            UpgradeParts card = _upgradeParts[Random.Range(0, _upgradeParts.Count)];
            MakeCard(card.partsName, card.partsSprite, card.effect, i);
            _upgradeParts.Remove(card);
        }
    }

    private async void MakeCard(string name, Sprite sprite, string effect, int num) {
        var template = _cardTemplate.Instantiate().Q<VisualElement>("card");

        template.Q<Label>("name").text = name;
        template.Q<VisualElement>("image").style.backgroundImage = new StyleBackground(sprite);
        template.Q<Label>("effect").text = effect;

        template.RegisterCallback<ClickEvent>(e => Debug.Log(num));

        _contentBox.Add(template);
        await Task.Delay(100 + 500 * num);
        template.AddToClassList("on");
    }
}
