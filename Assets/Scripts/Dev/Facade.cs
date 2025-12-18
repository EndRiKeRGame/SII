using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Configs;
using History;
using Math;
using Shop;
using VContainer;

namespace Dev
{
    public class Facade
    {
        private readonly LikesService _likeService;
        private readonly HistoryService _historyService;
        private readonly ItemsConfig _itemsConfig;
        private readonly ItemCardGridView _itemCardGridView;
        private readonly FiltersService _filtersService;
        private readonly ItemDistanceWithLikes _itemDistanceWithLikes;
        
        [Inject]
        public Facade(
            FiltersService filtersService,
            LikesService likeService,
            HistoryService historyService,
            ItemCardGridView itemCardGridView,
            ItemsConfig itemsConfig,
            ItemDistanceWithLikes itemDistanceWithLikes)
        {
            _filtersService = filtersService;
            _likeService = likeService;
            _historyService = historyService;
            _itemCardGridView = itemCardGridView;
            _itemsConfig = itemsConfig;
            _itemDistanceWithLikes = itemDistanceWithLikes;
            
            _itemDistanceWithLikes.CalculateProximityForAllOnLikes();
        }

        public Item[] GetItems() => _itemsConfig.GetAllItems();
        
        public void AddToLike(Item item)
        {
            _likeService.TryAddLike(item);
        }
        
        public void AddToDislike(Item item)
        {
            _likeService.TryAddDislike(item);
        }
        
        public void RemoveLikeDislike(Item item)
        {
            if (_likeService.IsLike(item))
                _likeService.TryRemoveLike(item);
            else
                _likeService.TryRemoveDislike(item);
        }

        public void ShowAll()
        {
            _itemCardGridView.ShowAll();
        }
        
        public void ShowFirstX(int x = 3)
        {
            _itemCardGridView.ShowFirstX(x);
        }

        public void ClearFilters()
        {
            _filtersService.ClearFiltersView();
        }
        
        public FilterItem GetFilterItem()
        {
            return _filtersService.GetFilterItem();
        }
        
        public void ApplyParamsFilter()
        {
            var filter = _filtersService.GetFilterItem();
            
            ApplyParamsFilterWithParams(filter);
        }

        public void ApplyParamsFilterWithParams(FilterItem filter)
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
        
        public void ApplyLikesFilter()
        {
            var likes = _likeService.Likes.ToList();
            var dislikes = _likeService.Dislikes.ToList();

            ApplyLikesFilterWithParams(likes, dislikes);
        }
        
        private void ApplyLikesFilterWithParams(List<Item> likes, List<Item> dislikes)
        {
            _likeService.UpdateAllData(likes, dislikes);
            
            _itemCardGridView.UpdateItemsPositions(likes, dislikes, _itemDistanceWithLikes.CalculateProximityForAllOnLikes());
            _itemCardGridView.ApplyLikes(likes, dislikes);
        }
        
        public void SaveShopState()
        {
            _historyService.AddToHistory();
        }
        
        public void UndoShopState()
        {
            var prevState = _historyService.UndoMove();
            ApplyParamsFilterWithParams(prevState.Filter);
            ApplyLikesFilterWithParams(prevState.Likes, prevState.Dislikes);
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