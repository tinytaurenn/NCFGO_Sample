
using UnityEngine;

public class BetterRBCharMovement : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    private Vector2 moveInput;
    public float moveSpeed = 5f;
    public float maxSpeed = 10f;
    private Rigidbody rb;
    public bool m_IsGrounded = true;

    [Header("Stair Stepping")]
    public float m_StepHeight = 0.3f;
    public float m_StepDistance = 0.5f;
    public float m_StepSmoothSpeed = 10f;
    public float m_StepUpOffset = 0.05f;
    public float distanceStepMultiplier = 1f; 
    public LayerMask m_WalkableLayer;
    private CapsuleCollider m_CapsuleCollider;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();
        rb = GetComponent<Rigidbody>();
        m_CapsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Update()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Checkgrounded();

        // Handle stairs BEFORE the movement calculation
        HandleStairs();

        Vector3 velocity = rb.linearVelocity;
        Vector3 targetVelocity = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;
        targetVelocity = transform.TransformDirection(targetVelocity);

        Vector3 velocityChange = targetVelocity - velocity;
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);

        // This line was the problem; ClampMagnitude returns a value, it doesn't modify the original variable.
        velocityChange = Vector3.ClampMagnitude(velocityChange, maxSpeed);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    void HandleStairs()
    {
        if (!m_IsGrounded || moveInput.magnitude < 0.1f) return;

        // Calculate the base of the capsule collider
        float capsuleBaseY = transform.position.y + m_CapsuleCollider.center.y - m_CapsuleCollider.height / 2f;
        Vector3 capsuleBase = new Vector3(transform.position.x, capsuleBaseY, transform.position.z);
        Vector3 forwardDir = new Vector3(moveInput.x, 0, moveInput.y) ; 

        // Origins for the two raycasts, starting from the base
        Vector3 lowerRayOrigin = capsuleBase + forwardDir * m_StepDistance * 0.1f + Vector3.up * 0.05f;
        Vector3 upperRayOrigin = capsuleBase + forwardDir * m_StepDistance * 0.1f + Vector3.up * (m_StepHeight + 0.05f);

        RaycastHit lowerHit;

        if (Physics.Raycast(lowerRayOrigin, forwardDir, out lowerHit, m_StepDistance, m_WalkableLayer))
        {
            float stepTopY = lowerHit.collider.bounds.max.y;
            float distanceToStepTop = stepTopY - capsuleBaseY;
            if (Vector3.Dot(lowerHit.normal, Vector3.up) < 0.1f)
            {
                if (!Physics.Raycast(upperRayOrigin, forwardDir, m_StepDistance, m_WalkableLayer))
                {
                    Debug.Log("adding force");
                    rb.AddForce(Vector3.up * m_StepSmoothSpeed * (distanceToStepTop * distanceStepMultiplier), ForceMode.VelocityChange);
                }
            }

            
        }
    }

    void Checkgrounded()
    {
        float capsuleBaseY = transform.position.y + m_CapsuleCollider.center.y - m_CapsuleCollider.height / 2f;
        Vector3 capsuleBase = new Vector3(transform.position.x, capsuleBaseY  +0.05f, transform.position.z);
        m_IsGrounded = Physics.Raycast(capsuleBase, Vector3.down, 0.1f, m_WalkableLayer);
    }
}
