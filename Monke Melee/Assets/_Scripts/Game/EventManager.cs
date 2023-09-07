using System;
using UnityEngine;
public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public event Action<ulong> OnPlayerRespawn;
    public void OnPlayerRespawnEvent(ulong clientId) => OnPlayerRespawn?.Invoke(clientId);

    public event Action<ulong> OnPlayerDeath;
    public void OnPlayerDeathEvent(ulong deathId) => OnPlayerDeath?.Invoke(deathId);
}
