using Common;
using VContainer;

namespace Shop
{
    public class FiltersService
    {
        private readonly FiltersView _filtersView;
        
        [Inject]
        public FiltersService(FiltersView filtersView)
        {
            _filtersView = filtersView;
        }

        public void ClearFiltersView()
        {
            _filtersView.ClearFilters();
        }
        
        public FilterItem CreateEmptyFilterItem()
        {
            return new FilterItem
            {
                Name = "",
                PriceFrom = -1,
                PriceTo = int.MaxValue,
                NumOfPlayersFrom = -1,
                NumOfPlayersTo = int.MaxValue,
                MinAge = -1,
                AvgPlayTimeFrom = -1,
                AvgPlayTimeTo = int.MaxValue,
                Tags = new(),
            };
        }

        public FilterItem GetFilterItem()
        {
            var filter = CreateEmptyFilterItem();
            _filtersView.ApplyFilters(ref filter);
            
            return filter;
        }
    }
}