using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSystem : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _menuSource;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _gameSource;


    [Header("Audio Clips")]
    [SerializeField] private AudioClips _menuClips;
    [SerializeField] private AudioClips _musicClips;
    [SerializeField] private AudioClips _gameClips;


    [Header("PlayClipAtPoint Mixer")]
    [SerializeField] private AudioMixerGroup _mixerGroup;

    public static AudioSystem Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayMusicClipOneShot(int array, int index, float vol = 1)
    {
        AudioClip clip = _musicClips.clipArrays[array].clips[index];
        _musicSource.PlayOneShot(clip);
    }

    public void PlayMenuClipOneShot(int array, int index, float vol = 1)
    {
        AudioClip clip = _menuClips.clipArrays[array].clips[index];
        _menuSource.PlayOneShot(clip);
    }

    public void PlayGameClipOneShot(int array, int index, float vol = 1)
    {
        AudioClip clip = _gameClips.clipArrays[array].clips[index];
        _gameSource.PlayOneShot(clip, vol);
    }

    public void PlayClipAtPoint(int array, int index, Vector3 pos, float vol = 1, float pitch = 1)
    {
        AudioClip clip = _gameClips.clipArrays[array].clips[index];

        PlayOneShotAtPoint(clip, pos, vol, pitch);
    }

    public void PlayRandomClipAtPoint(int array, Vector3 pos, float vol = 1, float pitch = 1)
    {
        int i = Random.Range(0, _gameClips.clipArrays[array].clips.Length);
        AudioClip clip = _gameClips.clipArrays[array].clips[i];

        PlayOneShotAtPoint(clip, pos, vol, pitch);
    }

    // This parents a clip to a transform
    private Dictionary<int, AudioSource> _audioSources = new Dictionary<int, AudioSource>();

    public int GenerateId()
    {
        int id = _audioSources.Count;
        while (_audioSources.ContainsKey(id))
        {
            id++;
        }
        return id;
    }

    public int PlayOnParent(int array, int index, Transform parent, float volume = 1)
    {
        AudioClip clip = _gameClips.clipArrays[array].clips[index];

        GameObject audioObject = new GameObject("AudioObject");
        audioObject.transform.parent = parent;

        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = _mixerGroup;
        audioSource.loop = true;
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        
        int id = GenerateId();
        _audioSources[id] = audioSource;
        return id;
    }

    public void StopOnParent(int id)
    {
        if (_audioSources.ContainsKey(id))
        {
            AudioSource audioSource = _audioSources[id];
            audioSource.Stop();
            Destroy(audioSource.gameObject);
            _audioSources.Remove(id);
        }
    }

    // This is affected by the mixer
    private void PlayOneShotAtPoint(AudioClip clip, Vector3 position, float volume = 1, float pitch = 1)
    {
        GameObject audioObject = new GameObject("AudioObject");
        audioObject.transform.position = position;

        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = _mixerGroup;
        audioSource.volume = volume;
        audioSource.pitch = pitch;

        audioSource.PlayOneShot(clip, volume);

        Destroy(audioObject, clip.length);
    }

    // This uses unitys static method, which is not affected by the mixer
    public void PlayClipAtPointStatic(int array, int index, Vector3 pos, float vol = 1)
    {
        AudioClip clip = _gameClips.clipArrays[array].clips[index];

        AudioSource.PlayClipAtPoint(clip, pos, vol);
    }

    // This uses unitys static method, which is not affected by the mixer
    public void PlayRandomClipAtPointStatic(int array, Vector3 pos, float vol = 1)
    {
        int i = Random.Range(0, _gameClips.clipArrays[array].clips.Length);
        AudioClip clip = _gameClips.clipArrays[array].clips[i];

        AudioSource.PlayClipAtPoint(clip, pos, vol);
    }
}