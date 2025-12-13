using System;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LikedCardView : MonoBehaviour
{
    [SerializeField]
    private Image _itemImage;
    
    [SerializeField]
    private TMP_Text _itemName;
    
    [SerializeField]
    private TMP_Text _itemPrice;
    
    [SerializeField]
    private TMP_Text _itemNumOfPlayers;
    
    [SerializeField]
    private TMP_Text _itemAvgTime;
    
    [SerializeField]
    private TMP_Text _itemMinAge;
    
    [SerializeField]
    private Button _removeBtn;
    
    public string Name => _itemName.text;
    
    private UnityAction _onRemove;
    
    public void SetupItem(Item item)
    {
        _itemImage.sprite = item.Sprite;
        _itemName.text = item.Name;
        
        _itemPrice.text = item.Price.ToString();
        _itemNumOfPlayers.text = $"{item.NumOfPlayers.Start.Value} - {item.NumOfPlayers.End.Value}";
        _itemAvgTime.text = item.AvgPlayTime.ToString();
        _itemMinAge.text = item.MinimumAge.ToString();
    }

    public void SetupButtons(Action onRemove)
    {
        _onRemove = new UnityAction(onRemove);
        _removeBtn.onClick.AddListener(_onRemove);
    }
    
    public void OnDestroy()
    {
        _removeBtn.onClick.RemoveListener(_onRemove);
    }
}
