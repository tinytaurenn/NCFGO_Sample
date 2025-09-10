
using PurrNet;
using UnityEngine;

public class BetterRBCharMovement : MonoBehaviour
{
    [SerializeField] protected NetworkAnimator m_Animator;
    public Vector3 MoveInput { get; set; }
    public Vector3 MoveInputRaw { get; set; }
    public Vector3 PreviousMoveInputRaw { get; set; }
    public bool IsSprinting { get; set; }

    public bool IsLocked { get; private set; }
    [SerializeField] float m_MoveSpeed = 5f;
    [SerializeField] float m_MaxSpeed = 10f;
    [SerializeField] protected float m_RotationSpeed = 8f; 
    protected Rigidbody m_RigidBody;
    [SerializeField] bool m_IsGrounded = true;

    [SerializeField] float m_SprintMultiplier = 1.5f;


   [Header("Stair Stepping")]
    [SerializeField] float m_StepHeight = 0.3f;
    [SerializeField] float m_StepDistance = 0.5f;
    [SerializeField] float distanceStepMultiplier = 1f;
    [SerializeField] LayerMask m_WalkableLayer;
    [SerializeField]protected CapsuleCollider m_CapsuleCollider;

    protected virtual void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_CapsuleCollider = GetComponent<CapsuleCollider>();
    }
    protected virtual void Start()
    {
        
    }
    

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    protected virtual void Update()
    {
    }

    protected virtual void FixedUpdate()
    {
        MovementUpdate(); 
    }

    protected virtual void MovementUpdate()
    {
        Vector3 moveDir = MoveInput;
        Checkgrounded();

        if (m_IsGrounded)
        {
            RaycastHit hit;
            // Cast a ray down from the character to get the ground's normal
            if (Physics.Raycast(transform.position, Vector3.down, out hit, m_CapsuleCollider.height / 2f + 0.1f, m_WalkableLayer))
            {
                // Project the movement direction onto the plane of the ground
                moveDir = Vector3.ProjectOnPlane(moveDir, hit.normal).normalized;
            }
        }

        // Handle stairs BEFORE the movement calculation
        HandleStairs();

        Vector3 velocity = m_RigidBody.linearVelocity;
        Vector3 targetVelocity = moveDir * m_MoveSpeed;
        //targetVelocity = transform.TransformDirection(targetVelocity);

        if (IsSprinting)
        {
            targetVelocity *= m_SprintMultiplier;
        }


        Vector3 velocityChange = targetVelocity - velocity;
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);

        // This line was the problem; ClampMagnitude returns a value, it doesn't modify the original variable.
        velocityChange = Vector3.ClampMagnitude(velocityChange, m_MaxSpeed);

        m_RigidBody.AddForce(velocityChange, ForceMode.VelocityChange);
        UpdateAnimator();
    }

    void HandleStairs()
    {
        if (!m_IsGrounded || MoveInput.magnitude < 0.1f) return;

        // Calculate the base of the capsule collider
        float capsuleBaseY = transform.position.y + m_CapsuleCollider.center.y - m_CapsuleCollider.height / 2f;
        Vector3 capsuleBase = new Vector3(transform.position.x, capsuleBaseY, transform.position.z);
        Vector3 forwardDir = MoveInput ; 

        // Origins for the two raycasts, starting from the base
        Vector3 lowerRayOrigin = capsuleBase + forwardDir * m_StepDistance * 0.1f + Vector3.up * 0.05f;
        Vector3 upperRayOrigin = capsuleBase + forwardDir * m_StepDistance * 0.1f + Vector3.up * (m_StepHeight + 0.05f);

        RaycastHit lowerHit;
        float newStepDistance = IsSprinting ? m_StepDistance * 1.5f : m_StepDistance;

        if (Physics.Raycast(lowerRayOrigin, forwardDir, out lowerHit, newStepDistance, m_WalkableLayer))
        {
            float stepTopY = lowerHit.collider.bounds.max.y;
            float distanceToStepTop = stepTopY - capsuleBaseY;
            if (Vector3.Dot(lowerHit.normal, Vector3.up) < 0.1f)
            {
                if (!Physics.Raycast(upperRayOrigin, forwardDir, newStepDistance, m_WalkableLayer))
                {
                    Vector3 force = Vector3.up * m_StepHeight;  
                    //
                    float highestHitY = GetHighestHitY(lowerRayOrigin, forwardDir, newStepDistance, m_StepHeight, m_WalkableLayer);
                    ;
                    if (highestHitY != -1)
                    {
                        float heightDifference = highestHitY - capsuleBase.y;
                        Debug.Log(heightDifference); 
                        force = Vector3.up * heightDifference * distanceStepMultiplier; 
                    }

                    //Debug.Log("adding force");
                    m_RigidBody.AddForce(force, ForceMode.VelocityChange);
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

    void UpdateAnimator()
    {
        Vector2 newMoveInput = Vector2.Lerp(PreviousMoveInputRaw, MoveInputRaw, Time.deltaTime * 5f);
        if (newMoveInput.magnitude < 0.01f) newMoveInput = Vector2.zero;
        PreviousMoveInputRaw = newMoveInput;
        if (IsSprinting && newMoveInput.y > 0.5f) newMoveInput *= m_SprintMultiplier;
        m_Animator.SetFloat("VelocityZ", newMoveInput.y);
        m_Animator.SetFloat("VelocityX", newMoveInput.x);
    }

    public virtual void StopMovement()
    {
        MoveInput = Vector3.zero;
        m_RigidBody.linearVelocity = Vector3.zero;
    }

    public void Bump(Vector3 normalizedDirection, float force)
    {


      


    }

    private float GetHighestHitY(Vector3 origin, Vector3 direction, float distance, float maxStepHeight, LayerMask layerMask)
    {
        RaycastHit highestHit = new RaycastHit();
        bool foundHit = false;

        // Start a series of raycasts from the origin and move upwards
        for (float yOffset = 0; yOffset <= maxStepHeight; yOffset += 0.025f)
        {
            Vector3 rayOrigin = origin + Vector3.up * yOffset;
            RaycastHit hit;
            

            if (Physics.Raycast(rayOrigin, direction, out hit, distance, layerMask))
            {
                // Check if the surface is vertical enough to be a stair face
                if (!foundHit || hit.point.y > highestHit.point.y)
                {
                    highestHit = hit;
                    foundHit = true;
                }
            }
        }

        if (foundHit)
        {
            return highestHit.point.y;
        }
        else
        {
            // Return a default value to indicate no valid stair was found
            return -1.0f;
        }
    }

    protected virtual void OnDrawGizmos()
    {
        if (m_CapsuleCollider == null) return; 
        float capsuleBaseY = transform.position.y + m_CapsuleCollider.center.y - m_CapsuleCollider.height / 2f;
        Vector3 capsuleBase = new Vector3(transform.position.x, capsuleBaseY, transform.position.z);
        Vector3 forwardDir = MoveInput;

        // Origins for the two raycasts, starting from the base
        Vector3 lowerRayOrigin = capsuleBase + forwardDir * m_StepDistance * 0.1f + Vector3.up * 0.05f;
        Vector3 upperRayOrigin = capsuleBase + forwardDir * m_StepDistance * 0.1f + Vector3.up * (m_StepHeight + 0.05f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(lowerRayOrigin, 0.05f);
        Gizmos.DrawRay(lowerRayOrigin, forwardDir * m_StepDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(upperRayOrigin, 0.05f);
        Gizmos.DrawRay(upperRayOrigin, forwardDir * m_StepDistance);

        Gizmos.color = Color.green;
        Vector3 moveDir = MoveInput;
        if (m_IsGrounded)
        {
            RaycastHit hit;
            // Cast a ray down from the character to get the ground's normal
            if (Physics.Raycast(transform.position, Vector3.down, out hit, m_CapsuleCollider.height / 2f + 0.1f, m_WalkableLayer))
            {
                // Project the movement direction onto the plane of the ground
                moveDir = Vector3.ProjectOnPlane(moveDir, hit.normal).normalized;
                Gizmos.DrawRay(transform.position,moveDir*2);
            }
        }

    }
}
