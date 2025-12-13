using Common;
using Dev;
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
    
    [SerializeField]
    private BaseTabView _filterTabView;
    
    [SerializeField]
    private BaseTabView _chatTabView;
    
    private LikesService _likesService;
    private Facade _facade;
    
    [Inject]
    public void Constructor(
        LikesService likesService,
        Facade facade)
    {
        _likesService = likesService;
        _facade = facade;
    }

    public void Awake()
    {
        _likesService.OnLikesChanged += OnLikesChanged;
        _likesService.OnDislikesChanged += OnDislikesChanged;
        
        _likesTabView.Init(OnLikeTab);
        _dislikesTabView.Init(OnDislikeTab);
        _filterTabView.Init(OnFilterTab);
        _chatTabView.Init(OnChatTab);
        
        _dislikesTabView.ClearItems();
        _likesTabView.ClearItems();
    }
    
    public void OnDestroy()
    {
        _likesService.OnLikesChanged -= OnLikesChanged;
        _likesService.OnDislikesChanged -= OnDislikesChanged;
    }
    
    private void OnLikesChanged()
    {
        _likesTabView.ClearItems();
        
        var likes = _likesService.Likes;
        foreach (var like in likes)
            _likesTabView.AddItem(like, () =>
            {
                _likesService.TryRemoveLike(like);
                _facade.ApplyLikesFilter();
                _facade.SaveShopState();
            });
    }
    
    private void OnDislikesChanged()
    {
        _dislikesTabView.ClearItems();
        
        var dislikes = _likesService.Dislikes;
        foreach (var dislike in dislikes)
            _dislikesTabView.AddItem(dislike, () =>
            {
                _likesService.TryRemoveDislike(dislike);
                _facade.ApplyLikesFilter();
                _facade.SaveShopState();
            });
    }

    private void OnLikeTab()
    {
        if (_dislikesTabView.IsVisible())
            _dislikesTabView.Hide();
        
        if (_filterTabView.IsVisible())
            _filterTabView.Hide();
        
        if (_chatTabView.IsVisible())
            _chatTabView.Hide();
        
        if (!_likesTabView.IsVisible())
            _likesTabView.Show();
        else
            _likesTabView.Hide();
    }
    
    private void OnDislikeTab()
    {
        if (_likesTabView.IsVisible())
            _likesTabView.Hide();
        
        if (_filterTabView.IsVisible())
            _filterTabView.Hide();
        
        if (_chatTabView.IsVisible())
            _chatTabView.Hide();
        
        if (!_dislikesTabView.IsVisible())
            _dislikesTabView.Show();
        else
            _dislikesTabView.Hide();
    }
    
    private void OnFilterTab()
    {
        if (_likesTabView.IsVisible())
            _likesTabView.Hide();
        
        if (_dislikesTabView.IsVisible())
            _dislikesTabView.Hide();
        
        if (_chatTabView.IsVisible())
            _chatTabView.Hide();
        
        if (!_filterTabView.IsVisible())
            _filterTabView.Show();
        else
            _filterTabView.Hide();
    }
    
    private void OnChatTab()
    {
        if (_likesTabView.IsVisible())
            _likesTabView.Hide();
        
        if (_dislikesTabView.IsVisible())
            _dislikesTabView.Hide();
        
        if (_filterTabView.IsVisible())
            _filterTabView.Hide();
        
        if (!_chatTabView.IsVisible())
            _chatTabView.Show();
        else
            _chatTabView.Hide();
    }
}