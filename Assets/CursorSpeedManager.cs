using TMPro;
using UnityEngine;

public class CursorSpeedManager : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Dropdown speedDropdown;      // התפריט של בחירת המהירות
    public RectTransform cursorObject;      // המצביע שאתה מזיז ב־UI

    public static float cursorSpeedMultiplier = 1f; // מהירות ברירת מחדל

    void Start()
    {
        if (speedDropdown != null)
        {
            speedDropdown.onValueChanged.AddListener(OnCursorSpeedChanged);
            OnCursorSpeedChanged(speedDropdown.value); // הפעלה ראשונית
        }
    }

    void Update()
    {
        float moveX = Input.GetAxis("Mouse X") * cursorSpeedMultiplier;
        float moveY = Input.GetAxis("Mouse Y") * cursorSpeedMultiplier;

        if (cursorObject != null)
        {
            cursorObject.anchoredPosition += new Vector2(moveX, moveY);
        }
    }

    public void OnCursorSpeedChanged(int index)
    {
        if (speedDropdown == null || speedDropdown.options.Count <= index)
        {
            Debug.LogWarning("Dropdown option index out of range or not assigned!");
            return;
        }

        string selectedOption = speedDropdown.options[index].text.Trim();

        switch (selectedOption)
        {
            case "Normal":
                SetCursorSpeed(1f);
                break;
            case "Slow":
                SetCursorSpeed(0.5f);
                break;
            case "Very Slow":
                SetCursorSpeed(0.25f);
                break;
            default:
                SetCursorSpeed(1f);
                break;
        }
    }

    private void SetCursorSpeed(float speed)
    {
        cursorSpeedMultiplier = speed;
        Debug.Log("Cursor speed set to: " + cursorSpeedMultiplier);
    }
}
