using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject Crosshair;
    public GameObject DeathUI;
    public GameObject ControlHint;
    public GameObject NetworkButtons;
    public GameObject MenuCam;

    public void DisableMenuCamera()
    {
        MenuCam.SetActive(false);
        Crosshair.SetActive(true);
    }

    public void EnableMenuCamera()
    {
        MenuCam.SetActive(true);
        Crosshair.SetActive(false);
    }
}
