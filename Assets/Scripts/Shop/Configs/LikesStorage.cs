using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Shop.Configs
{
    [CreateAssetMenu(fileName = nameof(LikesStorage), menuName = "Shop/" + nameof(LikesStorage), order = 0)]
    public class LikesStorage : ScriptableObject
    {
        [field: SerializeField]
        public HashSet<Item> Likes { get; set; } = new HashSet<Item>();

        [field: SerializeField]
        public HashSet<Item> Dislikes { get; set; } = new HashSet<Item>();
    }
}