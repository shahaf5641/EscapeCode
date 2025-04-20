using UnityEngine;

public class PressableButton : MonoBehaviour
{
    public Material redMaterial;
    public Material greenMaterial;

    private bool isPressed = false;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material = redMaterial; // Set to red at start
    }

    void OnMouseDown()
    {
        if (!isPressed)
        {
            isPressed = true;
            PuzzleState.pressedButton = true;

            rend.material = greenMaterial; // Change to green
            FindFirstObjectByType<FeedbackUIManager>().ShowMessage("Button pressed!");
        }
    }
}
