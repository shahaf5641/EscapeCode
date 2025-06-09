using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Christina.CustomCursor
{
    public class CursorController : MonoBehaviour
    {
        [SerializeField] private Texture2D cursorTextureDefault;
        [SerializeField] private Texture2D cursorTextureClickable;
        [SerializeField] private Vector2 clickPosition = Vector2.zero;
        [SerializeField] private LayerMask clickableLayers;
        private Texture2D currentCursor;
        void Update()
        {
            // ✅ Always allow hover for UI buttons
            if (IsPointerOverClickableUI())
            {
                SetCursor(cursorTextureClickable);
                return;
            }

            // ❌ Block world hover if CodeWindow is open
            if (CodeWindowManager.IsOpen)
            {
                SetCursor(cursorTextureDefault);
                return;
            }

            // ✅ Allow 3D object hover only when CodeWindow is closed
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, clickableLayers))
            {
                if (hit.collider.CompareTag("WorldClickable"))
                {
                    SetCursor(cursorTextureClickable);
                    return;
                }
            }

            SetCursor(cursorTextureDefault);
        }

        private void SetCursor(Texture2D texture)
        {
            if (currentCursor == texture) return;
            Cursor.SetCursor(texture, clickPosition, CursorMode.Auto);
            currentCursor = texture;
        }

        private bool IsPointerOverClickableUI()
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult result in results)
            {
                if (result.gameObject.CompareTag("UIClickable"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
