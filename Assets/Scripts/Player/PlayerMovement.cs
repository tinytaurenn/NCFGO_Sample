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

    [SerializeField] float m_LookRange = 1f;
    [SerializeField] float m_LookValue = 0f;
    public float LookValue => m_LookValue;
    [SerializeField] float m_VerticalLookSensivity = 0.5f;

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

        Quaternion rot = Quaternion.LookRotation(m_HorizontalVelocity, Vector3.up);
        Quaternion newRot = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * m_RotationSpeed);

        if (m_HorizontalVelocity.magnitude > 0.1) transform.rotation = newRot; 

    }
}
