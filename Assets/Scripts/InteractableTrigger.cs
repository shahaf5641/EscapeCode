using UnityEngine;

public class InteractableTrigger : MonoBehaviour
{
    [Tooltip("The UI panel or object to activate on click.")]
    public GameObject objectToActivate;

    private bool hasBeenActivated = false;

    void OnMouseDown()
    {
        if (!hasBeenActivated && objectToActivate != null)
        {
            objectToActivate.SetActive(true);
            hasBeenActivated = true;
        }
    }
}
