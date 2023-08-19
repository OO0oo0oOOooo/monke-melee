using UnityEngine;
using UnityEngine.Audio;

public class DMVolume : MonoBehaviour, IDMSlider
{
    [SerializeField] private AudioClip _testSound;
    [SerializeField] private AudioMixer _mixer;

    const string MIXER_MASTER = "MasterVolume";
    const string MIXER_MENU = "MenuVolume";
    const string MIXER_MUSIC = "MusicVolume";
    const string MIXER_GAME = "GameVolume";

    public void EnterCommand(string str)
    {   
        switch (str)
        {
            case "Master":
                break;
            case "Game":
                AudioSystem.Instance.PlayGameClipOneShot((int)GameAudioEnums.Bounce, 0, 1);
                break;
            case "Music":
                AudioSystem.Instance.PlayMusicClipOneShot((int)MusicAudioEnums.Default, 0, 1);
                break;
            case "Menu":
                AudioSystem.Instance.PlayMenuClipOneShot((int)MenuAudioEnums.ButtonClick, 0, 5);
                break;
            default:
                break;
        }
        

    }

    public void RightCommand(float value, string str)
    {
        SetMixerValues(str, value);
    }

    public void LeftCommand(float value, string str)
    {
        SetMixerValues(str, value);
    }

    private void SetMixerValues(string str, float value)
    {
        switch (str)
        {
            case "Master":
                _mixer.SetFloat(MIXER_MASTER, Mathf.Log10(value) * 20);
                break;
            case "Game":
                _mixer.SetFloat(MIXER_GAME, Mathf.Log10(value) * 20);
                break;
            case "Music":
                _mixer.SetFloat(MIXER_MUSIC, Mathf.Log10(value) * 20);
                break;
            case "Menu":
                _mixer.SetFloat(MIXER_MENU, Mathf.Log10(value) * 20);
                break;
            default:
                break;
        }
    }
}
