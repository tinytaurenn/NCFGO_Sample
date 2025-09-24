using Steamworks;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    InputSystem_Actions m_InputActions;
    [SerializeField] PlayerMovement m_PlayerMovement;
    [SerializeField] PlayerCamera m_PlayerCamera; 

    Vector2 m_MoveValue;
    [SerializeField] public Vehicule m_CurrentVehicule;

    [SerializeField] float m_UsableDistance = 3f; 
     IUsable m_UsableFocus;
    //to serialize
    [SerializeField] GameObject m_UsableFocusDebug; 


    public enum ELocomotionState
    {
        Foot,
        Bicycle
    }
    [SerializeField] ELocomotionState m_LocomotionState = ELocomotionState.Foot;

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
        CheckUsable(); 
        LocomotionStateUpdate(); 

        //

    }
    private void OnEnable()
    {
        m_InputActions.Player.Enable();
        m_InputActions.Player.Interact.performed += OnInteract; 


    }


    private void OnDisable()
    {
        m_InputActions.Player.Interact.performed -= OnInteract;
        m_InputActions.Player.Disable();

    }
    private void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("try using something"); 
        if(m_CurrentVehicule != null)
        {
            Debug.Log("can't use while in vehicle");
            return; 
        }
        m_UsableFocus?.TryUse(); 
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
    void SetVehicleValue(Vector2 moveInput)
    {
        if (m_CurrentVehicule == null) return; 
        m_MoveValue = moveInput;

        //Debug.Log("Movement Input: " + moveInput);
        Vector3 forward = m_PlayerCamera.transform.parent.forward;
        Vector3 right = m_PlayerCamera.transform.parent.right;
        m_CurrentVehicule.m_VehicleMovement.MoveInput =  forward * m_MoveValue.y;
        m_CurrentVehicule.m_VehicleMovement.MoveInputRaw = moveInput;

        m_CurrentVehicule.m_VehicleMovement.MoveInput = right * m_MoveValue.x + forward * m_MoveValue.y;
    }

    void SetIsSprinting(bool isSprinting)
    {
        m_PlayerMovement.IsSprinting = isSprinting;
    }

    public void SwitchLocomotionState(ELocomotionState newState)
    {
        if (m_LocomotionState == newState) return;
        OnExitLocomotionState();
        m_LocomotionState = newState;
        OnEnterLocomotionState();

    }
    public void SwitchToBicycle(Vehicule vehicule)
    {
        m_CurrentVehicule = vehicule;
        SwitchLocomotionState(ELocomotionState.Bicycle);
    }
    void LocomotionStateUpdate()
    {
        switch (m_LocomotionState)
        {
            case ELocomotionState.Foot:
                SetMovementValue(m_InputActions.Player.Move.ReadValue<Vector2>());
                m_PlayerMovement.MouseDelta = m_InputActions.Player.Look.ReadValue<Vector2>();
                m_PlayerCamera.MouseDelta = m_InputActions.Player.Look.ReadValue<Vector2>();
                SetIsSprinting(m_InputActions.Player.Sprint.IsPressed());
                break;
            case ELocomotionState.Bicycle:
                SetVehicleValue(m_InputActions.Player.Move.ReadValue<Vector2>());
                m_PlayerMovement.MouseDelta = m_InputActions.Player.Look.ReadValue<Vector2>();
                m_PlayerCamera.MouseDelta = m_InputActions.Player.Look.ReadValue<Vector2>();
                break;
            default:
                break;
        }
    }
    void OnEnterLocomotionState()
    {
        switch (m_LocomotionState)
        {
            case ELocomotionState.Foot:
                m_CurrentVehicule = null;
                break;
            case ELocomotionState.Bicycle:
                //Physics.IgnoreCollision(m_CurrentVehicule.GetComponent<Collider>(), GetComponent<Collider>(), true);
                
                break;
            default:
                break;
        }
    }
    void OnExitLocomotionState()
    {
        switch (m_LocomotionState)
        {
            case ELocomotionState.Foot:
                break;
            case ELocomotionState.Bicycle:
                break;
            default:
                break;
        }
    }

    void CheckUsable()
    {
        Ray ray = new Ray(m_PlayerCamera.transform.position, m_PlayerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, m_UsableDistance))
        {
            //Debug.Log("hitting something : " + hitInfo.collider.name);

            if (hitInfo.collider.TryGetComponent<IUsable>(out IUsable usable))
            {
                m_UsableFocus = usable;
                m_UsableFocusDebug = hitInfo.collider.gameObject; 
                //Debug.Log("hitting usable : " + hitInfo.collider.name);
                UI_Manager.Instance.SetUseText("Press Numpad 3 to use " + hitInfo.collider.transform.root.name);
                UI_Manager.Instance.ShowText(true);
                return;
            }
        }
        m_UsableFocus = null;   
        m_UsableFocusDebug = null;
        UI_Manager.Instance.ShowText(false);
    }
}
