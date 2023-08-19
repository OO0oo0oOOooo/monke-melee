using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DMEnum : MonoBehaviour, IMenuItem
{
    [SerializeField] private DevMenuManager _devMenuManager;
    // [SerializeField] private int _parentMenuIndex;

    private Color _defaultColor = new Vector4(0, 0, 0, 0.6f);
    private Color _defaultTextColor = Color.white;

    private Color _highlightedColor = Color.white;
    private Color _highlightedTextColor = Color.black;

    [SerializeField] private Image _background;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Image _icon;

    public void ExecuteCommand()
    {
        Debug.Log("Execute Command Enum");
    }

    public void LeftCommand()
    {
        Debug.Log("Cycle Left Enum");
    }

    public void RightCommand()
    {
        Debug.Log("Cycle Right Enum");
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
