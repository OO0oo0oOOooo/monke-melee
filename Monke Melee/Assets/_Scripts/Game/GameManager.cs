using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private UIManager _uiManager;

    public static GameManager Instance { get; private set; }
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

    public override void OnNetworkSpawn()
    {
        _uiManager.DisableMenuCamera();

        EventManager.Instance.OnPlayerDeath += RespawnTimer;
    }

    public override void OnNetworkDespawn()
    {
        EventManager.Instance.OnPlayerDeath -= RespawnTimer;
    }

    private void RespawnTimer(ulong clientId)
    {
        if (!IsServer) return;

        StartCoroutine(RespawnPlayer(clientId));
    }

    IEnumerator RespawnPlayer(ulong clientId)
    {
        if (!IsServer) yield break;

        yield return new WaitForSeconds(3f);
        EventManager.Instance.InvokePlayerRespawnEvent(clientId);
    }
}
