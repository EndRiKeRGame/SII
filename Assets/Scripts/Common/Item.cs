using System;
using Common.Enums;
using UnityEngine;

namespace Common
{
    [Serializable]
    public class Item
    {
        public Guid ItemGuid = new Guid();
        
        public string Name = "Default Board Game";
        public int Price = -1;
        public ItemTag[] Tags;
        
        public Range NumOfPlayers = ..0;
        public int AvgPlayTime = -1;
        public int MinimumAge = -1;
        
        public Sprite Sprite;
    }
}