using Common.Configs;
using Dev;
using History;
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
        
        private LikesService _likesService;
        private ItemDistanceWithLikes _itemDistanceWithLikes;
        private FiltersService _filtersService;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_itemsConfig);
            builder.RegisterInstance(_tagsTree);
            builder.RegisterInstance(_likesStorage);
            
            builder.RegisterComponentInHierarchy<LikesTabService>();
            builder.RegisterComponentInHierarchy<AdvancedHeatmapView>();
            builder.RegisterComponentInHierarchy<FiltersView>();
            builder.RegisterComponentInHierarchy<ItemCardGridView>();
            builder.RegisterComponentInHierarchy<ItemDistance>();
            builder.RegisterComponentInHierarchy<ItemDistanceWithLikes>();
            
            builder.Register<LikesService>(Lifetime.Singleton);
            builder.Register<FiltersService>(Lifetime.Singleton);
            builder.Register<HistoryService>(Lifetime.Singleton);
            
            builder.Register<Facade>(Lifetime.Singleton);
            
            builder.RegisterComponentInHierarchy<ShopInitializer>();
            builder.RegisterComponentInHierarchy<InputHandler>();
        }
    }
}