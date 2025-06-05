using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
public class PlayerController : MonoBehaviour
{
    public static bool IsMovementLocked = false;

    const string IDLE = "Idle";
    const string WALK = "Walk";
    CustomActions input;
    NavMeshAgent agent;
    Animator animator;
    private bool isWalking = false;
    private AudioSource walkSource;

    [SerializeField] private AudioSource walkLoopSound;

    [Header("Movement")]
    [SerializeField] ParticleSystem clickEffect;
    private ParticleSystem currentClickEffect;
    [SerializeField] LayerMask clickableLayers;
    float lookRotationSpeed = 2f;

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
            {
                Destroy(currentClickEffect.gameObject);
            }

            if (clickEffect != null)
            {
                currentClickEffect = Instantiate(clickEffect, moveHit.point + Vector3.up * 0.1f, Quaternion.identity);
            }
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
        if (agent == null) return;

        bool shouldWalk = agent.velocity.sqrMagnitude > 0.1f;

        if (shouldWalk && !isWalking)
        {
            animator.Play(WALK);
            isWalking = true;

            if (walkLoopSound != null && !walkLoopSound.isPlaying)
                walkLoopSound.Play();
        }
        else if (!shouldWalk && isWalking)
        {
            animator.Play(IDLE);
            isWalking = false;

            if (walkLoopSound != null && walkLoopSound.isPlaying)
                walkLoopSound.Stop();
        }
    }
}
