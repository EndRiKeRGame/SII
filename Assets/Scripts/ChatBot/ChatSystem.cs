using System;
using System.Text.RegularExpressions;
using ChatBot.Configs;
using ChatBot.Enums;
using Common.Configs;
using Dev;
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

        [Inject]
        private void Construct(
            RegularExpressionsConfig regexConfig,
            Facade facade,
            ChatBotView chatBot,
            ItemsConfig items)
        {
            _facade = facade;
            _regexConfig = regexConfig;
            _chatBot = chatBot;
            _items = items;
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
            
                // 1. Рекомендательная система
                if (TryMatchRecommendation(request, out string recResponse))
                    return recResponse;
        
                // 2. Поиск с фильтрами
                if (TryMatchSearch(request, out string searchResponse))
                    return searchResponse;
        
                // 3. Комбинированные запросы
                if (TryMatchCombined(request, out string combinedResponse))
                    return combinedResponse;
        
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
                                
                                if (!_items.TryGetItemByName($"{gameName}", out var like))
                                    throw new Exception($"Не нашел игры с названием {gameName} =(");
                                
                                _facade.AddToLike(like);
                                response = $"Добавил {gameName} в понравившиеся!";
                                return true;
                            
                            case "dislike_game":
                                gameName = match.Groups[1].Value;
                                
                                if (!_items.TryGetItemByName($"{gameName}", out var dislike))
                                    throw new Exception($"Не нашел игры с названием {gameName} =(");
                                
                                _facade.AddToDislike(dislike);
                                response = $"Добавил {gameName} в не понравившиеся!";
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


            return false;
        }
        
        private bool TryMatchCombined(string request, out string response)
        {
            response = "";


            return false;
        }
        
        private bool TryMatchHistory(string request, out string response)
        {
            response = "";


            return false;
        }
        
        private bool TryMatchGeneral(string request, out string response)
        {
            response = "";


            return false;
        }
    }
}