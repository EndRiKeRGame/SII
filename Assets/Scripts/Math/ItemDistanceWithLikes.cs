using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Configs;
using Shop;
using TriInspector;
using UnityEngine;
using VContainer;

namespace Math
{
    public class ItemDistanceWithLikes : MonoBehaviour
    {
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
        }

        [Button]
        public Dictionary<Item, float> CalculateProximityForAllOnLikes()
        {
            Dictionary<Item, float> proximityOnLikes = new Dictionary<Item, float>();
            
            var likes = _likeService.Likes;
            var dislikes = _likeService.Dislikes;
            List<Item> allItems = _itemsConfig.GetAllItems().ToList();
            
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
            
            for (int i = 0; i < allItems.Count; i++)
            {
                proximityOnLikes.Add(allItems[i], TotalProximityForLikes(likesProximity[i], dislikesProximity[i]));
            }
            
            return proximityOnLikes;
        }

        private float TotalProximityForLikes(float[] likeProximity, float[] dislikeProximity)
        {
            float likesSum = likeProximity.Sum();
            float dislikesSum = 100f * dislikeProximity.Length - dislikeProximity.Sum();
            return (likesSum + dislikesSum) / (likeProximity.Length + dislikeProximity.Length);
        }
    }
}
