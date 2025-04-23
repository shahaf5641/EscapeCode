using UnityEngine;

public class CanvasSwitcher : MonoBehaviour
{
    [Header("Canvases")]
    public GameObject[] canvasesToDisable;
    public GameObject[] canvasesToEnable;

    public void Switch()
    {
        foreach (GameObject go in canvasesToDisable)
        {
            if (go != null) go.SetActive(false);
        }

        foreach (GameObject go in canvasesToEnable)
        {
            if (go != null) go.SetActive(true);
        }
    }
}
