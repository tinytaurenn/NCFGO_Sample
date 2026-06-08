using Steamworks;
using System;
using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    InputSystem_Actions m_InputActions; // test 
    [SerializeField] PlayerMovement m_PlayerMovement;
    [SerializeField] PlayerCamera m_PlayerCamera;
    [SerializeField] private Transform m_FootCameraAnchor; 
    [SerializeField] private Transform m_BicycleCameraAnchor;

    Vector2 m_MoveValue;
    [SerializeField] public Vehicule m_CurrentVehicule;

    [SerializeField] float m_UsableDistance = 3f; 
     IUsable m_UsableFocus;
    //to serialize
    [SerializeField] GameObject m_UsableFocusDebug; 
    
    //Pause
    
    bool IsPauseOpen = false;
    [SerializeField] GameObject m_PauseMenuGO;
    [SerializeField] UI_Manager m_UI_Manager;


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
        m_InputActions.Player.Pause.performed += OnTogglePauseMenu;


    }

    private void OnTogglePauseMenu(InputAction.CallbackContext context)
    {
        
        TogglePauseMenu();
        
        
    }

    void TogglePauseMenu()
    {
        m_PauseMenuGO.SetActive(!IsPauseOpen);
        IsPauseOpen = !IsPauseOpen;
    }

    public void PauseRespawnButton()
    {
        Debug.Log("pause respawn button");
        //transform.SetPositionAndRotation(GameManager.Instance.PlayerSpawnPoint.position, Quaternion.identity);
        TogglePauseMenu();
        transform.GetComponent<PlayerEntity>().PlayerRespawn();
    }


    private void OnDisable()
    {
        m_InputActions.Player.Interact.performed -= OnInteract;
        m_InputActions.Player.Pause.performed -= OnTogglePauseMenu;
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
        if(m_UsableFocus == null)
        {
            Debug.Log("no usableFocus");
            return; 
        }
        m_UsableFocus?.TryUse(transform.GetComponent<PlayerEntity>()); 
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
        if (!m_CurrentVehicule ) return; 
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
                m_PlayerMovement.StopMovement();
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

                
                transform.SetParent(null,true);
                if (m_CurrentVehicule)
                {
                    m_CurrentVehicule.HasDriver.value = false;
                    //m_CurrentVehicule.RemoveOwnership();
                }
                transform.GetComponent<Rigidbody>().isKinematic = false;
                m_PlayerCamera.transform.SetParent(m_FootCameraAnchor, false);
                m_CurrentVehicule = null;
                m_PlayerMovement.CurrentVehicule = null; 
                //m_PlayerMovement.enabled = true;
                break;
            case ELocomotionState.Bicycle:
                transform.GetComponent<Rigidbody>().isKinematic = true;
                m_PlayerCamera.transform.SetParent(m_BicycleCameraAnchor, false);
                m_PlayerMovement.CurrentVehicule = m_CurrentVehicule;
                //m_PlayerMovement.enabled = false;
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
            if (m_CurrentVehicule && hitInfo.transform.gameObject == m_CurrentVehicule.gameObject)
            {
                m_UsableFocus = null;   
                m_UsableFocusDebug = null;
                m_UI_Manager.ShowText(false);
                return; 
            }
            if (hitInfo.collider.TryGetComponent<IUsable>(out IUsable usable))
            {
                m_UsableFocus = usable;
                m_UsableFocusDebug = hitInfo.collider.gameObject; 
                //Debug.Log("hitting usable : " + hitInfo.collider.name);
                m_UI_Manager.SetUseText("E : Use  " + hitInfo.collider.transform.root.name);
                m_UI_Manager.ShowText(true);
                return;
            }
        }
        m_UsableFocus = null;   
        m_UsableFocusDebug = null;
        m_UI_Manager.ShowText(false);
    }
}
