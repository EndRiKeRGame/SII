using System;
using System.Collections.Generic;
using Common;
using Shop.Configs;
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
        private readonly DebugConsoleView _console;
        
        [Inject]
        public LikesService(
            LikesStorage likesStorage,
            DebugConsoleView console)
        {
            _likesStorage = likesStorage;
            _console = console;
        }

        public void InvokeLikesChanged()
        {
            OnLikesChanged?.Invoke();
        }
        
        public void InvokeDislikesChanged()
        {
            OnDislikesChanged?.Invoke();
        }

        public bool TryAddLike(Item item)
        {
            if (ContainsInLikesDislikes(item))
            {
                if (IsDislike(item))
                    TryRemoveDislike(item);
                else
                    TryRemoveLike(item);
                
                _console.LogMessage($"Item already in Likes or Dislikes! {item.Name}");
                return false;
            }

            return _likesStorage.Likes.Add(item);
        }
        
        public bool TryRemoveLike(Item item)
        {
            if (!IsLike(item))
            {
                _console.LogMessage($"Item not in Likes! {item.Name}");
                return false;
            }
            
            return _likesStorage.Likes.Remove(item);
        }
        
        public bool TryAddDislike(Item item)
        {
            if (ContainsInLikesDislikes(item))
            {
                if (IsDislike(item))
                    TryRemoveDislike(item);
                else
                    TryRemoveLike(item);
                
                _console.LogMessage($"Item already in Likes or Dislikes! {item.Name}");
                return false;
            }
            
            return _likesStorage.Dislikes.Add(item);
        }
        
        public bool TryRemoveDislike(Item item)
        {
            if (!IsDislike(item))
            {
                _console.LogMessage($"Item not in Dislikes! {item.Name}");
                return false;
            }
            
            return _likesStorage.Dislikes.Remove(item);
        }

        private bool ContainsInLikesDislikes(Item item)
        {
            return IsLike(item) || IsDislike(item);
        }
        
        public bool IsLike(Item item)
        {
            return _likesStorage.Likes.Contains(item);
        }
        
        public bool IsDislike(Item item)
        {
            return _likesStorage.Dislikes.Contains(item);
        }
    }
}