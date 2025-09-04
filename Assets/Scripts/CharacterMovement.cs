using PurrNet;
using UnityEngine;

public abstract class CharacterMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected Rigidbody m_rigidBody;
    [SerializeField] protected NetworkAnimator m_Animator;

    public Vector3 MoveInput { get; set; }
    public Vector3 MoveInputRaw { get; set; }
    public Vector3 PreviousMoveInputRaw { get; set; }
    public bool IsSprinting { get; set; }

    public bool IsLocked { get; private set; }

    [Header("Horizontal parameters")]
    [SerializeField] float m_MovementSpeed = 7f;
    [SerializeField] float m_SprintMultiplier = 1.5f;
    [SerializeField] protected float m_RotationSpeed = 12f;
    [SerializeField] float m_Acceleration = 10f;
    [SerializeField] float m_Deceleration = 15f;
    [SerializeField] float m_AirAcceleration = 5f;
    [SerializeField] float m_AirDecceleration = 1f;

    protected Vector3 m_HorizontalVelocity;
    Vector3 m_PreviousMoveInput;
    float m_PreviousMagnitude;
    float m_CurrentSpeed;

    [Header("Jump Parameters")]
    [SerializeField] float m_Weight = 35f;
    [SerializeField] float m_JumpLength = 0.15f;
    [SerializeField] float m_JumpForce = 20f;
    [SerializeField] float m_JumpCut = 0.5f;
    [SerializeField] float m_AirTime = 1.125f;
    [SerializeField] float m_MaxFallSpeed = 20f;

    float m_JumpTimer = 0f;
    float m_TimeSincegrounded = 0f;
    [SerializeField] internal bool m_IsFalling = false;
    [SerializeField] internal bool m_Isgrounded = true;
    Vector3 m_VerticalVelocity;

    [Header("Spring")]
    [SerializeField] LayerMask m_WalkableLayer;
    [SerializeField] float m_SpringHeight = 1f;
    [SerializeField] float m_SpringForce = 40f;
    [SerializeField] float m_RayCastHeight = 1.3f;
    [SerializeField] float m_Damping = 0.01f;

    Vector3 m_SpringVector;

    Transform m_Platform;
    bool m_IsOnPlatform = false;
    Vector3 m_PushBackVelocity;

    Vector3 m_BumpVelocity;

    [Header("Slip")]

    [SerializeField] float m_SlipCheckDistance = 0.3f;
    [SerializeField] float m_SlipRayDistance = 0f;

    [Header("Step")]
    Vector3 m_StepRay = Vector3.zero;
    [SerializeField] bool m_IsStepping = false;
    [SerializeField] float m_StepHeight = 0.1f;
    [SerializeField] float m_StepHeightSmooth = 0.1f;
    [SerializeField] float m_StepMoveHeight = 0.65f;
    [SerializeField] float m_StepDistance = 1f;
    [SerializeField] float m_StepSpeed = 5f;
    [SerializeField] float m_StepAngle = 45f;




    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }
    protected virtual void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody>();

    }
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {



        JumpTick();
        m_rigidBody.linearVelocity += Vector3.up * 10;

        AvoidBorderStuck();



    }


    protected virtual void FixedUpdate()
    {
        //Debug.Log("vertical velocity is : " + m_VerticalVelocity.magnitude);
        MovementUpdate();

    }

    protected virtual void MovementUpdate()
    {
        Vector3 rigidBodyVelocity = m_rigidBody.linearVelocity;
        VerticalMovement(rigidBodyVelocity);

        float magnitude = MoveInput.magnitude;

        if (IsLocked && magnitude > 0.2f && m_Animator.GetBool("Seated"))
        {
            m_Animator.SetBool("Seated", false);
            LockMovement(false);


        }

        float acceleration = 0f;
        if (magnitude > m_PreviousMagnitude)
        {
            if (m_Isgrounded)
            {
                acceleration = m_Acceleration;
            }
            else
            {
                acceleration = m_AirAcceleration;
            }
        }
        else
        {
            if (m_Isgrounded)
            {
                acceleration = m_Deceleration;
            }
            else
            {
                acceleration = m_AirDecceleration;
            }
        }

        Vector3 vectorDelta = Vector3.Lerp(m_PreviousMoveInput, MoveInput, Time.deltaTime * acceleration);
        float magnitudeDelta = vectorDelta.magnitude;

        m_PushBackVelocity *= .8f;

        m_CurrentSpeed = IsSprinting ? m_MovementSpeed * m_SprintMultiplier : m_MovementSpeed;

        m_HorizontalVelocity = (vectorDelta) * m_CurrentSpeed;

        //STEPS

        Vector3 stepPos = transform.position + Vector3.up * m_StepMoveHeight + MoveInput * 1;
        Vector3 stepCheck = transform.position + Vector3.up * 0.1f + MoveInput * 1;
        m_StepRay = (stepPos - transform.position).normalized;
        Vector3 checkRay = (stepCheck - transform.position).normalized;

        if (m_IsStepping = StepCheck(out m_StepRay, out float stepHeight))
        {
            if (stepHeight <= m_StepHeight)
            {
                m_rigidBody.position += Vector3.up * m_StepHeightSmooth;
            }
            m_HorizontalVelocity += m_StepRay * m_StepSpeed;
        }
        //


        //next frame
        m_PreviousMoveInput = vectorDelta;
        m_PreviousMagnitude = magnitudeDelta;

        //Capping
        if (m_IsFalling && m_VerticalVelocity.magnitude > m_MaxFallSpeed)
            m_VerticalVelocity = m_VerticalVelocity.normalized * m_MaxFallSpeed;


        m_rigidBody.linearVelocity = m_HorizontalVelocity + m_VerticalVelocity;

        if (!m_Isgrounded) m_VerticalVelocity = Vector3.Project(m_rigidBody.linearVelocity, Vector3.up);


        //deprecated
        //if (magnitude > 0f)
        //{
        //    Quaternion newRotation = Quaternion.LookRotation(MoveInput);
        //    transform.rotation = Quaternion.Lerp(m_rigidBody.rotation, newRotation, Time.deltaTime * m_RotationSpeed);
        //}



        UpdateAnimator();

    }



    void VerticalMovement(Vector3 velocity)
    {
        float rayDistance = m_Isgrounded ? m_RayCastHeight : m_SpringHeight;

        if (m_JumpTimer > 0)
        {
            ApplyGravity();
        }
        else
        {

            //ground check
            bool wasGrounded = m_Isgrounded;
            Ray groundRay = new Ray(transform.position + Vector3.up, Vector3.down);
            //QueryTriggerInteraction.Ignore is for triggers 
            m_Isgrounded = Physics.Raycast(groundRay, out RaycastHit raycast, rayDistance, m_WalkableLayer, QueryTriggerInteraction.Ignore);


            if (m_JumpTimer > 0)
            {
                m_Isgrounded = false;
            }
            if (m_Isgrounded)
            {
                if (!wasGrounded || m_Platform == null)
                {
                    if (raycast.rigidbody != null)
                    {
                        m_IsOnPlatform = raycast.rigidbody.TryGetComponent(out Transform platform);
                        m_Platform = m_IsOnPlatform ? platform : null;
                    }
                    if (m_IsOnPlatform) GetOnPlatform();
                }
                //spring
                float delta = raycast.distance - m_SpringHeight;
                float spring = delta * m_SpringForce - -velocity.y * m_Damping;

                m_SpringVector = spring * groundRay.direction;
                m_VerticalVelocity = Vector3.Lerp(m_VerticalVelocity, m_SpringVector, Time.deltaTime * 20f);

                m_TimeSincegrounded = 0;


            }
            else
            {
                //fall
                if (wasGrounded) Leaveground();
                ApplyGravity();

            }
        }

    }

    public virtual void StopMovement()
    {
        MoveInput = Vector3.zero;
        m_HorizontalVelocity = Vector3.zero;
        m_VerticalVelocity = Vector3.zero;
        m_rigidBody.linearVelocity = Vector3.zero;
    }

    private void Leaveground()
    {
        m_Isgrounded = false;
        if (m_IsOnPlatform)
        {
            transform.SetParent(null, true);
            m_Platform = null;
            m_IsOnPlatform = false;
        }
    }

    private void ApplyGravity()
    {
        m_VerticalVelocity += Vector3.up * (-m_Weight * Time.deltaTime);
        m_TimeSincegrounded += Time.deltaTime;
    }

    internal void TryJump()
    {
        Debug.Log("try Jumping");
        bool canJump = m_Isgrounded || m_TimeSincegrounded < m_AirTime;
        if (canJump && m_JumpTimer <= 0) Jump();

    }

    private void Jump()
    {
        Debug.Log("Jumping");

        if (m_Isgrounded)
        {
            if (m_Animator == null) return;
            m_Animator.SetTrigger("Jump");
            //if (m_sync != null) m_sync.SendCommand<Animator>(nameof(Animator.SetTrigger), MessageTarget.Other, "Jump");
        }
        m_JumpTimer = m_JumpLength;

        Leaveground();

        m_VerticalVelocity = Vector3.up * m_JumpForce;
        Vector3 velocity = m_rigidBody.linearVelocity;
        velocity -= Vector3.Project(velocity, Vector3.up);
        velocity += m_VerticalVelocity;
        m_rigidBody.linearVelocity = velocity;
    }
    private void JumpTick()
    {

        if (m_JumpTimer > 0)
        {
            m_JumpTimer -= Time.deltaTime;
            if (m_JumpTimer < 0f) InterrupJump();


        }
        m_IsFalling = !m_Isgrounded && Vector3.Dot(m_rigidBody.linearVelocity, Vector3.up) < 0;


    }


    internal void InterrupJump()
    {
        m_JumpTimer = -1;
        m_VerticalVelocity *= m_JumpCut;

    }

    public void Bump(Vector3 normalizedDirection, float force)
    {


        Leaveground();
        m_BumpVelocity = new(normalizedDirection.x, 0, normalizedDirection.z);
        m_BumpVelocity *= force;

        if (m_Isgrounded)
        {
            m_VerticalVelocity = Vector3.up * force;

        }
        else
        {
            m_VerticalVelocity = Vector3.up * force / 3f;
        }

        Vector3 velocity = m_rigidBody.linearVelocity;
        velocity -= Vector3.Project(velocity, Vector3.up);
        velocity += m_VerticalVelocity;
        m_rigidBody.linearVelocity = velocity;



    }

    public virtual void LockMovement(bool locked)
    {

        m_rigidBody.constraints = locked ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.FreezeRotation;
        IsLocked = locked;

    }
    public virtual void SitOnTarget(Transform target)
    {
        StopMovement();
        LockMovement(true);
        transform.position = target.position;
        transform.rotation = target.rotation;
        m_Animator.SetBool("Seated", true);
    }

    public void Stun()
    {
        m_Animator.SetBool("Stunned", true);

    }

    public void UnStun()
    {
        m_Animator.SetBool("Stunned", false);
    }

    void GetOnPlatform()
    {
        transform.SetParent(m_Platform, true);
    }
    public float ThrowSpeed()
    {
        float throwSpeed = Vector3.Dot(transform.forward, m_HorizontalVelocity);
        throwSpeed = Mathf.Clamp(throwSpeed, 0f, Mathf.Infinity);
        return throwSpeed;
    }

    public void ThrowPushBack(float speed)
    {
        m_PushBackVelocity += -transform.forward * speed * 0.6f;
    }

    private void UpdateAnimator()
    {
        //Debug.Log("magnitude : " + m_HorizontalVelocity.magnitude + " current speed : " + m_CurrentSpeed);

        float animSpeed = m_HorizontalVelocity.magnitude / m_CurrentSpeed;
        if (m_IsStepping) animSpeed = (((m_HorizontalVelocity - m_StepRay * m_StepSpeed).magnitude) / m_CurrentSpeed);
        if (IsSprinting) animSpeed *= m_SprintMultiplier;

        if (animSpeed < 0.01f)
        {
            animSpeed = 0f; // avoid animator to read extreme values

        }
        //Debug.Log("moveinput is" + MoveInput.ToString());
        Vector2 newMoveInput = Vector2.Lerp(PreviousMoveInputRaw, MoveInputRaw, Time.deltaTime * 5f);
        if(newMoveInput.magnitude < 0.01f) newMoveInput = Vector2.zero;
        PreviousMoveInputRaw = newMoveInput;
        if(IsSprinting && newMoveInput.y > 0.5f) newMoveInput *= m_SprintMultiplier;
        m_Animator.SetFloat("VelocityZ", newMoveInput.y);
        m_Animator.SetFloat("VelocityX", newMoveInput.x);
        
           
        //m_Animator.SetFloat("MoveSpeed", animSpeed);
        //m_Animator.SetBool("Grounded", m_Isgrounded);

    }



    void AvoidBorderStuck()
    {
        if (m_Isgrounded) return;
        if (m_JumpTimer > 0f) return;

        if (SlipCheck(out Vector3 direction))
        {
            Bump(direction, 20f);
        }

    }


    bool SlipCheck(out Vector3 direction)
    {
        direction = Vector3.up;


        Vector3 raySpawnPos = transform.position + Vector3.down * m_SlipRayDistance;

        Vector3 forward = transform.forward * m_SlipCheckDistance;
        Vector3 back = -transform.forward * m_SlipCheckDistance;
        Vector3 right = transform.right * m_SlipCheckDistance;
        Vector3 left = -transform.right * m_SlipCheckDistance;

        Ray frontRay = new Ray(raySpawnPos, forward);
        Ray backRay = new Ray(raySpawnPos, back);
        Ray rightRay = new Ray(raySpawnPos, right);
        Ray leftRay = new Ray(raySpawnPos, left);

        Ray frontDownRay = new Ray(raySpawnPos + forward, Vector3.down);
        Ray backDownRay = new Ray(raySpawnPos + back, Vector3.down);
        Ray rightDownRay = new Ray(raySpawnPos + right, Vector3.down);
        Ray leftDownRay = new Ray(raySpawnPos + left, Vector3.down);


        if (Physics.Raycast(frontRay, m_SlipCheckDistance, m_WalkableLayer))
        {
            direction = back;
            Debug.Log("slipping backward");
            return true;
        }
        if (Physics.Raycast(backRay, m_SlipCheckDistance, m_WalkableLayer))
        {
            direction = forward;
            Debug.Log("slipping forward");
            return true;
        }
        if (Physics.Raycast(rightRay, m_SlipCheckDistance, m_WalkableLayer))
        {
            direction = left;
            Debug.Log("slipping left");
            return true;
        }
        if (Physics.Raycast(leftRay, m_SlipCheckDistance, m_WalkableLayer))
        {
            direction = right;
            Debug.Log("slipping right");
            return true;
        }

        if (Physics.Raycast(frontDownRay, m_SlipCheckDistance, m_WalkableLayer))
        {
            m_Isgrounded = true;
            return false;
        }
        if (Physics.Raycast(backDownRay, m_SlipCheckDistance, m_WalkableLayer))
        {
            m_Isgrounded = true;
            return false;
        }
        if (Physics.Raycast(rightDownRay, m_SlipCheckDistance, m_WalkableLayer))
        {
            m_Isgrounded = true;
            return false;
        }
        if (Physics.Raycast(leftDownRay, m_SlipCheckDistance, m_WalkableLayer))
        {
            m_Isgrounded = true;
            return false;
        }



        return false;
    }

    bool StepCheck(out Vector3 ray, out float stepHeight)
    {
        Vector3 stepCheck = transform.position + Vector3.up * 0.1f + MoveInput;
        Vector3 checkRay = (stepCheck - transform.position).normalized;
        Vector3 wallCheck = transform.position + Vector3.up * m_StepHeight + MoveInput;
        Vector3 wallRay = (wallCheck - transform.position).normalized;
        ray = checkRay;
        stepHeight = 0f;

        if (!Physics.Raycast(transform.position, checkRay, m_StepDistance, m_WalkableLayer))
        {

            return false;
        }
        if (Physics.Raycast(transform.position, wallRay, m_StepDistance, m_WalkableLayer))
        {
            //Debug.Log("wall rayed"); 
            return false;
        }


        for (float i = 0.05f; i <= m_StepMoveHeight; i += 0.05f)
        {
            stepCheck = transform.position + Vector3.up * i + MoveInput;
            checkRay = (stepCheck - transform.position).normalized;


            if (!Physics.Raycast(transform.position, checkRay, m_StepDistance, m_WalkableLayer) && MoveInput.magnitude > 0.2f)
            {
                stepHeight = i;
                ray = checkRay;
                return true;
            }

        }
        for (float i = 0.05f; i <= m_StepMoveHeight; i += 0.05f)
        {
            stepCheck = transform.position + Vector3.up * i + MoveInput;

            checkRay = (stepCheck - transform.position).normalized;
            Vector3 rightCheckRay = Quaternion.Euler(0f, m_StepAngle, 0f) * checkRay;

            if (!Physics.Raycast(transform.position, rightCheckRay, m_StepDistance, m_WalkableLayer) && MoveInput.magnitude > 0.2f)
            {
                stepHeight = i;
                ray = rightCheckRay;
                return true;
            }

        }
        for (float i = 0.05f; i <= m_StepMoveHeight; i += 0.05f)
        {
            stepCheck = transform.position + Vector3.up * i + MoveInput;

            checkRay = (stepCheck - transform.position).normalized;
            Vector3 leftCheckRay = Quaternion.Euler(0f, -m_StepAngle, 0f) * checkRay;

            if (!Physics.Raycast(transform.position, leftCheckRay, m_StepDistance, m_WalkableLayer) && MoveInput.magnitude > 0.2f)
            {
                stepHeight = i;
                ray = leftCheckRay;
                return true;
            }

        }


        return false;

    }


    private void OnDrawGizmos()
    {

        BorderDetectionGizmos();




    }

    void BorderDetectionGizmos()
    {
        //Vector3 raySpawnPos = transform.position + Vector3.down * m_SlipRayDistance;

        //Vector3 forward = transform.forward * m_SlipCheckDistance;
        //Vector3 back = -transform.forward * m_SlipCheckDistance;
        //Vector3 right = transform.right * m_SlipCheckDistance;
        //Vector3 left = -transform.right * m_SlipCheckDistance;

        //Ray frontRay = new Ray(raySpawnPos, forward);
        //Ray backRay = new Ray(raySpawnPos, back);
        //Ray rightRay = new Ray(raySpawnPos, right);
        //Ray leftRay = new Ray(raySpawnPos, left);

        //Ray frontDownRay = new Ray(raySpawnPos + forward, Vector3.down);
        //Ray backDownRay = new Ray(raySpawnPos + back, Vector3.down);
        //Ray rightDownRay = new Ray(raySpawnPos + right, Vector3.down);
        //Ray leftDownRay = new Ray(raySpawnPos + left, Vector3.down);




        //Gizmos.color = Color.yellow;

        //Gizmos.color = (Physics.Raycast(frontRay, m_SlipCheckDistance, m_WalkableLayer)) ? Color.green : Color.yellow;
        //Gizmos.DrawRay(raySpawnPos, forward);
        //Gizmos.color = (Physics.Raycast(backRay, m_SlipCheckDistance, m_WalkableLayer)) ? Color.green : Color.yellow;
        //Gizmos.DrawRay(raySpawnPos, back);
        //Gizmos.color = (Physics.Raycast(rightRay, m_SlipCheckDistance, m_WalkableLayer)) ? Color.green : Color.yellow;
        //Gizmos.DrawRay(raySpawnPos, right);
        //Gizmos.color = (Physics.Raycast(leftRay, m_SlipCheckDistance, m_WalkableLayer)) ? Color.green : Color.yellow;
        //Gizmos.DrawRay(raySpawnPos, left);

        ////downs 

        //Gizmos.color = (Physics.Raycast(frontDownRay, m_SlipCheckDistance, m_WalkableLayer)) ? Color.green : Color.yellow;
        //Gizmos.DrawRay(raySpawnPos + forward, Vector3.down);
        //Gizmos.color = (Physics.Raycast(backDownRay, m_SlipCheckDistance, m_WalkableLayer)) ? Color.green : Color.yellow;
        //Gizmos.DrawRay(raySpawnPos + back, Vector3.down);
        //Gizmos.color = (Physics.Raycast(rightDownRay, m_SlipCheckDistance, m_WalkableLayer)) ? Color.green : Color.yellow;
        //Gizmos.DrawRay(raySpawnPos + right, Vector3.down);
        //Gizmos.color = (Physics.Raycast(leftDownRay, m_SlipCheckDistance, m_WalkableLayer)) ? Color.green : Color.yellow;
        //Gizmos.DrawRay(raySpawnPos + left, Vector3.down);

        //Gizmos.color = Color.blue;

        //

        //if(StepCheck(out Vector3 ray,out float stepHeight))
        //{
        //    Debug.Log("step height is : " + stepHeight);
        //    Gizmos.DrawRay(transform.position, ray);
        //}




        Gizmos.color = Color.magenta;


        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, MoveInput);
    }
}
