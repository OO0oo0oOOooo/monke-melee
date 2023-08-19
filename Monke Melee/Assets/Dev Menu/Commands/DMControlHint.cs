using UnityEngine;

public class DMControlHint : MonoBehaviour, IDMToggle
{
    public GameObject _controlHintGameobject;

    public void EnterCommand(bool b)
    {
        _controlHintGameobject.SetActive(b);
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
