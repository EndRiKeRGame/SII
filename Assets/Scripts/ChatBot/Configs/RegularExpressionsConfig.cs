using System;
using System.Security;
using System.Text.RegularExpressions;
using ChatBot.Enums;
using UnityEngine;

namespace ChatBot.Configs
{
    [CreateAssetMenu(fileName = "RegularExpressionsConfig", menuName = "Game/RegularExpressionsConfig", order = 0)]
    public class RegularExpressionsConfig : ScriptableObject
    {
        [SerializeField]
        private PatternHolder[] _stringsForRecommendations = new PatternHolder[1];
        
        [SerializeField]
        private PatternHolder[] _stringsForSearch = new PatternHolder[1];
        
        [SerializeField]
        private PatternHolder[] _stringsForHistory = new PatternHolder[1];
        
        [SerializeField]
        private PatternHolder[] _stringsForGeneral = new PatternHolder[1];

        [HideInInspector]
        [SerializeField]
        private RegexHolder[] _regexForRecommendations;
        
        [HideInInspector]
        [SerializeField]
        private RegexHolder[] _regexForSearch;
        
        [HideInInspector]
        [SerializeField]
        private RegexHolder[] _regexForHistory;
        
        [HideInInspector]
        [SerializeField]
        private RegexHolder[] _regexForGeneral;

        private void OnValidate()
        {
            _regexForRecommendations = CreateRegexs(_stringsForRecommendations);
            _regexForSearch = CreateRegexs(_stringsForSearch);
            _regexForHistory = CreateRegexs(_stringsForHistory);
            _regexForGeneral = CreateRegexs(_stringsForGeneral);
            
            Debug.Log("Save complete!");
        }
        
        private RegexHolder[] CreateRegexs(PatternHolder[] examples)
        {
            RegexHolder[] regexs = new RegexHolder[examples.Length];
            for (int i = 0; i < examples.Length; i++)
            {
                regexs[i] = new RegexHolder
                {
                    GroupName = examples[i].GroupName,
                    Regexs = new Regex[examples[i].Patterns.Length]
                };

                for (var j = 0; j < examples[i].Patterns.Length; j++)
                    regexs[i].Regexs[j] = new Regex(examples[i].Patterns[j], RegexOptions.IgnoreCase);
            }
            return regexs;
        }

        public RegexHolder[] GetRegex(RequestTypes type)
        {
            switch (type)
            {
                case RequestTypes.Recommendation:
                    return _regexForRecommendations;
                case RequestTypes.Search:
                    return _regexForSearch;
                case RequestTypes.History:
                    return _regexForHistory;
                case RequestTypes.General:
                    return _regexForGeneral;
            }
            
            return null;
        }

        [Serializable]
        public class PatternHolder
        {
            public string GroupName;
            public string[] Patterns;
        }
        
        [Serializable]
        public class RegexHolder
        {
            public string GroupName;
            public Regex[] Regexs;
        }
    }
}