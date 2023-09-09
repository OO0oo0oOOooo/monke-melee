using UnityEngine;
using TMPro;
using System.Collections;

public class RespawnTimerUI : MonoBehaviour
{
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private TMP_Text _timerText;

    [SerializeField] private int _respawnTime = 3;

    private void Awake()
    {
        EventManager.Instance.OnPlayerDeath += RespawnTimer;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnPlayerDeath -= RespawnTimer;
    }

    private void RespawnTimer(ulong clientId)
    {
        _uiManager.Crosshair.SetActive(false);
        _uiManager.DeathUI.SetActive(true);
        _timerText.text = "3";

        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        while(_respawnTime > 0)
        {
            yield return new WaitForSeconds(1f);
            _respawnTime--;
            _timerText.text = _respawnTime.ToString();
        }

        _uiManager.Crosshair.SetActive(true);
        _uiManager.DeathUI.SetActive(false);
    }
}
