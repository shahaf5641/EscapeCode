using UnityEngine;
using UnityEngine.AI;
using Unity.Cinemachine;
using System.Collections;

public class RobotSphereController : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent agent;

    [SerializeField] private Transform destination;
    [SerializeField] private GameObject bigRobot;
    [SerializeField] private CinemachineCamera sphereFollowCam;
    [SerializeField] private CinemachineCamera playerCam;
    private Vector3 preRollPosition;
    [SerializeField] private NavMeshObstacle obstacle;


    private bool hasRolled = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
            agent.enabled = false; // Only enable when rolling starts
    }

    // Called when laptop is solved
    public void PlayWakeUpAnimation()
    {
        anim.SetTrigger("Open_Anim");
    }

    // Called when password panel is solved
    public void StartRollingToTarget()
    {
        if (hasRolled || destination == null) return;

        hasRolled = true;
        anim.SetTrigger("Roll_Anim");
        PlayerController.IsMovementLocked = true;
        if (sphereFollowCam != null) sphereFollowCam.Priority = 20;
        if (playerCam != null) playerCam.Priority = 5;
        StartCoroutine(BeginMovementAfterRoll());
    }
    private IEnumerator BeginMovementAfterRoll()
    {
        // Wait for the length of the Roll_Anim
        yield return new WaitForSeconds(1f);
        if (agent != null)
        {
            agent.enabled = true;
            agent.SetDestination(destination.position);
        }
    }
    void Update()
    {
        if (agent == null || !agent.enabled || !agent.hasPath) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Reached destination
            anim.ResetTrigger("Roll_Anim");
            if (bigRobot != null && !bigRobot.activeSelf)
                bigRobot.SetActive(true);
            obstacle.enabled = true;
            // Switch back to player cam
            StartCoroutine(SwitchCameraBackWithDelay(1f));
            agent.enabled = false;
        }
    }
    private IEnumerator SwitchCameraBackWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (sphereFollowCam != null) sphereFollowCam.Priority = 5;
        if (playerCam != null) playerCam.Priority = 15;
    }

}
