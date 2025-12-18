using System;
using System.Linq;
using UnityEngine;

namespace Common.Enums
{
    public enum ItemTag
    {
        Категория = 0,
        
        Стратегия = 1,
        Семейная,
        Вечериночные,
        Хардкорная,
        Настольная_Ролевая_Игра,
        RPG = 6,
        
        
        Ужас_Аркхема,
        Живая_Карточная_Игра,
        Игра_по_Вселенной,
        Ужас_Аркхема_Карточная_Игра = 10,
        
        
        Gloomhaven,
        Древний_Ужас,
        
        Dangeons_and_Dragons,
        Каркасон,
        Fallout,
        Игра_Престолов = 16,
        
        Манчкин,
        Ticket_to_Ride,
        Космический_контакт,
        Варгейм,
        Место_Преступления,
        Цитадели,
        Подарочное_Издание,
        Семь_чудес,
        Мачи_Коро,
        Бэнг = 26,
        
    }
    
    public static class StringToTag
    {
        public static ItemTag Convert(string str)
        {
            return (ItemTag)System.Enum.Parse(typeof(ItemTag), str);
        }
        
        public static ItemTag? FindClosestEnum(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return null;
            
            string normalizedSearch = searchString.ToLower().Trim();
        
            return Enum.GetValues(typeof(ItemTag))
                .Cast<ItemTag>()
                .Select(e => new 
                { 
                    Value = e, 
                    Distance = LevenshteinDistance(normalizedSearch, e.ToString().ToLower())
                })
                .OrderBy(x => x.Distance)
                .FirstOrDefault(x => x.Distance <= 5) // Пороговое значение
                ?.Value;
        }
    
        private static int LevenshteinDistance(string a, string b)
        {
            if (string.IsNullOrEmpty(a)) return b?.Length ?? 0;
            if (string.IsNullOrEmpty(b)) return a.Length;
        
            int[,] matrix = new int[a.Length + 1, b.Length + 1];
        
            for (int i = 0; i <= a.Length; i++) matrix[i, 0] = i;
            for (int j = 0; j <= b.Length; j++) matrix[0, j] = j;
        
            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = (a[i - 1] == b[j - 1]) ? 0 : 1;
                    matrix[i, j] = Mathf.Min(
                        Mathf.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost
                    );
                }
            }
        
            return matrix[a.Length, b.Length];
        }
    }
}