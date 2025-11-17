using UnityEngine;
using VContainer;

namespace Dev
{
    public class InputHandler : MonoBehaviour
    {
        private Facade _facade;

        [Inject]
        private void Constructor(Facade facade)
        {
            _facade = facade;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                _facade.ApplyParamsFilter();
            }
            
            if (Input.GetKeyDown(KeyCode.Z))
            {
                _facade.UndoShopState();
            }
        }
    }
}