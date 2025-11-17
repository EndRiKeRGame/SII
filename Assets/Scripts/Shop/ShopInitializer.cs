using System.Collections.Generic;
using Common;
using Dev;
using UnityEngine;
using VContainer;

namespace Shop
{
    public class ShopInitializer : MonoBehaviour
    {
        [SerializeField]
        private ItemCardGridView _itemCardGridView;
        
        [SerializeField]
        private FiltersView _filtersView;

        private Facade _facade;

        [Inject]
        public void Construct(Facade facade)
        {
            _facade = facade;

            InitAllViews();
        }
        
        public void InitAllViews()
        {
            _itemCardGridView.SetupItems(_facade.GetItems(), OnLike, OnDislike);
            _filtersView.ClearFilters();
            _filtersView.AddButtons(OnFilterSubmit, OnFilterClear);
        }

        private void OnLike(Item item)
        {
            _facade.AddToLike(item);
            _facade.ApplyLikesFilter();
            _facade.SaveShopState();
        }
        
        private void OnDislike(Item item)
        {
            _facade.AddToDislike(item);
            _facade.ApplyLikesFilter();
            _facade.SaveShopState();
        }
        
        private void OnFilterSubmit()
        {
            _facade.ApplyParamsFilter();
            _facade.SaveShopState();
        }
        
        private void OnFilterClear()
        {
            _facade.ClearFilters();
            _facade.ApplyParamsFilter();
            _facade.SaveShopState();
        }
    }
}