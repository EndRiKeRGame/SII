using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsoleView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _log;

    [SerializeField]
    private CanvasGroup _canvasGroup;
    
    [SerializeField]
    private float _animationDuration = 0.5f;

    [SerializeField]
    private Button _hideButton;
    
    [SerializeField]
    private Button _fontSizeButton;
    
    [SerializeField]
    private Button _clearButton;

    private Tween _currentTween;
    
    private const int BIG_FONT = 50;
    private const int SMALL_FONT = 30;
    private bool _isFontBig = false;
    private bool _isActive = false;
    
    private void OnEnable()
    {
        _hideButton.onClick.AddListener(ChangeScrollActiveState);
        _fontSizeButton.onClick.AddListener(ChangeFontSize);
        _clearButton.onClick.AddListener(ClearTextOutput);
        _log.fontSize = SMALL_FONT;
        ClearTextOutput();
        ShowScroll();
    }

    private void OnDestroy()
    {
        _hideButton.onClick.RemoveListener(ChangeScrollActiveState);
        _fontSizeButton.onClick.RemoveListener(ChangeFontSize);
        _clearButton.onClick.RemoveListener(ClearTextOutput);
    }

    public void ClearTextOutput()
    {
        _log.text = "";
    }
    
    public void LogMessage(string msg)
    {
        _log.text += msg;
    }

    private void ChangeScrollActiveState()
    {
        if (_isActive)
            HideScroll();
        else
            ShowScroll();
    }

    private void ShowScroll()
    {
        if (_isActive)
            return;
        
        _currentTween.Stop();
        _currentTween = Tween.Alpha(_canvasGroup, 0, 1, _animationDuration);
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;

        _isActive = true;
    }
    
    private void HideScroll()
    {
        if (!_isActive)
            return;
        
        _currentTween.Stop();
        _currentTween = Tween.Alpha(_canvasGroup, 1, 0, _animationDuration);
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        _isActive = false;
    }
    
    private void HideScrollSilently()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
    
    private void ChangeFontSize()
    {
        _log.fontSize = _isFontBig ? SMALL_FONT : BIG_FONT;

        _isFontBig = !_isFontBig;
    }
}
