using UnityEngine;

public class VehiculeMovement : MonoBehaviour
{
    public Vector3 MoveInput { get; set; }
    public Vector3 MoveInputRaw { get; set; }
    public Vector3 PreviousMoveInputRaw { get; set; }

    [SerializeField] float m_MoveSpeed = 5f;
    [SerializeField] float m_MaxSpeed = 10f;
    [SerializeField] LayerMask m_WalkableLayer;
    [SerializeField] Collider m_Collider;
    protected Rigidbody m_RigidBody;
    [SerializeField] bool m_IsGrounded = true;

    private void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<Collider>();

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected virtual void FixedUpdate()
    {
        MovementUpdate();
    }
    protected virtual void MovementUpdate()
    {
        //if()
        Vector3 moveDir = MoveInput;
        Checkgrounded();
        CapsuleCollider collider =  m_Collider.GetComponent<CapsuleCollider>();

        if (m_IsGrounded)
        {
            RaycastHit hit;
            // Cast a ray down from the character to get the ground's normal
            if (Physics.Raycast(transform.position, Vector3.down, out hit, collider.height / 2f + 0.1f, m_WalkableLayer))
            {
                // Project the movement direction onto the plane of the ground
                moveDir = Vector3.ProjectOnPlane(moveDir, hit.normal).normalized;
            }
        }

        // Handle stairs BEFORE the movement calculation
        //HandleStairs();

        Vector3 velocity = m_RigidBody.linearVelocity;
        Vector3 targetVelocity = moveDir * m_MoveSpeed;
        //targetVelocity = transform.TransformDirection(targetVelocity);




        Vector3 velocityChange = targetVelocity - velocity;
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);

        // This line was the problem; ClampMagnitude returns a value, it doesn't modify the original variable.
        velocityChange = Vector3.ClampMagnitude(velocityChange, m_MaxSpeed);

        m_RigidBody.AddForce(velocityChange, ForceMode.VelocityChange);
        UpdateAnimator();
    }

    void Checkgrounded()
    {
        CapsuleCollider collider = m_Collider.GetComponent<CapsuleCollider>();

        float capsuleBaseY = transform.position.y + collider.center.y - collider.height / 2f;
        Vector3 capsuleBase = new Vector3(transform.position.x, capsuleBaseY + 0.05f, transform.position.z);
        m_IsGrounded = Physics.Raycast(capsuleBase, Vector3.down, 0.1f, m_WalkableLayer);
    }

    void UpdateAnimator()
    {
        
    }
}
