using Common;
using Math;
using Shop;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class ItemCardGridView : MonoBehaviour
{
    [SerializeField]
    private LayoutGroup _gridLayout;
    
    [SerializeField]
    private ItemCardView _cardPrefab;
    
    private ItemDistance _itemDistance;
    private LikesService _likesService;

    [Inject]
    public void Construct(
        ItemDistance itemDistance,
        LikesService likesService)
    {
        _itemDistance = itemDistance;
        _likesService = likesService;
    }
    
    public void SetupItems(Item[] items)
    {
        foreach (var item in items)
        { 
            ItemCardView view = Instantiate(_cardPrefab, _gridLayout.transform);
            view.SetupItem(item);
            view.SetupButtons(() => OnLike(item), () => OnDislike(item));
        }
    }

    private void OnLike(Item item)
    {
        _likesService.TryAddLike(item);
        _likesService.InvokeLikesChanged();
    }
    
    private void OnDislike(Item item)
    {
        _likesService.TryAddDislike(item);
        _likesService.InvokeDislikesChanged();
    }
}
