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

        public static float ZoomAction(this InputManager manager)
        {
            if (Touch.activeTouches.Count < 2)
            {
                return 0; //single input , ignore
            }

            Touch touch0 = Touch.activeTouches[0];
            Touch touch1 = Touch.activeTouches[1];

            if (touch0.phase != TouchPhase.Moved && touch1.phase != TouchPhase.Moved)
            {
                return 0;
            }

            if (touch0.history.Count < 1 || touch1.history.Count < 1)
            {
                return 0;
            }

            return Mathf.Clamp(Vector2.Distance(touch0.screenPosition, touch1.screenPosition) - Vector2.Distance(touch0.history[0].screenPosition, touch1.history[0].screenPosition), -1, 1);
        }
    }
}
