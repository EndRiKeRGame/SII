using System.Collections.Generic;
using System.Linq;
using Common;
using Shop;
using VContainer;

namespace History
{
    public class HistoryService
    {
        private LikesService _likesService;
        private FiltersService _filtersService;
        
        private Stack<HistoryStep> _history = new();

        [Inject]
        public HistoryService(LikesService likesService, FiltersService filtersService)
        {
            _likesService = likesService;
            _filtersService = filtersService;
        }

        public void AddToHistory()
        {
            HistoryStep step = new HistoryStep
            {
                Likes = _likesService.Likes.ToList(),
                Dislikes = _likesService.Dislikes.ToList(),
                Filter = _filtersService.GetFilterItem()
            };

            _history.Push(step);
        }

        public HistoryStep UndoMove()
        {
            if (_history.Count <= 1)
            {
                _history.Clear();
                return CreateEmptyHistoryStep();
            }

            _history.Pop();
            return _history.Peek();
        }
        
        private HistoryStep CreateEmptyHistoryStep()
        {
            return new HistoryStep
            {
                Likes = new List<Item>(),
                Dislikes = new List<Item>(),
                Filter = _filtersService.CreateEmptyFilterItem()
            };
        }
    }
}