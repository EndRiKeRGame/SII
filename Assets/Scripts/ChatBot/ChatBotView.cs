using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ChatBot
{
    public class ChatBotView : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _dialogueInputField;
        
        [SerializeField]
        private TMP_Text _chatZone;

        [SerializeField]
        private Button _sendRequestButton;

        private void Awake()
        {
            _sendRequestButton.onClick.AddListener(SendRequest);
        }

        public void AddAction(UnityAction action)
        {
            _sendRequestButton.onClick.AddListener(action);
        }
        
        public void RemoveAction(UnityAction action)
        {
            _sendRequestButton.onClick.RemoveListener(action);
        }

        public string GetRequest() => _dialogueInputField.text;

        public void SendRequest()
        {
            _chatZone.text += "\n";
            _chatZone.text += "\n";
            _chatZone.text += "User:";
            _chatZone.text += GetRequest();
        }
        
        public void WriteLine(string line)
        {
            _chatZone.text += line;
        }
    }
}