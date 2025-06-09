using UnityEngine;
using System.Collections;

public class GlobalUIActivator : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(EnableHamburgerDelayed());
    }

    IEnumerator EnableHamburgerDelayed()
    {
        // Wait 1 frame to ensure DontDestroyOnLoad objects are ready
        yield return null;

        GameObject hamburger = GameObject.Find("GlobalUICanvas");
        if (hamburger != null)
        {
            hamburger.SetActive(true);
            Debug.Log("✅ GlobalUICanvas activated.");
        }
        else
        {
            Debug.LogWarning("❌ GlobalUICanvas not found.");
        }
    }
}
