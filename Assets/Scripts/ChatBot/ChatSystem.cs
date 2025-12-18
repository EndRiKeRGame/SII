using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ChatBot.Configs;
using ChatBot.Enums;
using Common.Configs;
using Common.Enums;
using Dev;
using NUnit.Framework;
using UnityEngine;
using VContainer;

namespace ChatBot
{
    public class ChatSystem : MonoBehaviour
    {
        private RegularExpressionsConfig _regexConfig;
        private Facade _facade;
        private ChatBotView _chatBot;
        private ItemsConfig _items;
        private TimeVarConfig _timeVarConfig;

        [Inject]
        private void Construct(
            RegularExpressionsConfig regexConfig,
            Facade facade,
            ChatBotView chatBot,
            ItemsConfig items,
            TimeVarConfig timeVarConfig)
        {
            _facade = facade;
            _regexConfig = regexConfig;
            _chatBot = chatBot;
            _items = items;
            _timeVarConfig = timeVarConfig;
            
            _regexConfig.Save();
        }

        private void Start()
        {
            _chatBot.AddAction(ProcessRequest);
        }

        private void ProcessRequest()
        {
            var request = _chatBot.GetRequest();
            _chatBot.WriteLine("\n\n" + ResponseForeRequest(request) + "\n");
        }

