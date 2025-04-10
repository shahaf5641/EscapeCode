using UnityEngine;

public class ChestInteraction : MonoBehaviour
{
    private Animator animator;
    public GameObject key; // Assign this in Inspector (initially disabled)
    public GameObject chestCodeWindowPanel; // Code panel shown on first click

    void Start()
    {
        animator = GetComponent<Animator>();
        gameObject.SetActive(false); // Hide chest at start

        if (key != null)
        {
            key.SetActive(false); // Hide key at start
        }

        if (chestCodeWindowPanel != null)
        {
            chestCodeWindowPanel.SetActive(false); // Hide code popup at start
        }
    }

    void OnMouseDown()
    {
        if (chestCodeWindowPanel != null && !chestCodeWindowPanel.activeSelf)
        {
            chestCodeWindowPanel.SetActive(true); // show puzzle instead of auto-opening
        }
    }

    public void OpenChestAndRevealKey()
    {
        animator.SetTrigger("Open");

        if (key != null)
        {
            key.SetActive(true);
            Transform glow = key.transform.Find("KeyGlow");
            if (glow != null)
            {
                glow.gameObject.SetActive(true);
            }
        }
    }
}
