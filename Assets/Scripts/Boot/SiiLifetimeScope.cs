using Common.Configs;
using Math;
using Shop;
using Shop.Configs;
using Ui;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Boot
{
    public class SiiLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private ItemsConfig _itemsConfig;

        [SerializeField]
        private MatrixTreeConfig _tagsTree;

        [SerializeField]
        private LikesStorage _likesStorage;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_itemsConfig);
            builder.RegisterInstance(_tagsTree);
            builder.RegisterInstance(_likesStorage);
            
            builder.RegisterComponentInHierarchy<DebugConsoleView>();
            builder.RegisterComponentInHierarchy<FiltersView>();
            
            builder.Register<LikesService>(Lifetime.Singleton);
            builder.Register<FiltersService>(Lifetime.Singleton);
            
            builder.RegisterComponentInHierarchy<LikesTabService>();
            builder.RegisterComponentInHierarchy<AdvancedHeatmap>();
            builder.RegisterComponentInHierarchy<ShopInitializer>();
            builder.RegisterComponentInHierarchy<ItemDistance>();
            builder.RegisterComponentInHierarchy<ItemDistanceWithLikes>();
            builder.RegisterComponentInHierarchy<ItemCardGridView>();
        }
    }
}