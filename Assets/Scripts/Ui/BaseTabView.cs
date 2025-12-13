using System;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using UnityEngine.Events;

public class BaseTabView : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Button _toggleButton;
    
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [Header("Animation Settings")]
    [SerializeField]
    private float _animationDuration = 0.3f;
    
    [SerializeField]
    private Ease _easeType = Ease.OutCubic;
    
    private bool _isVisible = true;
    private Sequence _animationSequence;
    private UnityAction _onButtonClick;

    private void OnValidate()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Awake()
    {
        InitializeTab();
        Hide();
    }

    public void Init(Action onButtonClick)
    {
        _onButtonClick = new UnityAction(onButtonClick);
        _toggleButton.onClick.AddListener(_onButtonClick);
    }

    private void InitializeTab()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        
        _isVisible = false;
    }

    private void AnimateTab()
    {
        _animationSequence.Stop();
        _animationSequence = Sequence.Create();

        if (_canvasGroup == null)
            return;
        
        _animationSequence.Group(Tween.Alpha(
            target: _canvasGroup,
            endValue: _isVisible ? 1f : 0f,
            duration: _animationDuration * 0.8f,
            ease: _easeType
        ));
            
        _canvasGroup.interactable = _isVisible;
        _canvasGroup.blocksRaycasts = _isVisible;
    }

    public void Show()
    {
        if (_isVisible)
            return;
        
        _isVisible = true;
        AnimateTab();
    }

    public void Hide()
    {
        if (!_isVisible)
            return;
        
        _isVisible = false;
        AnimateTab();
    }
    
    public bool IsVisible()
    {
        return _isVisible;
    }

    private void OnDestroy()
    {
        _toggleButton.onClick.RemoveAllListeners();
        _animationSequence.Stop();
    }
}