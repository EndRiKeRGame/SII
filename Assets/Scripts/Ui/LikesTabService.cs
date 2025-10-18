using UnityEngine;
using Shop;
using VContainer;

public class LikesTabService : MonoBehaviour
{
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
}