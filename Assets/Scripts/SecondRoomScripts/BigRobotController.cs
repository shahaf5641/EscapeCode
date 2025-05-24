using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class BigRobotController : MonoBehaviour
{
    public static bool IsMovementLocked = false;
    const string IDLE = "Idle";
    const string WALK = "Walk";
    CustomActions input;
    NavMeshAgent agent;
    Animator animator;
    [SerializeField] private AudioSource walkSound;
    [Header("Movement")]
    [SerializeField] ParticleSystem clickEffect;
    private ParticleSystem currentClickEffect;
    [SerializeField] LayerMask clickableLayers;
    float lookRotationSpeed = 2f;
    private bool isWalking = false;

    void Awake()
    {
        IsMovementLocked = false;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        input = new CustomActions();
        AssignInputs();
    }

    void AssignInputs()
    {
        input.Main.Move.performed += ctx => ClickToMove();
    }

    void ClickToMove()
    {
        if (IsMovementLocked) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 100, clickableLayers);

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("WorldClickable"))
            {
                return;
            }
        }

        if (hits.Length > 0)
        {
            RaycastHit moveHit = hits[0];
            agent.destination = moveHit.point;

            if (currentClickEffect != null)
                Destroy(currentClickEffect.gameObject);

            if (clickEffect != null)
                currentClickEffect = Instantiate(clickEffect, moveHit.point + Vector3.up * 0.1f, Quaternion.identity);
        }
    }

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        FaceTarget();
        SetAnimations();

        if (currentClickEffect != null && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            Destroy(currentClickEffect.gameObject);
            currentClickEffect = null;
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (agent.steeringTarget - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
        }
    }
    void SetAnimations()
    {
        if (agent == null || !agent.enabled) return;

        bool shouldWalk = agent.velocity.sqrMagnitude > 0.1f;

        if (shouldWalk && !isWalking)
        {
            animator.SetBool("IsWalking", true);
            isWalking = true;

            if (walkSound != null && !walkSound.isPlaying)
                walkSound.Play(); // ✅ Will loop automatically because Loop is checked
        }
        else if (!shouldWalk && isWalking)
        {
            animator.SetBool("IsWalking", false);
            isWalking = false;

            if (walkSound != null && walkSound.isPlaying)
                walkSound.Stop(); // ✅ Stops when idle
        }
    }
}
