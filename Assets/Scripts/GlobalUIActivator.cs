using UnityEngine;
using System.Collections;

public class GlobalUIActivator : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f); // wait enough time

        GameObject btn = null;
        foreach (Transform t in GameObject.FindObjectsOfType<Transform>(true))
        {
            if (t.name == "GlobalButtonMenu")
            {
                btn = t.gameObject;
                break;
            }
        }

        if (btn != null)
        {
            btn.SetActive(true);
            Debug.Log("✅ GlobalButtonMenu found and activated.");
        }
        else
        {
            Debug.LogWarning("❌ GlobalButtonMenu could not be found in hierarchy.");
        }
    }
}
