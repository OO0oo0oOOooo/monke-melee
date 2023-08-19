using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DMSlider : MonoBehaviour, IMenuItem
{
    [SerializeField] private DevMenuManager _devMenuManager;
    // [SerializeField] private int _parentMenuIndex;

    private Color _defaultColor = new Vector4(0, 0, 0, 0.6f);
    private Color _defaultTextColor = Color.white;

    private Color _defaultSliderColor = new Vector4(1, 1, 1, 0.25f);
    private Color _defaultKnobColor = new Vector4(0.8f, 0.8f, 0.8f, 1);

    private Color _highlightedColor = Color.white;
    private Color _highlightedTextColor = Color.black;

    private Color _highlightedSliderColor = new Vector4(0, 0, 0, 0.25f);
    private Color _highlightedKnobColor = new Vector4(0.2f, 0.2f, 0.2f, 1);

    [SerializeField] private Image _background;
    [SerializeField] private TMP_Text _text;

    [SerializeField] private Image _arrowLeft;
    [SerializeField] private Image _arrowRight;
    [SerializeField] private Image _sliderBG;
    [SerializeField] private GameObject _knobIcon;

    public string _stringValue = "";

    public float _sliderValue = 0f;
    public float _maxValue = 1.2f;

    public void ExecuteCommand()
    {
        // Debug.Log("Execute Command");
        gameObject.GetComponent<IDMSlider>().EnterCommand(_stringValue);
    }

    public void LeftCommand()
    {
        _sliderValue -= 0.1f;
        _sliderValue = Mathf.Clamp(_sliderValue, 0.0001f, _maxValue);

        UpdateSlider();
        gameObject.GetComponent<IDMSlider>().LeftCommand(_sliderValue, _stringValue);
    }

    public void RightCommand()
    {
        _sliderValue += 0.1f;
        _sliderValue = Mathf.Clamp(_sliderValue, 0.0001f, _maxValue);

        UpdateSlider();
        gameObject.GetComponent<IDMSlider>().LeftCommand(_sliderValue, _stringValue);
    }

    public void HighlightSelection()
    {
        _background.color = _highlightedColor;
        _text.color = _highlightedTextColor;

        _arrowLeft.color = _highlightedTextColor;
        _arrowRight.color = _highlightedTextColor;

        _sliderBG.color = _highlightedSliderColor;
        _knobIcon.GetComponent<Image>().color = _highlightedKnobColor;
    }

    public void UnhighlightSelection()
    {
        _background.color = _defaultColor;
        _text.color = _defaultTextColor;

        _arrowLeft.color = _defaultTextColor;
        _arrowRight.color = _defaultTextColor;

        _sliderBG.color = _defaultSliderColor;
        _knobIcon.GetComponent<Image>().color = _defaultKnobColor;
    }

    public void UpdateSlider()
    {
        float knobX = Mathf.Lerp(-160, 160, _sliderValue);        
        _knobIcon.GetComponent<RectTransform>().localPosition = new Vector3(knobX, 0, 0);
    }
}
