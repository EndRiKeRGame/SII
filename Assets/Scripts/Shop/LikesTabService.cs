using System.Collections.Generic;
using Common;
using PrimeTween;
using UnityEngine;
using Shop;
using VContainer;

public class LikesTabService : MonoBehaviour
{
    [SerializeField]
    private RectTransform _buttonsLayout;
    
    [SerializeField]
    private LikesTabView _likesTabView;
    
    [SerializeField]
    private LikesTabView _dislikesTabView;
    
    private LikesService _likesService;
    
    [Inject]
    public void Constructor(LikesService likesService)
    {
        _likesService = likesService;
    }

    public void Awake()
    {
        _likesService.OnLikesChanged += OnLikesChanged;
        _likesService.OnDislikesChanged += OnDislikesChanged;
        
        _likesTabView.ToggleButton.onClick.AddListener(OnLikeTab);
        _dislikesTabView.ToggleButton.onClick.AddListener(OnDislikeTab);
        
        _dislikesTabView.ClearItems();
        _likesTabView.ClearItems();
    }
    
    public void OnDestroy()
    {
        _likesService.OnLikesChanged -= OnLikesChanged;
        _likesService.OnDislikesChanged -= OnDislikesChanged;
        
        _likesTabView.ToggleButton.onClick.RemoveListener(OnLikeTab);
        _dislikesTabView.ToggleButton.onClick.RemoveListener(OnDislikeTab);
    }
    
    private void OnLikesChanged()
    {
        _likesTabView.ClearItems();
        
        var likes = _likesService.Likes;
        foreach (var like in likes)
        {
            _likesTabView.AddItem(like);
        }
    }
    
    private void OnDislikesChanged()
    {
        _dislikesTabView.ClearItems();
        
        var dislikes = _likesService.Dislikes;
        foreach (var dislike in dislikes)
        {
            _dislikesTabView.AddItem(dislike);
        }
    }

    private void OnLikeTab()
    {
        if (_dislikesTabView.IsVisible())
            _dislikesTabView.Hide();
        
        if (!_likesTabView.IsVisible())
            _likesTabView.Show();
        else
            _likesTabView.Hide();

        MoveButtons();
    }
    
    private void OnDislikeTab()
    {
        if (_likesTabView.IsVisible())
            _likesTabView.Hide();
        
        if (!_dislikesTabView.IsVisible())
            _dislikesTabView.Show();
        else
            _dislikesTabView.Hide();

        MoveButtons();
    }

    private void MoveButtons()
    {
        if (!_likesTabView.IsVisible() && !_dislikesTabView.IsVisible() && !Mathf.Approximately(_buttonsLayout.anchoredPosition.x, -295))
            Tween.UIAnchoredPositionX(_buttonsLayout, -295, 0.5f);
        else
            Tween.UIAnchoredPositionX(_buttonsLayout, 307, 0.5f);
    }
}