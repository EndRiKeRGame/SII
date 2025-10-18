using Common;
using Common.Configs;
using TriInspector;
using UnityEngine;
using VContainer;

namespace Shop
{
    public class ShopInitializer : MonoBehaviour
    {
        [SerializeField]
        private ItemCardGridView _itemCardGridView;
        
        private ItemsConfig _itemsConfig;

        [Inject]
        public void Construct(ItemsConfig itemsConfig)
        {
            _itemsConfig = itemsConfig;
            ShowItems();
        }

        [Button]
        private void ShowItems()
        {
            Item[] items = _itemsConfig.GetAllItems();
            _itemCardGridView.SetupItems(items);
        }
    }
}