using System;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemCardView : MonoBehaviour
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
    private Button _likeBtn;
    
    [SerializeField]
    private Button _dislikeBtn;
    
    [SerializeField]
    private TMP_Text _proximityValue;
    
    public string Name => _itemName.text;
    
    private UnityAction _onLike;
    private UnityAction _onDislike;
    
    public void SetupItem(Item item)
    {
        _itemImage.sprite = item.Sprite;
        _itemName.text = item.Name;
        
        _itemPrice.text = item.Price.ToString();
        _itemNumOfPlayers.text = $"{item.NumOfPlayers.Start.Value} - {item.NumOfPlayers.End.Value}";
        _itemAvgTime.text = item.AvgPlayTime.ToString();
        _itemMinAge.text = item.MinimumAge.ToString();
    }
    
    public void SetProximityValue(float value)
    {
        _proximityValue.text = value.ToString();
    }

    public void SetupButtons(Action onLike, Action onDislike)
    {
        _onLike = new UnityAction(onLike);
        _onDislike = new UnityAction(onDislike);
        
        _likeBtn.onClick.AddListener(_onLike);
        _dislikeBtn.onClick.AddListener(_onDislike);
    }
}
