using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class HoverHighlighter : MonoBehaviour
{
    public float highlightDuration = 5f;

    private GameObject currentHighlighted;
    private float timer;

    void Update()
    {
        // פועל רק אם EyeTracking דלוק
        if (PlayerPrefs.GetInt("EyeTrackingEnabled", 0) != 1)
        {
            ClearHighlight();
            return;
        }

        Vector2 mousePos = Input.mousePosition;

        PointerEventData pointer = new PointerEventData(EventSystem.current) { position = mousePos };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, results);

        GameObject hovered = null;

        foreach (var result in results)
        {
            if (result.gameObject.GetComponent<Selectable>() != null)
            {
                hovered = result.gameObject;
                break;
            }
        }

        if (hovered != null)
        {
            if (hovered != currentHighlighted)
            {
                ClearHighlight();
                Highlight(hovered);
            }

            timer = highlightDuration;
        }

        if (currentHighlighted != null)
        {
            timer -= Time.deltaTime;

            if (Input.GetMouseButtonDown(0))
            {
                var pointerClick = new PointerEventData(EventSystem.current)
                {
                    position = Input.mousePosition
                };

                ExecuteEvents.Execute(currentHighlighted, pointerClick, ExecuteEvents.pointerClickHandler);
            }

            if (timer <= 0f)
            {
                ClearHighlight();
            }
        }
    }

    void Highlight(GameObject go)
    {
        currentHighlighted = go;

        var selectable = go.GetComponent<Selectable>();
        if (selectable != null)
        {
            EventSystem.current.SetSelectedGameObject(go);
        }
    }

    void ClearHighlight()
    {
        if (currentHighlighted != null)
        {
            if (EventSystem.current.currentSelectedGameObject == currentHighlighted)
                EventSystem.current.SetSelectedGameObject(null);

            currentHighlighted = null;
        }
    }
}
