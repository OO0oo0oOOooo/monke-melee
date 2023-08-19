using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DMCommand : MonoBehaviour, IMenuItem
{
    [SerializeField] private DevMenuManager _devMenuManager;
    [SerializeField] private int _parentMenuIndex;

    [SerializeField] private Color _defaultColor = new Vector4(0, 0, 0, 0.6f);
    private Color _defaultTextColor = Color.white;

    private Color _highlightedColor = Color.white;
    private Color _highlightedTextColor = Color.black;

    [SerializeField] private Image _background;
    [SerializeField] private TMP_Text _text;

    public void ExecuteCommand()
    {
        print("Execute Command");
        gameObject.GetComponent<IDMCommand>().EnterCommand();
    }

    public void LeftCommand()
    {
        _devMenuManager.UpdateCurrentMenu(_parentMenuIndex);
    }

    public void RightCommand()
    {
        return;
    }

    public void HighlightSelection()
    {
        _background.color = _highlightedColor;
        _text.color = _highlightedTextColor;
    }

    public void UnhighlightSelection()
    {
        _background.color = _defaultColor;
        _text.color = _defaultTextColor;
    }
}
