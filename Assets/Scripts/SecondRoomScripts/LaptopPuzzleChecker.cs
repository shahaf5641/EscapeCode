 using UnityEngine;
using System.Collections;

public class LaptopInteraction : MonoBehaviour
{
    private bool isSolved = false;
    public CodeWindowManager codeWindow;
    public string puzzleType = "laptop";
    [SerializeField] private AudioSource successSound;
    public Unity.Cinemachine.CinemachineCamera sphereRobotCam;
    public Unity.Cinemachine.CinemachineCamera playerCam;
    [SerializeField] private GameObject passwordPanel;
void OnMouseDown()
{
    PlayerController.IsMovementLocked = true;
    if (isSolved || codeWindow == null) return;

    string problemTitle = "Online Server Count";

    string problemDescription =
@"You're monitoring a room of five servers — some online, some down.
To restore full access, first you’ll need to count how many are active.
Complete the condition that tallies the online systems.";

    string problemCode =
@"servers_status = [True, False, True, True, False]
online_count = 0
for status in servers_status:
    if __________:
        online_count += 1";

    FindFirstObjectByType<ChatGPTClient>().currentPuzzle = this.gameObject;

    codeWindow.Open(
        problemTitle,
        problemDescription,
        problemCode,
        CheckLaptopCode,
        OnLaptopSolved
    );
}

    private bool CheckLaptopCode(string userCode)
    {
        return codeWindow.RunPythonValidator("p3", userCode);
    }
    private void OnLaptopSolved()
    {
        isSolved = true;
        PlayerController.IsMovementLocked = true;
        successSound.PlayOneShot(successSound.clip);
        FindFirstObjectByType<FeedbackUIManager>().ShowMessage("Message sent!");
        var collider = passwordPanel.GetComponent<BoxCollider>();
        var thisCollider = GetComponent<BoxCollider>();
        if (thisCollider != null)
            thisCollider.enabled = false;
        if (collider != null)
            collider.enabled = true;
        sphereRobotCam.Priority = 20; 
        playerCam.Priority = 10; 
        StartCameraAndAnimationSequence();

    }
    public void StartCameraAndAnimationSequence()
    {
        StartCoroutine(TriggerRobotAnimationAfterDelay(1f));
        StartCoroutine(ReturnToPlayerCameraAfterDelay(4f)); // 1s + 3s = 4s total
    }

    private IEnumerator TriggerRobotAnimationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Trigger robot wake-up animation
        FindFirstObjectByType<RobotSphereController>()?.PlayWakeUpAnimation();
    }

    private IEnumerator ReturnToPlayerCameraAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Switch back to player camera
        PlayerController.IsMovementLocked = false;
        sphereRobotCam.Priority = 5;
        playerCam.Priority = 15;
    }
}
