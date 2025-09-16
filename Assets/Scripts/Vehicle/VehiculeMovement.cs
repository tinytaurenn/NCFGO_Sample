using JamesFrowen.SimpleWeb;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class VehiculeMovement : MonoBehaviour
{
    public Vector3 MoveInput { get; set; }
    public Vector3 MoveInputRaw { get; set; }
    public Vector3 PreviousMoveInputRaw { get; set; }

    [SerializeField] float m_MoveSpeed = 5f;
    [SerializeField] float m_MaxSpeed = 10f;
    [SerializeField] float m_BackWardspeedModifier = 0.5f;  
    [SerializeField] float m_RotationSpeed = 10f; 
    [SerializeField] LayerMask m_WalkableLayer;
    protected Rigidbody m_RigidBody;
    //[SerializeField] bool m_IsGrounded = true;
    [SerializeField] Transform m_ForwardPivot;

    [SerializeField] WheelCollider frontWheel;
    [SerializeField] WheelCollider backWheel;

    [SerializeField] Transform frontWheelTransform;
    [SerializeField] Transform backWheelTransform;
    [SerializeField] Transform m_Handle; 

    [SerializeField] bool braking = false; 
    [SerializeField] float brakeForce = 20; 

    [Space(20)]
    [Header("Steering")]
    [Space(5)]
    [Tooltip("Defines the maximum steering angle for the bicycle")]
    [SerializeField] float maxSteeringAngle;
    [Tooltip("Sets how current_MaxSteering is reduced based on the speed of the RB, (0 - No effect) (1 - Full)")]
    [Range(0f, 1f)][SerializeField] float steerReductorAmmount;
    [Tooltip("Sets the Steering sensitivity [Steering Stiffness] 0 - No turn, 1 - FastTurn)")]
    [Range(0.001f, 1f)][SerializeField] float turnSmoothing;

    [Space(20)]
    [Header("Lean")]
    [Space(5)]
    [Tooltip("Defines the maximum leaning angle for this bicycle")]
    [SerializeField] float maxLeanAngle = 45f;
    [Tooltip("Sets the Leaning sensitivity (0 - None, 1 - full")]
    [Range(0.001f, 1f)][SerializeField] float leanSmoothing;
    float targetLeanAngle;

    [Space(20)]
    [HeaderAttribute("Info")]
    [SerializeField] float currentSteeringAngle;
    [Tooltip("Dynamic steering angle baed on the speed of the RB, affected by sterReductorAmmount")]
    [SerializeField] float current_maxSteeringAngle;
    [Tooltip("The current lean angle applied")]
    [Range(-45, 45)] public float currentLeanAngle;
    [Space(20)]
    [HeaderAttribute("Speed")]
    [SerializeField] float currentSpeed;

    private void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();

    }
    void Start()
    {
        frontWheel.ConfigureVehicleSubsteps(5, 12, 15);
        backWheel.ConfigureVehicleSubsteps(5, 12, 15);
    }

    // Update is called once per frame
    void Update()
    {
        braking = Keyboard.current.spaceKey.isPressed;
    }
    protected virtual void FixedUpdate()
    {
        MovementUpdate();
    }
    protected virtual void MovementUpdate()
    {
        //Checkgrounded();


        Engine(); 

        Turning();
        LeanOnTurn();
        UpdateHandle(); 
        UpdateWheels(); //wheels transform cant be the one with weels colliders

        UpdateAnimator();
    }

    void Engine()
    {
        // Use a variable to store the motor torque, which is based on MoveInputRaw
        float motorTorque = braking ? 0f : MoveInputRaw.y * m_MoveSpeed; 

        // Apply motor torque to the back wheel
        // This is the ONLY place where movement force should be applied
        backWheel.motorTorque = motorTorque;

        float force = braking ? brakeForce : 0f;
        Braking(force); 
    }

    void Braking(float force)
    {
        frontWheel.brakeTorque = force;
        backWheel.brakeTorque = force;
    }
    void MaxTurning()
    {
        //30 is the value of MaxSpeed at wich currentMaxSteering will be at its minimum,			
        float turn = (m_RigidBody.linearVelocity.magnitude / 30) * steerReductorAmmount;
        turn = turn > 1 ? 1 : turn;
        current_maxSteeringAngle = Mathf.LerpAngle(maxSteeringAngle, 5, turn); //5 is the lowest posisble degrees of Steering	
    }

    public void Turning()
    {
        MaxTurning();

        currentSteeringAngle = Mathf.Lerp(currentSteeringAngle, current_maxSteeringAngle * MoveInputRaw.x, turnSmoothing * 0.1f);
        frontWheel.steerAngle = currentSteeringAngle;

        //We set the target lean angle to the + or - input value of our steering 
        //We invert our input for rotating in the ocrrect axis
        targetLeanAngle = maxLeanAngle * - MoveInputRaw.x;
    }

    private void LeanOnTurn()
    {
        Vector3 currentRot = transform.rotation.eulerAngles;
        //Case: not moving much		
        if (m_RigidBody.linearVelocity.magnitude < 1)
        {
            //Debug.Log("not moving much");
            currentLeanAngle = Mathf.LerpAngle(currentLeanAngle, 0f, 0.1f);
            transform.rotation = Quaternion.Euler(currentRot.x, currentRot.y, currentLeanAngle);
            //return;
        }
        //Case: Not steering or steering a tiny amount
        if (currentSteeringAngle < 0.5f && currentSteeringAngle > -0.5)
        {
            //Debug.Log("Not steering or steering a tiny amount");
            currentLeanAngle = Mathf.LerpAngle(currentLeanAngle, 0f, leanSmoothing * 0.1f);
        }
        //Case: Steering
        else
        {
            Debug.Log("Steering");
            currentLeanAngle = Mathf.LerpAngle(currentLeanAngle, targetLeanAngle, leanSmoothing * 0.1f);
            m_RigidBody.centerOfMass = new Vector3(m_RigidBody.centerOfMass.x, -0.1f, m_RigidBody.centerOfMass.z);
        }
        transform.rotation = Quaternion.Euler(currentRot.x, currentRot.y, currentLeanAngle);
    }
    void UpdateHandle()
    {
       
        m_Handle.localEulerAngles = new Vector3(m_Handle.localEulerAngles.x, currentSteeringAngle, m_Handle.localEulerAngles.z);
    }
    public void UpdateWheels()
    {
        UpdateSingleWheel(frontWheel, frontWheelTransform);
        UpdateSingleWheel(backWheel, backWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 position;
        Quaternion rotation;
        wheelCollider.GetWorldPose(out position, out rotation);// this !! 
        Vector3 newRot = rotation.eulerAngles;
        newRot.z += 90;


        wheelTransform.rotation = Quaternion.Euler(newRot); 




        wheelTransform.position = position;
    }

    void Checkgrounded()
    {
        
    }

    void UpdateAnimator()
    {
        
    }
    private void OnDrawGizmos()
    {
       
    }
}
