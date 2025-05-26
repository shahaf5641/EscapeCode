using UnityEngine;
using System.Collections;

public class PasswordSphereeInteraction : MonoBehaviour
{
    private bool isSolved = false;
    public CodeWindowManager codeWindow;
    public string puzzleType = "password";
    [SerializeField] private AudioSource successSound;
    [SerializeField] private GameObject handRailToDisable;


void OnMouseDown()
{
    if (isSolved || codeWindow == null) return;

    PlayerController.IsMovementLocked = true;

    string problemTitle = "Robot Activation";

        string problemDescription =
@"The sphere robot is powered up, but won’t move until the right password is found.
Scan the list. Identify the condition that activates it.
One line stands between silence — and motion.";

    string problemCode =
@"passwords = [8271, 1235, 4312, 9001]
i = 0
password = 1
while i < 4:
    if passwords[i] % 2 == _____:
        password = passwords[i]
    i += 1
if password == 4312:
    activate_robot()";

    FindFirstObjectByType<ChatGPTClient>().currentPuzzle = this.gameObject;

    codeWindow.Open(
        problemTitle,
        problemDescription,
        problemCode,
        CheckPasswordCode,
        OnPasswordSolved
    );
}


    private bool CheckPasswordCode(string userCode)
    {
        return userCode.Equals("0");
    }

    private void OnPasswordSolved()
    {
        isSolved = true;
                var thisCollider = GetComponent<BoxCollider>();
        if (thisCollider != null)
            thisCollider.enabled = false;
        if (successSound != null && successSound.clip != null)
            successSound.PlayOneShot(successSound.clip);
        handRailToDisable.SetActive(false);
        FindFirstObjectByType<FeedbackUIManager>()?.ShowMessage("Robot Activated!");
        FindFirstObjectByType<RobotSphereController>()?.StartRollingToTarget();
        DeactivateAfterDelay(2f);
    }
        private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
