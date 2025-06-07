using UnityEngine;

public class PressableButton : MonoBehaviour
{
    public Material redMaterial;
    public Material greenMaterial;
    [SerializeField] public GameObject doorObject;
    private bool isPressed = false;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material = redMaterial; // Set to red at start
    }

    void OnMouseDown()
    {
        GetComponent<GlowEffect>()?.MarkInteracted();
        doorObject.GetComponent<GlowEffect>().ResetInteraction();
        doorObject.GetComponent<GlowEffect>().StartGlow();
        if (!isPressed)
        {
            isPressed = true;
            PuzzleState.pressedButton = true;

            rend.material = greenMaterial; // Change to green
            FindFirstObjectByType<FeedbackUIManager>().ShowMessage("Button pressed!");
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
