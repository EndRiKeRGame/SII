using System;
using System.Collections.Generic;
using Common;
using History;
using Shop.Configs;
using UnityEngine;
using VContainer;

namespace Shop
{
    public class LikesService
    {
        public event Action OnLikesChanged; 
        public event Action OnDislikesChanged; 
        
        public HashSet<Item> Likes => _likesStorage.Likes;
        public HashSet<Item> Dislikes => _likesStorage.Dislikes;
        
        private readonly LikesStorage _likesStorage;
        
        [Inject]
        public LikesService(LikesStorage likesStorage)
        {
            _likesStorage = likesStorage;
        }

        public bool TryAddLike(Item item)
        {
            if (ContainsInLikesDislikes(item))
            {
                if (IsDislike(item))
                    TryRemoveDislike(item);
                else
                    TryRemoveLike(item);
                
                Debug.Log($"Item already in Likes or Dislikes! {item.Name}");
                return false;
            }

            bool add = _likesStorage.Likes.Add(item);
            OnLikesChanged?.Invoke();
            
            return add;
        }
        
        public bool TryRemoveLike(Item item)
        {
            if (!IsLike(item))
            {
                Debug.Log($"Item not in Likes! {item.Name}");
                return false;
            }
            
            bool remove = _likesStorage.Likes.Remove(item);
            OnLikesChanged?.Invoke();
            
            return remove;
        }
        
        public bool TryAddDislike(Item item)
        {
            if (ContainsInLikesDislikes(item))
            {
                if (IsDislike(item))
                    TryRemoveDislike(item);
                else
                    TryRemoveLike(item);
                
                Debug.Log($"Item already in Likes or Dislikes! {item.Name}");
                return false;
            }
            
            bool add = _likesStorage.Dislikes.Add(item);
            OnDislikesChanged?.Invoke();
            
            return add;
        }
        
        public bool TryRemoveDislike(Item item)
        {
            if (!IsDislike(item))
            {
                Debug.Log($"Item not in Dislikes! {item.Name}");
                return false;
            }
            
            bool remove = _likesStorage.Dislikes.Remove(item);
            OnDislikesChanged?.Invoke();
            
            return remove;
        }

        private bool ContainsInLikesDislikes(Item item)
        {
            return IsLike(item) || IsDislike(item);
        }
        
        private bool IsLike(Item item)
        {
            return _likesStorage.Likes.Contains(item);
        }
        
        private bool IsDislike(Item item)
        {
            return _likesStorage.Dislikes.Contains(item);
        }
    }
}