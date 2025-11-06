using System;
using Common;
using UnityEngine;
using VContainer;

namespace Shop
{
    public class FiltersService
    {
        public event Action<FilterItem> OnFilterChanged;
        
        private readonly FiltersView _filtersView;
        
        [Inject]
        public FiltersService(FiltersView view)
        {
            _filtersView = view;
            Debug.Log("Filters service ready");
            
            _filtersView.FilterButton.onClick.AddListener(InvokeFilterChanged);
            _filtersView.ClearButton.onClick.AddListener(InvokeFilterChanged);
        }

        public void InvokeFilterChanged()
        {
            OnFilterChanged?.Invoke(_filtersView.FilterItem);
        }
        
        public void OnDestroy()
        {
            _filtersView.FilterButton.onClick.RemoveListener(InvokeFilterChanged);
            _filtersView.ClearButton.onClick.RemoveListener(InvokeFilterChanged);
        }
    }
}