using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public enum Resolution {
    ㅤ1280x720,
    ㅤ1920x1080,
    ㅤ2560x1440
}

public class Setting : MonoBehaviour
{
    [SerializeField] private bool _intro = false;

    private UIDocument _uiDocument;
    private VisualElement _settingPanel;

    public static Resolution _resolution;
    private bool _fullScreen = true;

    private void Awake() {
        _uiDocument = GetComponent<UIDocument>();
    }

    private void Start() {
        if(_intro) return;
        
        var root = _uiDocument.rootVisualElement;

        _settingPanel = root.Q<VisualElement>("setting-panel");

        root.Q<VisualElement>("setting-btn").RegisterCallback<ClickEvent>(e => OnAndOffSettingPanel());
        _settingPanel.Q<EnumField>("resolution-enum").RegisterValueChangedCallback(SetResolution);
        _settingPanel.Q<EnumField>("resolution-enum").SetValueWithoutNotify(_resolution);
        _settingPanel.Q<Toggle>("fullscreen").RegisterValueChangedCallback(e => _fullScreen = e.newValue);
        _settingPanel.Q<VisualElement>("complete-btn").RegisterCallback<ClickEvent>(e => CompleteSetting());
        _settingPanel.Q<VisualElement>("exit-btn").RegisterCallback<ClickEvent>(e => Application.Quit());

        _settingPanel.Q<SliderInt>("master-slider").SetValueWithoutNotify(SoundManager.Instance.masterVolume);
        _settingPanel.Q<SliderInt>("bgm-slider").SetValueWithoutNotify(SoundManager.Instance.backgroundMusicVolume);
        _settingPanel.Q<SliderInt>("sfx-slider").SetValueWithoutNotify(SoundManager.Instance.soundEffectVolume);

        _settingPanel.Q<SliderInt>("master-slider").RegisterValueChangedCallback(e => SoundManager.Instance.SetMasterVolume(e.newValue));
        _settingPanel.Q<SliderInt>("bgm-slider").RegisterValueChangedCallback(e => SoundManager.Instance.SetBGMVolume(e.newValue));
        _settingPanel.Q<SliderInt>("sfx-slider").RegisterValueChangedCallback(e => SoundManager.Instance.SetSFXVolume(e.newValue));
    }

    private void Update() {
        if(Keyboard.current.escapeKey.wasPressedThisFrame) {
            OnAndOffSettingPanel();
        }
    }

    public void OnAndOffSettingPanel(bool auto = false) {
        if(auto) _settingPanel.style.display = DisplayStyle.None;
        else _settingPanel.style.display = _settingPanel.style.display == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void SetResolution(ChangeEvent<Enum> e) {
        _resolution = (Resolution)e.newValue;
    }

    private void CompleteSetting() {
        switch(_resolution) {
            case Resolution.ㅤ1280x720:
                Screen.SetResolution(1280, 720, _fullScreen);
                break;
            case Resolution.ㅤ1920x1080:
                Screen.SetResolution(1920, 1080, _fullScreen);
                break;
            case Resolution.ㅤ2560x1440:
                Screen.SetResolution(2560, 1440, _fullScreen);
                break;
        }

        OnAndOffSettingPanel();
    }
}
