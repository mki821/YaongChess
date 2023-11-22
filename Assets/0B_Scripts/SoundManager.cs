using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance = null;

    public int masterVolume = 50;
    public int backgroundMusicVolume = 50;
    public int soundEffectVolume = 50;

    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioMixerGroup _bgmMixer;
    [SerializeField] private AudioMixerGroup _sfxMixer;

    private AudioSource _audioSource;

    private void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        _audioSource = GetComponent<AudioSource>();
    }

    public void SetMasterVolume(int volume) {
        masterVolume = volume;

        float setVolume = volume * 0.4f - 40;

        _audioMixer.SetFloat("Master", setVolume == -40f ? -80f : setVolume);
    }

    public void SetBGMVolume(int volume) {
        backgroundMusicVolume = volume;

        float setVolume = backgroundMusicVolume * 0.4f - 40;

        _audioMixer.SetFloat("BGM", setVolume == -40f ? -80f : setVolume);
    }
    
    public void SetSFXVolume(int volume) {
        soundEffectVolume = volume;

        float setVolume = soundEffectVolume * 0.4f - 40;

        _audioMixer.SetFloat("SFX", setVolume == -40f ? -80f : setVolume);
    }
}
