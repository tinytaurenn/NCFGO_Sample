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

    protected override void Awake()
    {
        base.Awake();
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

        float mouseDeltaX = MouseDelta.x * m_MouseSensivity;

        Quaternion newRotation = Quaternion.Euler(0, mouseDeltaX, 0);
        m_NeckTransform.rotation = Quaternion.Lerp(m_NeckTransform.rotation, m_NeckTransform.rotation * newRotation, Time.fixedDeltaTime * m_RotationSpeed);

        //return;
        if (m_HorizontalVelocity.magnitude < 0.1) return; 


        Quaternion bodyTargetRotation = Quaternion.Euler(0, m_NeckTransform.eulerAngles.y, 0);
        Quaternion slepRot = Quaternion.Slerp(transform.rotation, bodyTargetRotation, Time.fixedDeltaTime * m_RotationSpeed);

        // Interpoler la rotation du corps vers la rotation de la tête.
        transform.rotation = slepRot;




    }
}
