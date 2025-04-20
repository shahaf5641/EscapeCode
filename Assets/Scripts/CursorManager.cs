using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D defaultCursor;
    public Texture2D hoverCursor;
    public string hoverableTag = "Hoverable";
    public Vector2 hotSpot = Vector2.zero;

    private bool isHovering = false;

    void Start()
    {
        Cursor.SetCursor(defaultCursor, hotSpot, CursorMode.Auto);
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag(hoverableTag))
            {
                if (!isHovering)
                {
                    Cursor.SetCursor(hoverCursor, hotSpot, CursorMode.Auto);
                    isHovering = true;
                }
                return;
            }
        }

        if (isHovering)
        {
            Cursor.SetCursor(defaultCursor, hotSpot, CursorMode.Auto);
            isHovering = false;
        }
    }
}
