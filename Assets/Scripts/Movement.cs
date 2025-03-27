using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    [SerializeField] private InputAction movementAction;

    private NavMeshAgent agent;
    private Camera mainCamera;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        movementAction.Enable();
    }

    private void OnDisable()
    {
        movementAction.Disable();
    }

    private void Update()
    {
        if (movementAction.triggered)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                agent.SetDestination(hit.point);
            }
        }
    }
}
