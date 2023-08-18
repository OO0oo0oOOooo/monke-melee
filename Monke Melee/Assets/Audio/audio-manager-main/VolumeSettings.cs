using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _menuSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _gameSlider;

    const string MIXER_MASTER = "MasterVolume";
    const string MIXER_MENU = "MenuVolume";
    const string MIXER_MUSIC = "MusicVolume";
    const string MIXER_GAME = "GameVolume";

    private void Awake()
    {
        _masterSlider.onValueChanged.AddListener(SetMasterVolume);
        _menuSlider.onValueChanged.AddListener(SetMenuVolume);
        _musicSlider.onValueChanged.AddListener(SetMusicVolume);
        _gameSlider.onValueChanged.AddListener(SetGameVolume);
    }

    private void SetMasterVolume(float volume)
    {
        _mixer.SetFloat(MIXER_MASTER, Mathf.Log10(volume) * 20);
    }

    private void SetMenuVolume(float volume)
    {
        _mixer.SetFloat(MIXER_MENU, Mathf.Log10(volume) * 20);
    }

    private void SetMusicVolume(float volume)
    {
        _mixer.SetFloat(MIXER_MUSIC, Mathf.Log10(volume) * 20);
    }

    private void SetGameVolume(float volume)
    {
        _mixer.SetFloat(MIXER_GAME, Mathf.Log10(volume) * 20);
    }
}
