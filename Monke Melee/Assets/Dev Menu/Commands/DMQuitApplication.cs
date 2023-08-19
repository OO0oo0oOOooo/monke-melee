using UnityEngine;

public class DMQuitApplication : MonoBehaviour, IDMCommand
{
    public void EnterCommand()
    {
        Application.Quit();
    }

    public void RightCommand()
    {
        throw new System.NotImplementedException();
    }

    public void LeftCommand()
    {
        throw new System.NotImplementedException();
    }
}
