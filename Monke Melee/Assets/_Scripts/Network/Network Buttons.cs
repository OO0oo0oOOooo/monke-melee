using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkButtons : MonoBehaviour
{
    [SerializeField] private GameObject _networkButtons;

    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _joinButton;

    private void Awake()
    {
        _networkButtons.SetActive(true);

        _hostButton.onClick.AddListener(() => 
        {
            NetworkManager.Singleton.StartHost();
            _networkButtons.SetActive(false);
        });
        _joinButton.onClick.AddListener(() => 
        {
            NetworkManager.Singleton.StartClient();
            _networkButtons.SetActive(false); 
        });
    }
}
