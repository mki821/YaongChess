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

    [SerializeField] private AudioClip[] _bgmAudios;
    [SerializeField] private AudioClip[] _sfxAudios;

    private AudioSource _bgmAudioSource;
    private AudioSource _sfxAudioSource;

    private void Awake() {
        if(Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        _bgmAudioSource = GetComponent<AudioSource>();
        _sfxAudioSource = gameObject.AddComponent<AudioSource>();

        
        float setVolume = masterVolume * 0.4f - 40;
        _audioMixer.SetFloat("Master", setVolume == -40f ? -80f : setVolume);
        setVolume = backgroundMusicVolume * 0.4f - 40;
        _audioMixer.SetFloat("Master", setVolume == -40f ? -80f : setVolume);
        setVolume = soundEffectVolume * 0.4f - 40;
        _audioMixer.SetFloat("Master", setVolume == -40f ? -80f : setVolume);
    }

    public void SetBGM(int index) {
        _bgmAudioSource.clip = _bgmAudios[index];
        _bgmAudioSource.Play();
    }

    public void PlaySFX(int index) {
        _sfxAudioSource.clip = _sfxAudios[index];
        _sfxAudioSource.Play();
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
