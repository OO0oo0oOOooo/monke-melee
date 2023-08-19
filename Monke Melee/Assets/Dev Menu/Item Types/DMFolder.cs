using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DMFolder : MonoBehaviour, IMenuItem
{
    private Color _defaultColor = new Vector4(0, 0, 0, 0.6f);
    private Color _defaultTextColor = Color.white;

    private Color _highlightedColor = Color.white;
    private Color _highlightedTextColor = Color.black;

    [SerializeField] private Image _background;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Image _icon;

    [SerializeField] private DevMenuManager _devMenuManager;

    [SerializeField] private int _parentMenuIndex;
    [SerializeField] private int _childMenuIndex;

    public void ExecuteCommand() => _devMenuManager.UpdateCurrentMenu(_childMenuIndex);

    public void RightCommand() => ExecuteCommand();

    public void LeftCommand() => _devMenuManager.UpdateCurrentMenu(_parentMenuIndex);

    public void HighlightSelection()
    {
        _background.color = _highlightedColor;
        _text.color = _highlightedTextColor;
        _icon.color = _highlightedTextColor;
    }

    public void UnhighlightSelection()
    {
        _background.color = _defaultColor;
        _text.color = _defaultTextColor;
        _icon.color = _defaultTextColor;
    }
}
