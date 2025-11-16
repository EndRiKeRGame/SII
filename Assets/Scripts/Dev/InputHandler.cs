using Shop;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Dev
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField]
        private Button _filterEnterButton;

        private ShopInitializer _shopService;

        [Inject]
        private void Constructor(ShopInitializer shopService)
        {
            _shopService = shopService;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                _filterEnterButton.onClick.Invoke();
            }
            
            if (Input.GetKeyDown(KeyCode.Z))
            {
                _shopService.UndoMove();
            }
        }
    }
}