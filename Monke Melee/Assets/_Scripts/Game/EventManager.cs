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
    public void InvokePlayerRespawnEvent(ulong clientId) => OnPlayerRespawn?.Invoke(clientId);

    public event Action<ulong> OnPlayerDeath;
    public void InvokePlayerDeathEvent(ulong deathId) => OnPlayerDeath?.Invoke(deathId);
}
