using UnityEngine;
using System.Collections;

public class DoorInteraction : MonoBehaviour
{
    public CodeWindowManager codeWindow;
    private bool doorOpened = false;
    void Start()
    {
        PuzzleState.pressedButton = false;
    }

    void OnMouseDown()
    {
        if (doorOpened || codeWindow == null) return;

        string pressedButtonValue = PuzzleState.pressedButton ? "True" : "False";

        string problemText =
        @"The Final Boss

        first_number = 4
        second_number = 5
        third_number = 10
        button_pressed = False

        code = first_number * second_number + third_number

        answer = 0

        if answer == code and button_pressed == True:
            door_open = True
        else:
            door_open = False

        Write a line assigning the correct value to 'answer':
        answer = ""______""

        Need a hint? Say: “Give me a clue” or “Help me solve this”";

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

        bool containsKeyLogic = code.Contains("has_key=true");

        return containsKeyLogic && PuzzleState.pressedButton && KeyInventory.HasKey;
    }

    private void OnDoorSolved()
    {
        if (doorOpened) return;
        doorOpened = true;
        codeWindow.Close();
        FindFirstObjectByType<FeedbackUIManager>().ShowMessage("Room solved!");
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }
}
