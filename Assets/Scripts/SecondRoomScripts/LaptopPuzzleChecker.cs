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
        string problemText =
        @"Online Server Count
        The robot is trying to connect, but it can only connect if it knows how many servers are currently online.
        servers_status = [True, False, True, True, False]
        online_count = 0

        for status in servers_status:
            if __________:
                online_count += 1

        This server room has five systems.
        Can you fill in the condition to count how many are currently online?";
        FindFirstObjectByType<ChatGPTClient>().currentPuzzle = this.gameObject;
        string defaultCode ="";
        codeWindow.Open(
            problemText,
            defaultCode,
            CheckLaptopCode,
            OnLaptopSolved
        );
    }

    private bool CheckLaptopCode(string userCode)
    {
        return userCode.Contains("status == True") || userCode.Equals("status");
    }

    private void OnLaptopSolved()
    {
        isSolved = true;
        successSound.PlayOneShot(successSound.clip);
        FindFirstObjectByType<FeedbackUIManager>().ShowMessage("Laptop solved!");
        var collider = passwordPanel.GetComponent<BoxCollider>();
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
        sphereRobotCam.Priority = 5;
        playerCam.Priority = 15;
    }
}
