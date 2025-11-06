using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Common.Configs;
using Common.Enums;
using Math.Enums;
using MyNamespace;
using Shop;
using TriInspector;
using Ui;
using UnityEngine;
using VContainer;

namespace Math
{
    public class ItemDistanceWithLikes : MonoBehaviour
    {
        public Dictionary<Item, float> ProximityOnLikes { get; private set; } = null;
        
        private ItemsConfig _itemsConfig;
        private LikesService _likeService;
        private ItemDistance _baseCalculator;
        
        [Inject]
        public void Construct(
            ItemsConfig itemsConfig,
            LikesService likeService,
            ItemDistance baseCalculator)
        {
            _baseCalculator = baseCalculator;
            _itemsConfig = itemsConfig;
            _likeService = likeService;
            
            _likeService.OnLikesChanged += CalculateProximityForAllOnLikes;
            _likeService.OnDislikesChanged += CalculateProximityForAllOnLikes;
        }

        [Button]
        public void CalculateProximityForAllOnLikes()
        {
            List<Item> allItems = _itemsConfig.GetAllItems().ToList();
            HashSet<Item> likes = _likeService.Likes;
            HashSet<Item> dislikes = _likeService.Dislikes;
            foreach (var like in _likeService.Likes)
            {
                allItems.Remove(like);
            }
            
            foreach (var dislike in _likeService.Dislikes)
            {
                allItems.Remove(dislike);
            }

            List<float[]> likesProximity = new List<float[]>();
            List<float[]> dislikesProximity = new List<float[]>();
            
            foreach (var item in allItems)
            {
                likesProximity.Add(_baseCalculator.CompletedProximityForOne(item, likes.ToArray()));
                dislikesProximity.Add(_baseCalculator.CompletedProximityForOne(item, dislikes.ToArray()));
            }
            
            ProximityOnLikes?.Clear();
            ProximityOnLikes = new Dictionary<Item, float>();
            
            for (int i = 0; i < allItems.Count; i++)
            {
                ProximityOnLikes.Add(allItems[i], TotalProximityForLikes(likesProximity[i], dislikesProximity[i]));
            }
        }

        [Button]
        public float CalculateProximityForOneOnAllLikes(Item item)
        {
            if (ProximityOnLikes == null)
                CalculateProximityForAllOnLikes();

            if (!ProximityOnLikes.TryGetValue(item, out var proximity))
                throw new Exception("Item not found in last all proximity");

            return proximity;
        }

        private float TotalProximityForLikes(float[] likeProximity, float[] dislikeProximity)
        {
            float likesSum = likeProximity.Sum();
            float dislikesSum = 100f * dislikeProximity.Length - dislikeProximity.Sum();
            return (likesSum + dislikesSum) / (likeProximity.Length + dislikeProximity.Length);
        }
    }
}
