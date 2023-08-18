using System;
using UnityEngine;

[CreateAssetMenu]
public class AudioClips : ScriptableObject
{
    public AudioClipArray[] clipArrays;
}

[Serializable]
public class AudioClipArray
{
    public AudioClip[] clips;
};
