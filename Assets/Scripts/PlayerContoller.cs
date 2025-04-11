using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using System.Numerics;
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
    [Header("Movement")]
    [SerializeField] ParticleSystem clickEffect;
    [SerializeField] LayerMask clickableLayers;
    float lookRotationSpeed = 8f;
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
            // ✅ Check if this hit object is interactable
            if (hit.collider.GetComponent<BookInteraction>() != null ||
                hit.collider.GetComponent<ChestInteraction>() != null)
            {
                Debug.Log("Click on interactable. Skipping movement.");
                return; // Don't move!
            }
        }

        // ✅ If no interactable hit, use the FIRST hit for movement
        if (hits.Length > 0)
        {
            RaycastHit moveHit = hits[0];
            agent.destination = moveHit.point;

            if (clickEffect != null)
            {
                Instantiate(clickEffect, moveHit.point + new Vector3(0, 0.1f, 0), clickEffect.transform.rotation);
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
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent is missing on Player!");
            return;
        }

        if (agent.velocity == Vector3.zero)
        {
            animator.Play(IDLE);

        }
        else
        {
            animator.Play(WALK);
        }
    }
}
