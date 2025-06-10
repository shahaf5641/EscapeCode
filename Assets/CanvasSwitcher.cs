using UnityEngine;

public class CanvasSwitcher : MonoBehaviour
{
    [Header("Canvas Names")]
    public string[] canvasNamesToDisable;
    public string[] canvasNamesToEnable;

    private GameObject[] canvasesToDisable;
    private GameObject[] canvasesToEnable;

    public void Switch()
    {
        AssignCanvasesDynamically();

        foreach (GameObject go in canvasesToDisable)
        {
            if (go != null) go.SetActive(false);
        }

        foreach (GameObject go in canvasesToEnable)
        {
            if (go != null) go.SetActive(true);
        }
    }

    private void AssignCanvasesDynamically()
    {
        canvasesToDisable = new GameObject[canvasNamesToDisable.Length];
        for (int i = 0; i < canvasNamesToDisable.Length; i++)
        {
            canvasesToDisable[i] = GameObject.Find(canvasNamesToDisable[i]);
        }

        canvasesToEnable = new GameObject[canvasNamesToEnable.Length];
        for (int i = 0; i < canvasNamesToEnable.Length; i++)
        {
            canvasesToEnable[i] = GameObject.Find(canvasNamesToEnable[i]);
        }
    }
}
