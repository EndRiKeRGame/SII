using System;
using System.Text.RegularExpressions;
using ChatBot.Enums;
using TriInspector;
using UnityEngine;

namespace ChatBot.Configs
{
    [CreateAssetMenu(fileName = "TimeVarConfig", menuName = "Game/TimeVarConfig", order = 0)]
    public class TimeVarConfig : ScriptableObject
    {
        [SerializeField]
        private TimeVarHolder[] _timeVarDictionary;

        public bool GetValue(string timeName, out int timeValue)
        {
            timeValue = -1;
            timeName = timeName.Trim();
            
            foreach (var time in _timeVarDictionary)
            {
                if (!timeName.Contains(time.Name))
                    continue;

                timeValue = time.Value;
                
                return true;
            }

            return false;
        }
        
        [Serializable]
        public class TimeVarHolder
        {
            public string Name;
            public int Value;
        }
    }
}