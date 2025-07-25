using UnityEngine;
using System.Collections;

public class PasswordSphereeInteraction : MonoBehaviour
{
    private bool isSolved = false;
    public CodeWindowManager codeWindow;
    public string puzzleType = "password";
    [SerializeField] private AudioSource successSound;
    [SerializeField] private GameObject bigRobot;


void OnMouseDown()
{
    GetComponent<GlowEffect>()?.MarkInteracted();
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
        OnPasswordSolved,
        this.gameObject
    );
}


    private bool CheckPasswordCode(string userCode)
    {
        return codeWindow.RunPythonValidator("p4", userCode);
    }

    private void OnPasswordSolved()
    {
        isSolved = true;
        PlayerController.IsMovementLocked = true;
        bigRobot.GetComponent<GlowEffect>().StartGlow();
        StartCoroutine(DisableColliderAfterDelay(0.5f));
        if (successSound != null && successSound.clip != null)
            successSound.PlayOneShot(successSound.clip);
        FindFirstObjectByType<FeedbackUIManager>()?.ShowMessage("Robot Activated!");
        FindFirstObjectByType<RobotSphereController>()?.StartRollingToTarget();
        DeactivateAfterDelay(2f);
    }
    IEnumerator DisableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
    }
    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayerController.IsMovementLocked = false;
        gameObject.SetActive(false);
    }
}
