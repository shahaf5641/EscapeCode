using UnityEngine;

public class CanvasSwitcher : MonoBehaviour
{
    [Header("Canvas References (Optional)")]
    [SerializeField] private GameObject canvasToDisable;
    [SerializeField] private GameObject canvasToEnable;

    [Header("Canvas Names (Used If References Are Missing)")]
    [SerializeField] private string canvasNameToDisable;
    [SerializeField] private string canvasNameToEnable;

    public void Switch()
    {
        if (canvasToDisable == null && !string.IsNullOrEmpty(canvasNameToDisable))
        {
            canvasToDisable = FindObjectAnywhere(canvasNameToDisable);
        }

        if (canvasToEnable == null && !string.IsNullOrEmpty(canvasNameToEnable))
        {
            canvasToEnable = FindObjectAnywhere(canvasNameToEnable);
        }

        if (canvasToDisable != null)
            canvasToDisable.SetActive(false);

        if (canvasToEnable != null)
            canvasToEnable.SetActive(true);
    }
    private GameObject FindObjectAnywhere(string name)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == name)
                return obj;
        }
        return null;
    }
}
