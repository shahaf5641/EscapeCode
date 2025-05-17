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

        string problemText =
        $@"The Final Boss

        first_number = 4
        second_number = 5
        third_number = 10
        button_pressed = {pressedButtonValue}

        code = first_number * second_number + third_number

        answer = 0

        if answer == code and button_pressed == True:
            door_open = True
        else:
            door_open = False

        Write a line assigning the correct value to 'answer':
        answer = ______";


        FindFirstObjectByType<ChatGPTClient>().currentPuzzle = this.gameObject;

        string defaultCode ="";

        codeWindow.Open(
            problemText,
            defaultCode,
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
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("SecondRoomScene");
    }
}
