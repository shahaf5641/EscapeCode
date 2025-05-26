using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DoorInteraction : MonoBehaviour
{
    public CodeWindowManager codeWindow;
    private bool doorOpened = false;
    public string puzzleType = "door";
    [SerializeField] private AudioSource successSound;
    void Start()
    {
        PuzzleState.pressedButton = false;
    }

    void OnMouseDown()
    {
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
            OnDoorSolved
        );
    }


    private bool CheckDoorCode(string userCode)
    {
        string code = userCode.ToLower().Replace(" ", "").Replace("\n", "").Replace("\r", "");

        bool containsKeyLogic = code.Contains("answer=30");

        return containsKeyLogic && PuzzleState.pressedButton && KeyInventory.HasKey;
    }

    private void OnDoorSolved()
    {
        if (doorOpened) return;
        successSound.PlayOneShot(successSound.clip);
        doorOpened = true;
        FindFirstObjectByType<FeedbackUIManager>().ShowMessage("Room solved!");
        gameObject.GetComponent<BoxCollider>().enabled = false;
        // ✅ Start the delay to load next scene
        StartCoroutine(LoadNextSceneAfterDelay());
    }

    private IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("SecondRoomScene");
    }
}
