using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using UnityEngine;
using UnityEngine.UI;

public class ItemCardGridView : MonoBehaviour
{
    [SerializeField]
    private LayoutGroup _gridLayout;
    
    [SerializeField]
    private ItemCardView _cardPrefab;
    
    private Dictionary<Item, ItemCardView> _cardViews = new();
    
    private Action<Item> _onLike;
    private Action<Item> _onDislike;
    
    public void SetupItems(Item[] items, Action<Item> onLike, Action<Item> onDislike)
    {
        _onLike = onLike;
        _onDislike = onDislike;
        
        foreach (var item in items)
        { 
            ItemCardView view = Instantiate(_cardPrefab, _gridLayout.transform);
            view.SetupItem(item);
            view.SetupButtons(() => _onLike(item), () => _onDislike(item));
            
            _cardViews.Add(item, view);
        }
    }

    public void ApplyFilter(bool[] mask)
    {
        int i = 0;
        foreach (var (_, view) in _cardViews)
        {
            var maskValue = mask[i];
            view.gameObject.SetActive(maskValue);

            i++;
        }
    }
    
    public void ApplyLikes(List<Item> likes, List<Item> dislikes)
    {
        foreach (var (item, view) in _cardViews)
        {
            view.gameObject.SetActive(!likes.Contains(item) && !dislikes.Contains(item));
        }
    }
    
    public void UpdateItemsPositions(List<Item> likes, List<Item> dislikes, Dictionary<Item, float> proximity)
    {
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
    
    public void OnDestroy()
    {
        foreach (var (_, view) in _cardViews)
        {
            view.ClearButtons();
            Destroy(view.gameObject);
        }
        
        _cardViews.Clear();
    }
}
