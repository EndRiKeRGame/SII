using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Configs;
using Dev;
using History;
using Math;
using UnityEngine;
using VContainer;

namespace Shop
{
    public class ShopInitializer : MonoBehaviour
    {
        [SerializeField]
        private ItemCardGridView _itemCardGridView;
        
        private ItemDistanceWithLikes _itemDistanceWithLikes;
        private ItemsConfig _itemsConfig;
        private Facade _facade;
        
        [Inject]
        public void Construct(
            Facade facade,
            ItemsConfig itemStorage,
            ItemDistanceWithLikes itemDistanceWithLikes)
        {
            _facade = facade;
            _itemsConfig = itemStorage;
            _itemDistanceWithLikes = itemDistanceWithLikes;
            _itemDistanceWithLikes.CalculateProximityForAllOnLikes();
            
            InitItems();
        }
        
        private void InitItems()
        {
            Item[] items = _itemsConfig.GetAllItems();
            _itemCardGridView.SetupItems(items, OnLike, OnDislike);
        }

        private void ApplyFilters(bool saveInHistory)
        {
            _facade.ApplyParamsFilter();
            _facade.ApplyLikesFilter();
            
            if (saveInHistory)
                AddToHistory();
        }
        
        private void ApplyParamsFilter(FilterItem filter)
        {
            var items = _itemsConfig.GetAllItems();
            bool[] mask = new bool[items.Length];
            for (var i = 0; i < items.Length; i++)
            {
                var item = items[i];
                mask[i] = CheckItemForFilter(item, filter);
            }

            _itemCardGridView.ApplyFilter(mask);
        }
        
        private void OnLike(Item item)
        {
            _likesService.TryAddLike(item);
            ApplyFilters(saveInHistory: true);
        }
    
        private void OnDislike(Item item)
        {
            _likesService.TryAddDislike(item);
            ApplyFilters(saveInHistory: true);
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
    }
}