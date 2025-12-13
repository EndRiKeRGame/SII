using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Common;

public class LikesTabView : BaseTabView
{
    [SerializeField]
    private VerticalLayoutGroup _verticalLayout;
    
    [SerializeField]
    private LikedCardView _itemCardViewPrefab;
    
    private readonly List<LikedCardView> _contentItems = new();

   public void AddItem(Item itemData, Action onRemove)
    {
        var item = Instantiate(_itemCardViewPrefab, _verticalLayout.transform, false);
        item.SetupItem(itemData);
        item.SetupButtons(onRemove);
        _contentItems.Add(item);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_verticalLayout.GetComponent<RectTransform>());
    }
    
    public void ClearItems()
    {
        foreach (var item in _contentItems)
            Destroy(item.gameObject);
        
        _contentItems.Clear();
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(_verticalLayout.GetComponent<RectTransform>());
    }
}