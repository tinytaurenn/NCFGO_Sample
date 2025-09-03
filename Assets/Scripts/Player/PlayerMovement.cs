using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : CharacterMovement
{
    
    public Vector2 MouseDelta { get; set; }
    public float m_MouseSensivity = 1f; // mouse sensitivity

    [Space(10)]
    [Header("player movement speeds ")]
    [SerializeField] float m_BackWardspeedModifier = 0.5f;
    [SerializeField] float m_StrafeSpeedModifier = 0.75f;

    public float BackWardspeedModifier => m_BackWardspeedModifier;
    public float StrafeSpeedModifier => m_StrafeSpeedModifier;

    [Space(10)]
    [Header("Player Look ")]
    [SerializeField] float m_LookRange = 1f;
    [SerializeField] float m_LookValue = 0f;
    public float LookValue => m_LookValue;
    [SerializeField] float m_VerticalLookSensivity = 0.5f;


    [SerializeField] Transform m_NeckTransform;
    Quaternion m_WorldLookRotation;
    //[SerializeField] PlayerCamera m_PlayerCamera; 

    protected override void Awake()
    {
        base.Awake();
        m_WorldLookRotation = m_NeckTransform.rotation;
    }
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

    }
    protected override void MovementUpdate()
    {
        base.MovementUpdate();
        
        if(IsLocked) return;

        m_WorldLookRotation = Quaternion.Slerp(m_WorldLookRotation, m_WorldLookRotation * Quaternion.Euler(0 , MouseDelta.x * m_MouseSensivity, 0), Time.fixedDeltaTime * 10f);

        if (m_HorizontalVelocity.magnitude > 0.1)
        {
            Quaternion bodyTargetRotation = Quaternion.Euler(0, m_WorldLookRotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, bodyTargetRotation, Time.fixedDeltaTime * m_RotationSpeed);

            
        }

        Quaternion localTargetRot = Quaternion.Inverse(m_NeckTransform.parent.rotation) * m_WorldLookRotation;
        m_NeckTransform.localRotation = localTargetRot;
        //m_PlayerCamera.LookPos = m_NeckTransform.position + (m_WorldLookRotation * Vector3.forward * 10f); 




    }
    private void OnDrawGizmos()
    {
        //draw a spherewith  world look direction at 10 distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(m_NeckTransform.position + (m_WorldLookRotation * Vector3.forward * 10f), 0.2f);
       
        
    }

}
