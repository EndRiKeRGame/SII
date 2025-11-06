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

    private LikesService _likesService;
    private ItemsConfig _itemsConfig;
    private ItemDistanceWithLikes _itemDistanceWithLikes;
    private FiltersService _filtersService;
    
    private Dictionary<Item, ItemCardView> _cardViews = new();

    [Inject]
    public void Construct(
        LikesService likesService,
        FiltersService filtersService,
        ItemsConfig itemStorage,
        ItemDistanceWithLikes itemDistanceWithLikes)
    {
        _likesService = likesService;
        _itemsConfig = itemStorage;
        _filtersService = filtersService;
        _itemDistanceWithLikes = itemDistanceWithLikes;
        _itemDistanceWithLikes.CalculateProximityForAllOnLikes();
        
        _likesService.OnLikesChanged += UpdateItemsPositions;
        _likesService.OnDislikesChanged += UpdateItemsPositions;
        _filtersService.OnFilterChanged += FilterItems;
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

    public void FilterItems(FilterItem filter)
    {
        foreach (var (item, view) in _cardViews)
        {
            view.gameObject.SetActive(CheckItemForFilter(item, filter));
        }
    }

    private void ShowAllItems()
    {
        foreach (var (_, view) in _cardViews)
        {
            view.gameObject.SetActive(true);
        }
    }

    private bool CheckItemForFilter(Item item, FilterItem filter)
    {
        if (!item.Name.Contains(filter.Name))
            return false;

        if (item.Price < filter.PriceFrom || item.Price > filter.PriceTo)
            return false;

        if (item.NumOfPlayers.Start.Value < filter.NumOfPlayersFrom ||
            item.NumOfPlayers.End.Value > filter.NumOfPlayersTo)
            return false;

        if (item.MinimumAge < filter.MinAge)
            return false;
        
        if (item.AvgPlayTime < filter.AvgPlayTimeFrom ||
            item.AvgPlayTime > filter.AvgPlayTimeTo)
            return false;

        foreach (var itemTag in filter.Tags)
            if (!item.Tags.Contains(itemTag))
                return false;

        return true;
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
            Destroy(view.gameObject);
        
        _cardViews.Clear();
    }
}
