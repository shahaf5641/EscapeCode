using UnityEngine;

public class BigRobotRepairInteraction : MonoBehaviour
{
    [SerializeField] private CodeWindowManager codeWindow;
    [SerializeField] private AudioSource activationSound;
    [SerializeField] private Animator bigRobotAnimator;
    private bool isSolved = false;
    public string puzzleType = "robot";

    void OnMouseDown()
    {
        PlayerController.IsMovementLocked = true;
        if (isSolved || codeWindow == null) return;

        string problemText =
        @"Robot Sensor Merge Protocol\n
        The robot’s left and right sensors recorded voltage data separately,
        causing a desynchronized input stream.
        To restore navigation, merge the arrays into a single, synchronized log
        by alternating their readings.

        first_sensor = [110, 230, 450, 670]
        second_sensor = [120, 240, 460, 680]
        log = []

        for i in range(4):
            log.append(first_sensor[i])
            __________

        if log == [110, 120, 230, 240, 450, 460, 670, 680]:
            activate_robot()";

        FindFirstObjectByType<ChatGPTClient>().currentPuzzle = this.gameObject;

        string defaultCode = "";

        codeWindow.Open(
            problemText,
            defaultCode,
            CheckAnswer,
            OnSolved
        );
    }

    private bool CheckAnswer(string userCode)
    {
        return userCode.Equals("log.append(second_sensor[i])");
    }

    private void OnSolved()
    {
        isSolved = true;

        if (activationSound != null && activationSound.clip != null)
            activationSound.Play();

        if (bigRobotAnimator != null)
            bigRobotAnimator.SetTrigger("Walk");
        FindFirstObjectByType<FeedbackUIManager>()?.ShowMessage("Robot Activated. Navigation restored.");
    }
}
