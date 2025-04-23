using UnityEngine;

public class CursorMover : MonoBehaviour
{
    public RectTransform cursorObject;
    public static float cursorSpeedMultiplier = 1f; 

    void Update()
    {
        float moveX = Input.GetAxis("Mouse X") * cursorSpeedMultiplier;
        float moveY = Input.GetAxis("Mouse Y") * cursorSpeedMultiplier;

        cursorObject.anchoredPosition += new Vector2(moveX, moveY);
    }
}