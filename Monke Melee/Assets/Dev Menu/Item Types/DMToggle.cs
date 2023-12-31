using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DMToggle : MonoBehaviour, IMenuItem
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

    public bool _isOn = false;

    public void ExecuteCommand()
    {
        _isOn = !_isOn;
        
        _icon.gameObject.SetActive(_isOn);
        gameObject.GetComponent<IDMToggle>().EnterCommand(_isOn);
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
        // Set background to white
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
