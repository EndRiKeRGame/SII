using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Configs;
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

    private Dictionary<Item, ItemCardView> _cardViews = new();
    private LikesService _likesService;
    private ItemsConfig _itemsConfig;
    private ItemDistanceWithLikes _itemDistanceWithLikes;

    [Inject]
    public void Construct(
        LikesService likesService,
        ItemsConfig itemStorage,
        ItemDistanceWithLikes itemDistanceWithLikes
        )
    {
        _likesService = likesService;
        _itemsConfig = itemStorage;
        _itemDistanceWithLikes = itemDistanceWithLikes;
        _itemDistanceWithLikes.CalculateProximityForAllOnLikes();
        
        _likesService.OnLikesChanged += UpdateItemsPositions;
        _likesService.OnDislikesChanged += UpdateItemsPositions;
    }
    
    public void SetupItems(Item[] items)
    {
        foreach (var item in items)
        { 
            ItemCardView view = Instantiate(_cardPrefab, _gridLayout.transform);
            view.SetupItem(item);
            view.SetupButtons(() => OnLike(item), () => OnDislike(item));
            
            _cardViews.Add(item, view);
        }
    }
    
    public void UpdateItemsPositions()
    {
        var proximity = _itemDistanceWithLikes.ProximityOnLikes;
        var likes = _likesService.Likes;
        var dislikes = _likesService.Dislikes;

        if (proximity.Count + likes.Count + dislikes.Count != _cardViews.Count)
            return;
        
        int currentPos = 0;
        foreach (var like in likes)
        {
            _cardViews[like].transform.SetSiblingIndex(currentPos++);
            _cardViews[like].SetProximityValue(100f);
        }
        
        var sortedProximity = proximity.OrderByDescending(x => x.Value);

        foreach (var (item, prox) in sortedProximity)
        {
            _cardViews[item].transform.SetSiblingIndex(currentPos++);
            _cardViews[item].SetProximityValue(prox);
        }

        foreach (var dislike in dislikes)
        {
            _cardViews[dislike].transform.SetSiblingIndex(currentPos++);
            _cardViews[dislike].SetProximityValue(0f);
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

    public void OnDestroy()
    {
        foreach (var (_, view) in _cardViews)
        {
            Destroy(view.gameObject);
        }
        
        _cardViews.Clear();
    }
}
