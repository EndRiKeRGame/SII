using Common;
using History;
using Shop;
using VContainer;

namespace Dev
{
    public class Facade
    {
        private readonly FiltersService _filtersService;
        private readonly LikesService _likeService;
        private readonly HistoryService _historyService;
        
        [Inject]
        public Facade(FiltersService filtersService, LikesService likeService, HistoryService historyService)
        {
            _filtersService = filtersService;
            _likeService = likeService;
            _historyService = historyService;
        }
        
        // add to like
        public void AddToLike(Item item)
        {
            _likeService.TryAddLike(item);
        }
        
        // add to dislike
        public void AddToDislike(Item item)
        {
            _likeService.TryAddDislike(item);
        }
        
        // apply filter
        public void ApplyParamsFilter()
        {
            var filter = _filtersService.GetFilterItem();
            // apply filter
        }
        
        // apply filter
        public void ApplyLikesFilter()
        {
            var likes = _likeService.Likes;
            var dislikes = _likeService.Dislikes;
            // apply filter
        }
        
        // save shop state
        public void SaveShopState()
        {
            _historyService.AddToHistory();
        }
        
        // undo shop state
        public void UndoShopState()
        {
            _historyService.UndoMove();
        }
    }
}