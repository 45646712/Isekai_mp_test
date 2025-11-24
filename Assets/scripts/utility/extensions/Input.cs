using System.Linq;
using Constant;
using UnityEngine;
using UnityEngine.InputSystem;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Extensions
{
    public static class Input
    {
        public static void GenerateRay(this InputManager manager, InputAction.CallbackContext context)
        {
            if (!manager.playerInput.UI.Click.WasReleasedThisFrame())
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Pointer.current.position.ReadValue());
            
            Physics.Raycast(ray, out var hit, 20, LayerMask.GetMask("WorldUI"));

            foreach (var (key, value) in UIManager.Instance.AllActiveUIs)
            {
                if (key == UIConstants.NonPooledUITypes.ControlOverlay)
                {
                    continue;
                }

                return;
            }
            
            if (hit.collider == null)
            {
                return;
            }
            
            hit.collider.gameObject.GetComponent<IWorldUIObject>().InvokeRaycastEvt();
        }
    }
}
