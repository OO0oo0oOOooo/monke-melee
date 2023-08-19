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
    }

    public void DisableMenuCamera()
    {
        _cam.SetActive(false);
        _crosshair.SetActive(true);
    }
}
