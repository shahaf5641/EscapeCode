using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DoorInteraction : MonoBehaviour
{
    public CodeWindowManager codeWindow;
    [SerializeField] public GameObject buttonObject;
    private bool doorOpened = false;
    public string puzzleType = "door";
    [SerializeField] private AudioSource successSound;
    void Start()
    {
        PuzzleState.pressedButton = false;
    }

    void OnMouseDown()
    {
        GetComponent<GlowEffect>()?.MarkInteracted();
        buttonObject.GetComponent<GlowEffect>().StartGlow();
        if (doorOpened || codeWindow == null) return;

        string pressedButtonValue = PuzzleState.pressedButton ? "True" : "False";

        string problemTitle = "The Final Boss";

        string problemDescription =
@"This is it — the final step in the sequence.
Get the value right, and the path opens. Get it wrong… and it stays sealed.
Assign the correct value to 'answer':
answer = ______";

        string problemCode = 
$@"first_number = 4
second_number = 5
third_number = 10
button_pressed = {pressedButtonValue}
code = first_number * second_number + third_number
answer = 0
if answer == code and button_pressed == True:
    door_open = True
else:
    door_open = False";

        FindFirstObjectByType<ChatGPTClient>().currentPuzzle = this.gameObject;

        codeWindow.Open(
            problemTitle,
            problemDescription,
            problemCode,
            CheckDoorCode,
            OnDoorSolved,
            this.gameObject
        );
    }
    private bool CheckDoorCode(string userCode)
    {
        bool validCode = codeWindow.RunPythonValidator("p2", userCode);
        return validCode && PuzzleState.pressedButton && KeyInventory.HasKey;
    }
    private void OnDoorSolved()
    {
        if (doorOpened) return;
        successSound.PlayOneShot(successSound.clip);
        doorOpened = true;
        FindFirstObjectByType<FeedbackUIManager>().ShowMessage("Room solved!");
        StartCoroutine(DisableColliderAfterDelay(0.5f));
        // ✅ Start the delay to load next scene
        StartCoroutine(LoadNextSceneAfterDelay());
    }
    IEnumerator DisableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
    }

    private IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("SecondRoomScene");
    }
}
