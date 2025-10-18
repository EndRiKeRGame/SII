using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using TriInspector;
using UnityEngine;

namespace Common.Configs
{
    [CreateAssetMenu(fileName = nameof(ItemsConfig), menuName = "Configs/" + nameof(ItemsConfig))]
    public class ItemsConfig : ScriptableObject
    {
        [SerializeField]
        private Item[] _items;
        
        [field: SerializeField, ReadOnly]
        public Item MaxItem { get; set; }
        
        [field: SerializeField, ReadOnly]
        public Item MinItem { get; set; }
        
        [field: SerializeField, ReadOnly]
        public Item AvgItem { get; set; }

        [Button]
        public void UpdateMaxMinAvr()
        {
            MaxItem = new Item();
            MinItem = new Item();
            AvgItem = new Item();
            
            foreach (var item in _items)
            {
                if (item.Price > MaxItem.Price)
                    MaxItem.Price = item.Price;
                if (item.Price < MinItem.Price || MinItem.Price == -1)
                    MinItem.Price = item.Price;
                
                AvgItem.Price += item.Price;
                
                if (item.NumOfPlayers.End.Value > MaxItem.NumOfPlayers.End.Value)
                    MaxItem.NumOfPlayers = ..item.NumOfPlayers.End.Value;
                if (item.NumOfPlayers.Start.Value < MinItem.NumOfPlayers.Start.Value || MinItem.NumOfPlayers.Start.Value == 0)
                    MinItem.NumOfPlayers = item.NumOfPlayers.Start.Value..;
                
                if (item.AvgPlayTime > MaxItem.AvgPlayTime)
                    MaxItem.AvgPlayTime = item.AvgPlayTime;
                if (item.AvgPlayTime < MinItem.AvgPlayTime || MinItem.AvgPlayTime == -1)
                    MinItem.AvgPlayTime = item.AvgPlayTime;
                
                AvgItem.AvgPlayTime += item.AvgPlayTime;
                
                if (item.MinimumAge > MaxItem.MinimumAge)
                    MaxItem.MinimumAge = item.MinimumAge;
                if (item.MinimumAge < MinItem.MinimumAge || MinItem.MinimumAge == -1)
                    MinItem.MinimumAge = item.MinimumAge;
                
                AvgItem.MinimumAge += item.MinimumAge;
            }
            
            var length = _items.Length;
            AvgItem.Price /= length;
            AvgItem.AvgPlayTime /= length;
            AvgItem.MinimumAge /= length;
        }

        public Item[] GetAllItems() => _items;

        public Item[] GetItemsByPrice(int price, Item[] selectedItems = null)
        {
            selectedItems ??= _items;
            return selectedItems.Where(x => x.Price <= price).ToArray();
        }
        
        public Item[] GetItemsByTags(List<ItemTag> tags, Item[] selectedItems = null)
        {
            selectedItems ??= _items;
            return selectedItems.Where(x => x.Tags.Any(y => tags.Contains(y))).ToArray();
        }

        public bool TryGetItemByName(string name, out Item neededItem)
        {
            foreach (var item in _items)
            {
                if (item.Name != name)
                    continue;
                
                neededItem = item;
                return true;
            }

            neededItem = null;
            return false;
        }
    }
}