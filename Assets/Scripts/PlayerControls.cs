using Steamworks;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    InputSystem_Actions m_InputActions;
    [SerializeField] PlayerMovement m_PlayerMovement;
    [SerializeField] PlayerCamera m_PlayerCamera; 

    Vector2 m_MoveValue;

    private void Awake()
    {
        m_InputActions = new InputSystem_Actions();
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        SetMovementValue(m_InputActions.Player.Move.ReadValue<Vector2>());
        m_PlayerMovement.MouseDelta = m_InputActions.Player.Look.ReadValue<Vector2>();
        m_PlayerCamera.MouseDelta = m_InputActions.Player.Look.ReadValue<Vector2>();
        SetIsSprinting(m_InputActions.Player.Sprint.IsPressed());
    }
    private void OnEnable()
    {
        m_InputActions.Player.Enable();


    }
    private void OnDisable()
    {
        m_InputActions.Player.Disable();
        
    }

    void SetMovementValue(Vector2 moveInput)
    {
        
        m_MoveValue = moveInput;

        //Debug.Log("Movement Input: " + moveInput);
        Vector3 forward = m_PlayerCamera.transform.parent.forward; 
        Vector3 right = m_PlayerCamera.transform.parent.right;
        if(m_MoveValue.y <0)
        {
            m_MoveValue.y *= m_PlayerMovement.BackWardspeedModifier; 
        }
        m_PlayerMovement.MoveInput = right * (m_MoveValue.x * m_PlayerMovement.StrafeSpeedModifier) + forward * m_MoveValue.y;
        m_PlayerMovement.MoveInputRaw = moveInput;

        m_PlayerMovement.MoveInput = right * m_MoveValue.x + forward * m_MoveValue.y;



    }

    void SetIsSprinting(bool isSprinting)
    {
        m_PlayerMovement.IsSprinting = isSprinting;
    }
}
