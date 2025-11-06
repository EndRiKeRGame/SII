using UnityEngine;
using UnityEngine.UI;

namespace Dev
{
    public class ButtonEnterHandler : MonoBehaviour
    {
        public Button yourButton;
    
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                yourButton.onClick.Invoke();
            }
        }
    }
}