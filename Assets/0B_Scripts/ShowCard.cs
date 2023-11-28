using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ShowCard : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset _cardTemplate;
    [SerializeField] private List<UpgradeParts> _upgradeParts;

    private UIDocument _uiDocument;
    private VisualElement _contentBox;
    private VisualElement _selectedCardBox;
    private VisualElement _background;

    private UpgradeParts[] _selectedCards = new UpgradeParts[2];

    private void Awake() {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void Start() {
        var root = _uiDocument.rootVisualElement;

        _contentBox = root.Q<VisualElement>("content");
        _selectedCardBox = root.Q<VisualElement>("selected-card");
        _background = root.Q<VisualElement>("background");
    }

    public void ShowCards() {
        _background.style.visibility = Visibility.Visible;
        for(int i = 0; i < 3; i++) {
            UpgradeParts card = _upgradeParts[Random.Range(0, _upgradeParts.Count)];
            MakeCard(card, i);
            _upgradeParts.Remove(card);
        }
    }

    private async void MakeCard(UpgradeParts card, int num) {
        var template = _cardTemplate.Instantiate().Q<VisualElement>("card");

        template.Q<Label>("name").text = card.partsName;
        template.Q<VisualElement>("image").style.backgroundImage = new StyleBackground(card.partsSprite);
        template.Q<Label>("effect").text = card.effect;

        template.RegisterCallback<ClickEvent>(e => SelectCard(card));

        _contentBox.Add(template);
        await Task.Delay(100 + 500 * num);
        template.AddToClassList("on");
    }

    private void ShowSelectedCards() {
        for(int i = 0; i < _selectedCards.Length; i++) {
        if(_selectedCards[i] == null) break;

        var template = _cardTemplate.Instantiate().Q<VisualElement>("card");

        template.Q<Label>("name").text = _selectedCards[i].partsName;
        template.Q<VisualElement>("image").style.backgroundImage = new StyleBackground(_selectedCards[i].partsSprite);
        template.Q<Label>("effect").text = _selectedCards[i].effect;
        template.AddToClassList("on");

        template.RegisterCallback<ClickEvent>(e => SelectCard(_selectedCards[i]));

        _selectedCardBox.Add(template);
        }
    }

    private void SelectCard(UpgradeParts card) {
        if(card.name == "8ttruck") TCPClient.SendBuffer("8ttruck", RememberMe.Instance.team);
        else ChessBoard.upgradeParts[card.name] = true;
        _contentBox.Clear();
        _background.style.visibility = Visibility.Hidden;

        if(_selectedCards[0] == null) _selectedCards[0] = card;
        else _selectedCards[1] = card;

        ShowSelectedCards();
    }
}
