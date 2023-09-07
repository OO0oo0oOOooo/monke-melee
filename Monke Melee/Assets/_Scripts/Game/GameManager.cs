using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private GameObject _cam;
    [SerializeField] private GameObject _crosshair;

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
        DisableMenuCamera();

        EventManager.Instance.OnPlayerDeath += OnPlayerDeath;
    }

    public override void OnNetworkDespawn()
    {
        EventManager.Instance.OnPlayerDeath -= OnPlayerDeath;
    }

    public void DisableMenuCamera()
    {
        _cam.SetActive(false);
        _crosshair.SetActive(true);
    }

    private void OnPlayerDeath(ulong deathId)
    {
        if(!IsServer) return;

        // Get NetworkObject from ClientID
        NetworkManager.Singleton.ConnectedClients[deathId].PlayerObject.Despawn();
        EventManager.Instance.OnPlayerRespawnEvent(deathId);
    }
}
