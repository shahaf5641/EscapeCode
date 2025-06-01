using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BigRobotRepairInteraction : MonoBehaviour
{
    [SerializeField] private CodeWindowManager codeWindow;
    [SerializeField] private AudioSource clickSound;
    [SerializeField] private AudioClip activationSound;
    [SerializeField] private Animator bigRobotAnimator;
    [SerializeField] private GameObject bigRobot;
    [SerializeField] private Unity.Cinemachine.CinemachineCamera robotFocusCam;
    [SerializeField] private Unity.Cinemachine.CinemachineCamera robotGameplayCam;
    [SerializeField] private Transform robotTargetPoint;
    [SerializeField] private BoxCollider finalDoorCollider;
    private bool isSolved = false;
    public string puzzleType = "robot";

void OnMouseDown()
{
    PlayerController.IsMovementLocked = true;
    clickSound.PlayOneShot(clickSound.clip);

    if (isSolved || codeWindow == null) return;

    string problemTitle = "Robot Sensor Merge Protocol";

    string problemDescription =
@"The robot’s left and right sensors recorded voltage data separately,
causing a desynchronized input stream.
To restore navigation, merge the arrays into a single, synchronized log
by alternating their readings.";

    string problemCode =
@"first_sensor = [110, 230, 450, 670]
second_sensor = [120, 240, 460, 680]
log = []
for i in range(4):
    log.append(first_sensor[i])
    __________________________
if log == [110, 120, 230, 240, 450, 460, 670, 680]:
    activate_robot()";

    FindFirstObjectByType<ChatGPTClient>().currentPuzzle = this.gameObject;

    codeWindow.Open(
        problemTitle,
        problemDescription,
        problemCode,
        CheckAnswer,
        OnSolved
    );
}


    private bool CheckAnswer(string userCode)
    {
        return codeWindow.RunPythonValidator("p5", userCode);
    }
    private void OnSolved()
    {
        isSolved = true;
        BigRobotController.IsMovementLocked = true;
        finalDoorCollider.enabled = true;
        if (activationSound != null && clickSound != null)
            clickSound.PlayOneShot(activationSound);

        if (bigRobotAnimator != null)
            bigRobotAnimator.SetTrigger("Walk");

        FindFirstObjectByType<FeedbackUIManager>()?.ShowMessage("System Restored!");

        // 🎥 Focus camera on robot
        if (robotFocusCam != null) robotFocusCam.Priority = 20;
        if (robotGameplayCam != null) robotGameplayCam.Priority = 5;

        // Disable player, enable robot
        FindFirstObjectByType<PlayerController>().enabled = false;
        bigRobot.GetComponent<BigRobotController>().enabled = true;
        bigRobot.GetComponent<NavMeshAgent>().enabled = true;
        bigRobot.GetComponent<NavMeshObstacle>().enabled = false;
        bigRobot.GetComponent<BoxCollider>().enabled = false;

        // GameObject player = GameObject.FindWithTag("Player");
        // if (player != null) player.SetActive(false);

        // Set destination for robot and start camera coroutine
        bigRobot.GetComponent<NavMeshAgent>().SetDestination(robotTargetPoint.position);
        StartCoroutine(WaitForRobotArrival());
    }

    private IEnumerator WaitForRobotArrival()
    {
        // 🔒 Lock robot input while we wait for it to arrive
        BigRobotController.IsMovementLocked = true;

        NavMeshAgent agent = bigRobot.GetComponent<NavMeshAgent>();

        // Wait until the robot has reached its destination
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            yield return null;

        // Wait an extra second for animation or sound to finish
        yield return new WaitForSeconds(1f);

        // Reassign camera follow/LookAt
        if (GameObject.Find("CM vcam1").TryGetComponent<Unity.Cinemachine.CinemachineCamera>(out var vcam))
        {
            vcam.Follow = bigRobot.transform;
            vcam.LookAt = bigRobot.transform;
        }

        // Switch camera priorities
        if (robotFocusCam != null) robotFocusCam.Priority = 5;
        if (robotGameplayCam != null) robotGameplayCam.Priority = 20;

        // 🔓 Unlock robot movement
        BigRobotController.IsMovementLocked = false;
    }
}
