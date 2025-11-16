using System;
using Common;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace Shop
{
    public class FiltersService
    {
        public event Action OnFilterChanged;
        
        private readonly FiltersView _filtersView;
        
        [Inject]
        public FiltersService(FiltersView view)
        {
            _filtersView = view;
            Debug.Log("Filters service ready");
            
            _filtersView.FilterButton.onClick.AddListener(InvokeFilterChanged);
            _filtersView.ClearButton.onClick.AddListener(InvokeFilterChanged);
        }

        public void AdditionalInit(Action onFilterChanged)
        {
            _filtersView.FilterButton.onClick.AddListener(new UnityAction(onFilterChanged));
            _filtersView.ClearButton.onClick.AddListener(new UnityAction(onFilterChanged));
        }
        
        public void OnDestroy()
        {
            _filtersView.FilterButton.onClick.RemoveListener(InvokeFilterChanged);
            _filtersView.ClearButton.onClick.RemoveListener(InvokeFilterChanged);
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
        
        private void InvokeFilterChanged()
        {
            OnFilterChanged?.Invoke();
        }
    }
}