        private string ResponseForeRequest(string request)
        {
            try
            {
                request = request.Trim();
                
                // 3. Комбинированные запросы
                if (TryMatchCombined(request, out string combinedResponse))
                    return combinedResponse;
            
                // 1. Рекомендательная система
                if (TryMatchRecommendation(request, out string recResponse))
                    return recResponse;
        
                // 2. Поиск с фильтрами
                if (TryMatchSearch(request, out string searchResponse))
                    return searchResponse;
        
                // 4. Управление историей
                if (TryMatchHistory(request, out string historyResponse))
                    return historyResponse;
        
                // 5. Общие вопросы
                if (TryMatchGeneral(request, out string generalResponse))
                    return generalResponse;
            
                return "Я не могу ответить на ваш вопрос. Попробуйте перефразировать.";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private bool TryMatchRecommendation(string request, out string response)
        {
            response = "";
            foreach (var patternHolder in _regexConfig.GetRegex(RequestTypes.Recommendation))
            {
                foreach (var regex in patternHolder.Regexs)
                {
                    Match match = regex.Match(request);
                    if (match.Success)
                    {
                        string gameName = "";
                        
                        switch (patternHolder.GroupName)
                        {
                            case "like_game":
                                gameName = match.Groups[1].Value;
                                
                                AddToLike(gameName);
                                response = $"Добавил {gameName} в понравившиеся!";
                                return true;
                            
                            case "dislike_game":
                                gameName = match.Groups[1].Value;
                                
                                AddToDislike(gameName);
                                response = $"Добавил {gameName} в не понравившиеся!";
                                return true;
                            
                            case "show_recommendations":
                                _facade.ShowFirstX();
                                
                                response = $"Вот 3 настолки, которые должны тебе понравится!";;
                                return true;
                            
                            case "already_buy":
                                gameName = match.Groups[1].Value;
                                
                                if (!_items.TryGetItemByName($"{gameName}", out var buy))
                                    throw new Exception($"Не нашел игры с названием {gameName} =(");
                                
                                _facade.RemoveLikeDislike(buy);
                                _facade.ApplyLikesFilter();
                                
                                response = $"Круто! Убрал из лайков и дизлайков";
                                return true;
                        }
                    }
                }
                
            }

            return false;
        }
        
        private bool TryMatchSearch(string request, out string response)
        {
            response = "";
            var filter = _facade.GetFilterItem();
            foreach (var patternHolder in _regexConfig.GetRegex(RequestTypes.Search))
            {
                Debug.Log($"{patternHolder.GroupName}");
                
                foreach (var regex in patternHolder.Regexs)
                {
                    Match match = regex.Match(request);
                    if (match.Success)
                    {
                        string gameName = "";
                        
                        switch (patternHolder.GroupName)
                        {
                            case "price_filter":
                                var priceString = match.Groups[1].Value;
                                int price = Convert.ToInt32(priceString);
                                filter.PriceTo = price;
                                _facade.ApplyParamsFilterWithParams(filter);
                                
                                response = $"Применил фильтр по цене: все игры дешевле {price}!";
                                return true;
                            
                            case "num_of_players_filter":
                                int playersNum = -1;

                                try
                                {
                                    if (!string.IsNullOrEmpty(match.Groups[1].Value))
                                    {
                                        playersNum = Convert.ToInt32(match.Groups[1].Value.Trim('?'));
                                    }
                                }
                                catch (Exception e) { }
                                
                                try
                                {
                                    if (!string.IsNullOrEmpty(match.Groups[1].Value))
                                    {
                                        string word = match.Groups[1].Value.ToLower().Trim('?');
                                        playersNum = word switch
                                        {
                                            "двоих" or "двух" or "два" => 2,
                                            "троих" or "трех" or "три" => 3,
                                            "четверых" or "четырех" or "четыре" => 4,
                                            "пятерых" or "пяти" or "пять" => 5,
                                            _ => throw new ArgumentException($"Неизвестное количество игроков: {word}")
                                        };
                                    }
                                }
                                catch (Exception e) { }
                                
                                if (playersNum == -1)
                                    return false;
    
                                filter.NumOfPlayersFrom = playersNum;
                                _facade.ApplyParamsFilterWithParams(filter);
                                response = $"Применил фильтр по количеству игроков: все игры от {playersNum} человек!";
                                return true;
                            
                            case "name_filter":
                                gameName = match.Groups[1].Value;
                                
                                if (!_items.TryGetItemByNameFuzzy($"{gameName}", out var item, maxDistance: 5))
                                    throw new Exception($"Не нашел игры с названием {gameName} =(");
                                
                                filter.Name = item.Name;
                                _facade.ApplyParamsFilterWithParams(filter);
                                
                                response = $"Применил фильтр по названию: все игры с названием {item.Name}!";
                                return true;
                            
                            case "tags_filter":
                                var tagString = match.Groups[1].Value;
                                var itemTag = StringToTag.FindClosestEnum(tagString);
                                filter.Tags.Add(itemTag.Value);
                                _facade.ApplyParamsFilterWithParams(filter);
                                
                                response = $"Применил фильтр по тегам: все игры с тегом {itemTag.Value}!";
                                return true;
                            
                            case "time_filter":
                                var timeString = match.Groups[1].Value;
                                if (!_timeVarConfig.GetValue(timeString, out int time))
                                    throw new Exception($"Не нашел значения для лингвистической переменной {timeString} =(");
                                filter.AvgPlayTimeTo = time;
                                _facade.ApplyParamsFilterWithParams(filter);
                                
                                response = $"Применил фильтр по времени: все игры со временем {time}!";
                                return true;
                        }
                    }
                }
                
            }

            return false;
        }
        
        private bool TryMatchCombined(string request, out string response)
        {
            response = "";

            var filter = _facade.GetFilterItem();
            foreach (var patternHolder in _regexConfig.GetRegex(RequestTypes.Combo))
            {
                foreach (var regex in patternHolder.Regexs)
                {
                    Match match = regex.Match(request);
                    if (match.Success)
                    {
                        string gameName = "";
                        
                        switch (patternHolder.GroupName)
                        {
                            case "name_price":
                                gameName = match.Groups[1].Value;
                                var priceString = match.Groups[2].Value;
                                int price = int.Parse(priceString);
                                filter.Name = gameName;
                                filter.PriceTo = price;
                                _facade.ApplyParamsFilterWithParams(filter);
                                response = $"Применил фильтр по названию и цене: {gameName} дешевле {price}!";
                                return true;
                            
                            case "like_num_of_players":
                                int playersNum = -1;

                                try
                                {
                                    if (!string.IsNullOrEmpty(match.Groups[2].Value))
                                    {
                                        playersNum = Convert.ToInt32(match.Groups[2].Value.Trim('?'));
                                    }
                                }
                                catch (Exception e) { }
                                
                                try
                                {
                                    if (!string.IsNullOrEmpty(match.Groups[2].Value))
                                    {
                                        string word = match.Groups[2].Value.ToLower().Trim('?');
                                        playersNum = word switch
                                        {
                                            "двоих" or "двух" or "два" => 2,
                                            "троих" or "трех" or "три" => 3,
                                            "четверых" or "четырех" or "четыре" => 4,
                                            "пятерых" or "пяти" or "пять" => 5,
                                            _ => throw new ArgumentException($"Неизвестное количество игроков: {word}")
                                        };
                                    }
                                }
                                catch (Exception e) { }
                                
                                if (playersNum == -1)
                                    return false;
    
                                filter.NumOfPlayersFrom = playersNum;
                                
                                gameName = match.Groups[1].Value;
                                filter.NumOfPlayersFrom = playersNum;
                                _facade.ApplyParamsFilterWithParams(filter);
                                
                                AddToLike(gameName);
                                response = $"Я тебя услышал: поставил лайк {gameName} и добавил фильтр на количество игроков!";
                                return true;
                            
                            case "name_tag":
                                gameName = match.Groups[1].Value;
                                var tagString = match.Groups[2].Value.ToLower().Replace(" ", "_");
                                if (!StringToTag.FindClosestEnum(tagString).HasValue)
                                    throw new Exception($"Не нашел тег {tagString}");
                                    
                                filter.Tags.Add(StringToTag.FindClosestEnum(tagString).Value);
                                _facade.ApplyParamsFilterWithParams(filter);
                                
                                AddToDislike(gameName);
                                response = $"Хорошо, я поставил дизлайк на {gameName}. Выделил для тебя аналогичные по тегу.";
                                return true;
                            
                            case "likeTag":
                                var likeTagString = match.Groups[1].Value.ToLower().Replace(" ", "_");
                                ItemTag likeTag = StringToTag.FindClosestEnum(likeTagString).Value;
                                var items = _items.GetItemsByTags(new List<ItemTag> { likeTag });

                                foreach (var item in items)
                                {
                                    _facade.AddToLike(item);
                                }
                                
                                response = $"Хорошо, я поставил лайк на все игры жанра {likeTagString}.";
                                return true;
                            
                            case "dislikeTag":
                                var dislikeTagString = match.Groups[1].Value.ToLower().Replace(" ", "_");
                                ItemTag dislikeTag = StringToTag.FindClosestEnum(dislikeTagString).Value;
                                var disItems = _items.GetItemsByTags(new List<ItemTag> { dislikeTag });

                                foreach (var item in disItems)
                                {
                                    _facade.AddToLike(item);
                                }
                                
                                response = $"Хорошо, я поставил дизлайк на все игры жанра {dislikeTagString}.";
                                return true;
                        }
                    }
                }
                
            }

            return false;
        }
        
        private bool TryMatchHistory(string request, out string response)
        {
            response = "";

            foreach (var patternHolder in _regexConfig.GetRegex(RequestTypes.History))
            foreach (var regex in patternHolder.Regexs)
            {
                Match match = regex.Match(request);
                if (match.Success)
                {
                    string gameName = "";
                    
                    switch (patternHolder.GroupName)
                    {
                        case "undo":
                            _facade.UndoShopState();
                            response = $"Откатил магазин на один шаг назад!";
                            return true;
                        case "undo_all":
                            _facade.UndoAllSteps();
                            response = $"Откатил все твои действия!";
                            return true;
                    }
                }
            }

            return false;
        }
        
        private bool TryMatchGeneral(string request, out string response)
        {
            response = "";
            
            foreach (var patternHolder in _regexConfig.GetRegex(RequestTypes.General))
            foreach (var regex in patternHolder.Regexs)
            {
                Match match = regex.Match(request);
                if (match.Success)
                {
                    string gameName = "";
                    
                    switch (patternHolder.GroupName)
                    {
                        case "tags_ask":
                            response = $"Хороший вопрос! Жанры бывают самые раззнообразные, вот тебе список тех, которые есть сейчас в магазине:\n";
                            for (int i = 0; i <= 26; i++)
                                response += $"{(ItemTag)i}\n";
                            return true;
                        
                        case "help":
                            response = $"Привет! Меня зовут Хоббит! Я - твой чат-бот, который поможет тебе подобрать настолку. Напиши, во что ты уже играл или что тебе нравится и я посмотрю, что могу тебе предложить =)";
                            return true;
                        
                        case "top3":
                            response += $"Дай-ка гляну в интернет...\nПо оценкам пользователей сайта Hobby Games, народный топ-3 выглядит так:\n";
                            response += $"Каркасон\n";
                            response += $"Мачи Коро\n";
                            response += $"Взрывные котята\n";
                            return true;
                        
                        case "top3_likes":
                            _facade.ShowFirstX();
                            response = $"А вот и твой личный топ настолок!";
                            return true;
                        
                        case "describe_filters":
                            response += $"Тут все просто! Название и стоимость и так ясна. Время в игре указана для одной средней партии, но учти, что первая партия всегда будет дольше указанного времени!";
                            response += $"Количество игроков означает, сколько друзей нужно позвать на вечеринку, чтобы поиграть! В разделе жанры ты можешь указать жанр игры, которую ищещь";
                            return true;
                    }
                }
            }
            
            return false;
        }

        private void AddToLike(string gameName)
        {
            if (_items.TryGetItemByName($"{gameName}", out var like))
            {
                _facade.AddToLike(like);
                _facade.ApplyLikesFilter();
            }
            else if (_items.TryGetItemByNameFuzzy($"{gameName}", out var like2))
            {
                _facade.AddToLike(like2);
                _facade.ApplyLikesFilter();
            }
            else
            {
                throw new Exception($"Не нашел игры с названием {gameName} =(");
            }
        }
        
        private void AddToDislike(string gameName)
        {
            if (_items.TryGetItemByName($"{gameName}", out var like))
            {
                _facade.AddToDislike(like);
                _facade.ApplyLikesFilter();
            }
            else if (_items.TryGetItemByNameFuzzy($"{gameName}", out var like2))
            {
                _facade.AddToDislike(like2);
                _facade.ApplyLikesFilter();
            }
            else
            {
                throw new Exception($"Не нашел игры с названием {gameName} =(");
            }
        }
    }
}