using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using System.Collections.Generic;
using Common;
using TMPro;
using UnityEngine.Serialization;

public class LikesTabView : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Button _toggleButton;
    
    [SerializeField]
    private TMP_Text _toggleButton_Text;
    
    [SerializeField]
    private RectTransform _tabPanel;
    
    [SerializeField]
    private VerticalLayoutGroup _verticalLayout;
    
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [Header("Animation Settings")]
    [SerializeField]
    private float _animationDuration = 0.3f;
    
    [SerializeField]
    private Ease _easeType = Ease.OutCubic;
    
    [Header("Position Settings")]
    [SerializeField]
    private bool _hideToLeft = true;
    
    [SerializeField]
    private ItemCardView _itemCardViewPrefab;
    
    private bool _isVisible = true;
    private Sequence _animationSequence;
    private readonly List<ItemCardView> _contentItems = new List<ItemCardView>();
    private float _tabWidth;
    private Vector2 _shownPosition;
    private Vector2 _hiddenPosition;

    private void Awake()
    {
        _toggleButton.onClick.AddListener(ToggleTab);
        CalculatePositions();
        InitializeTab();
        
        Hide();
    }

    private void CalculatePositions()
    {
        _tabWidth = _tabPanel.rect.width;
        _shownPosition = _tabPanel.anchoredPosition;
        _hiddenPosition = _shownPosition;
        _hiddenPosition.x += _hideToLeft ? -_tabWidth : _tabWidth;
    }

    private void InitializeTab()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        
        _isVisible = true;
    }

    private void ToggleTab()
    {
        if (_animationSequence.isAlive)
            return;

        _isVisible = !_isVisible;
        AnimateTab();
    }

    private void AnimateTab()
    {
        // Останавливаем предыдущую анимацию
        _animationSequence.Complete();
        
        var targetPosition = _isVisible ? _shownPosition : _hiddenPosition;

        _animationSequence = Sequence.Create();

        // Анимируем основной блок
        _animationSequence.Group(Tween.UIAnchoredPosition(
            target: _tabPanel,
            endValue: targetPosition,
            duration: _animationDuration,
            ease: _easeType
        ));

        // Анимация прозрачности
        if (_canvasGroup != null)
        {
            _animationSequence.Group(Tween.Alpha(
                target: _canvasGroup,
                endValue: _isVisible ? 1f : 0f,
                duration: _animationDuration * 0.8f,
                ease: _easeType
            ));
            
            _canvasGroup.interactable = _isVisible;
            _canvasGroup.blocksRaycasts = _isVisible;
        }
    }
    
    public void AddItem(Item itemData)
    {
        var item = Instantiate(_itemCardViewPrefab, _verticalLayout.transform, false);
        item.SetupItem(itemData);
        _contentItems.Add(item);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_verticalLayout.GetComponent<RectTransform>());
    }
    
    public void ClearItems()
    {
        foreach (var item in _contentItems)
            Destroy(item);
        
        _contentItems.Clear();
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(_verticalLayout.GetComponent<RectTransform>());
    }

    public void Show()
    {
        if (_isVisible) return;
        
        _isVisible = true;
        AnimateTab();
    }

    public void Hide()
    {
        if (!_isVisible) return;
        
        _isVisible = false;
        AnimateTab();
    }

    private void OnDestroy()
    {
        _toggleButton.onClick.RemoveListener(ToggleTab);
        _animationSequence.Stop();
    }
}