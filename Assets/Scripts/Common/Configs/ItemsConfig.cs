using System;
using System.Collections.Generic;
using System.Linq;
using Common.Enums;
using TriInspector;
using UnityEngine;

namespace Common.Configs
{
    [CreateAssetMenu(fileName = nameof(ItemsConfig), menuName = "Configs/" + nameof(ItemsConfig))]
    public class ItemsConfig : ScriptableObject
    {
        [SerializeField]
        private Item[] _items;
        
        [field: SerializeField, ReadOnly]
        public Item MaxItem { get; set; }
        
        [field: SerializeField, ReadOnly]
        public Item MinItem { get; set; }
        
        [field: SerializeField, ReadOnly]
        public Item AvgItem { get; set; }

        [Button]
        public void UpdateMaxMinAvr()
        {
            MaxItem = new Item();
            MinItem = new Item();
            AvgItem = new Item();
            
            foreach (var item in _items)
            {
                if (item.Price > MaxItem.Price)
                    MaxItem.Price = item.Price;
                if (item.Price < MinItem.Price || MinItem.Price == -1)
                    MinItem.Price = item.Price;
                
                AvgItem.Price += item.Price;
                
                if (item.MaxNumOfPlayers > MaxItem.MaxNumOfPlayers || MaxItem.MaxNumOfPlayers == Int32.MaxValue)
                    MaxItem.MaxNumOfPlayers = item.MaxNumOfPlayers;
                if (item.MinNumOfPlayers < MinItem.MinNumOfPlayers || MinItem.MinNumOfPlayers == 0)
                    MinItem.MinNumOfPlayers = item.MinNumOfPlayers;
                
                AvgItem.MaxNumOfPlayers += item.MaxNumOfPlayers;
                AvgItem.MinNumOfPlayers += item.MinNumOfPlayers;
                
                if (item.AvgPlayTime > MaxItem.AvgPlayTime)
                    MaxItem.AvgPlayTime = item.AvgPlayTime;
                if (item.AvgPlayTime < MinItem.AvgPlayTime || MinItem.AvgPlayTime == -1)
                    MinItem.AvgPlayTime = item.AvgPlayTime;
                
                AvgItem.AvgPlayTime += item.AvgPlayTime;
                
                if (item.MinimumAge > MaxItem.MinimumAge)
                    MaxItem.MinimumAge = item.MinimumAge;
                if (item.MinimumAge < MinItem.MinimumAge || MinItem.MinimumAge == -1)
                    MinItem.MinimumAge = item.MinimumAge;
                
                AvgItem.MinimumAge += item.MinimumAge;
            }
            
            var length = _items.Length;
            AvgItem.Price /= length;
            AvgItem.AvgPlayTime /= length;
            AvgItem.MinimumAge /= length;
            AvgItem.MaxNumOfPlayers /= length;
            AvgItem.MinNumOfPlayers /= length;
        }

        public Item[] GetAllItems() => _items;

        public Item[] GetItemsByPrice(int price, Item[] selectedItems = null)
        {
            selectedItems ??= _items;
            return selectedItems.Where(x => x.Price <= price).ToArray();
        }
        
        public Item[] GetItemsByTags(List<ItemTag> tags, Item[] selectedItems = null)
        {
            selectedItems ??= _items;
            return selectedItems.Where(x => x.Tags.Any(y => tags.Contains(y))).ToArray();
        }

        public bool TryGetItemByName(string name, out Item neededItem)
        {
            foreach (var item in _items)
            {
                if (!string.Equals(item.Name, name, StringComparison.CurrentCultureIgnoreCase))
                    continue;
                
                neededItem = item;
                return true;
            }

            neededItem = null;
            return false;
        }
        
        public bool TryGetItemByName(string name, out Item neededItem, int maxDistance = 3)
        {
            if (string.IsNullOrWhiteSpace(name) || _items == null || _items.Length == 0)
            {
                neededItem = null;
                return false;
            }

            string normalizedName = name.Trim().ToLowerInvariant();
            
            int bestDistance = int.MaxValue;
            Item bestItem = null;
            
            foreach (var item in _items)
            {
                if (string.IsNullOrWhiteSpace(item.Name))
                    continue;
                
                string itemName = item.Name.Trim().ToLowerInvariant();
                
                // Если точное совпадение - сразу возвращаем
                if (string.Equals(itemName, normalizedName, StringComparison.OrdinalIgnoreCase))
                {
                    neededItem = item;
                    return true;
                }
                
                // Вычисляем расстояние Левенштейна
                int distance = LevenshteinDistance(normalizedName, itemName);
                
                // Если нашли лучшее совпадение
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestItem = item;
                }
            }
            
            // Проверяем, находится ли лучшее совпадение в допустимых пределах
            if (bestItem != null && bestDistance <= maxDistance)
            {
                neededItem = bestItem;
                return true;
            }
            
            neededItem = null;
            return false;
        }

        private static int LevenshteinDistance(string a, string b)
        {
            if (string.IsNullOrEmpty(a))
                return string.IsNullOrEmpty(b) ? 0 : b.Length;
            if (string.IsNullOrEmpty(b))
                return a.Length;
            
            int[,] matrix = new int[a.Length + 1, b.Length + 1];
            
            // Инициализация первой строки и столбца
            for (int i = 0; i <= a.Length; i++) matrix[i, 0] = i;
            for (int j = 0; j <= b.Length; j++) matrix[0, j] = j;
            
            // Вычисление расстояния
            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = (a[i - 1] == b[j - 1]) ? 0 : 1;
                    matrix[i, j] = Mathf.Min(
                        Mathf.Min(
                            matrix[i - 1, j] + 1,      // Удаление
                            matrix[i, j - 1] + 1       // Вставка
                        ),
                        matrix[i - 1, j - 1] + cost    // Замена
                    );
                }
            }
            
            return matrix[a.Length, b.Length];
        }

        // Дополнительный метод с возможностью поиска по части имени
        public bool TryGetItemByNameFuzzy(string name, out Item neededItem, int maxDistance = 3)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                neededItem = null;
                return false;
            }
            
            string normalizedName = name.Trim().ToLowerInvariant();
            
            // Сначала ищем полное совпадение
            if (TryGetItemByName(name, out neededItem, maxDistance))
                return true;
            
            // Если не нашли, ищем по части имени (если имя длинное)
            if (normalizedName.Length >= 3)
            {
                var candidates = _items
                    .Where(item => !string.IsNullOrWhiteSpace(item.Name))
                    .Select(item => new 
                    { 
                        Item = item, 
                        Name = item.Name.Trim().ToLowerInvariant(),
                        Distance = LevenshteinDistance(normalizedName, item.Name.Trim().ToLowerInvariant())
                    })
                    .Where(x => x.Name.Contains(normalizedName) || normalizedName.Contains(x.Name))
                    .OrderBy(x => x.Distance)
                    .ToList();
                
                if (candidates.Any())
                {
                    var bestCandidate = candidates.First();
                    if (bestCandidate.Distance <= maxDistance * 2) // Более мягкий порог для частичного совпадения
                    {
                        neededItem = bestCandidate.Item;
                        return true;
                    }
                }
            }
            
            neededItem = null;
            return false;
        }
    }
}