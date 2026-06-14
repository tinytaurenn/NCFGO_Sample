using Steamworks;
using System;
using System.Net;
using PurrNet;
using UnityEngine.Animations.Rigging;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : NetworkBehaviour
{
    InputSystem_Actions m_InputActions; // test 
    [Space(10)]
    [Header("Movement")]
    [Space(10)]
    [SerializeField] PlayerMovement m_PlayerMovement;
    [SerializeField] PlayerCamera m_PlayerCamera;
    [SerializeField] PlayerEntity m_PlayerEntity;
    [SerializeField] private Transform m_FootCameraAnchor; 
    [SerializeField] private Transform m_BicycleCameraAnchor;

    Vector2 m_MoveValue;
    [SerializeField] public Vehicule m_CurrentVehicule;

    [SerializeField] float m_UsableDistance = 3f; 
     IUsable m_UsableFocus;
    //to serialize
    [SerializeField] GameObject m_UsableFocusDebug; 
    // bicycle
    [Space(10)]
    [Header("IKs")]
    [Space(10)]
    [SerializeField] Transform RightHandIK;
    [SerializeField] Transform LeftHandIK;
    [SerializeField] TwoBoneIKConstraint m_RightHandIKConstraint;
    [SerializeField] TwoBoneIKConstraint m_LeftHandIKConstraint;
    //
    [SerializeField] Transform RightFootIK;
    [SerializeField] Transform LeftFootIK;
    [SerializeField] TwoBoneIKConstraint m_RightFootIKConstraint;
    [SerializeField] TwoBoneIKConstraint m_LeftFootIKConstraint;
    
    public SyncVar<float> GlobalIkWeight = new SyncVar<float>(ownerAuth : true);
    
    [Space(10)]
    [Header("Pause")]
    [Space(10)]
    //Pause
    
    [SerializeField] GameObject m_PauseMenuGO;
    [SerializeField] UI_Manager m_UI_Manager;
    bool IsPauseOpen = false;


    public enum ELocomotionState
    {
        Foot,
        Bicycle
    }
    [Space(10)]
    [Header("States")]
    [Space(10)]
    [SerializeField] ELocomotionState m_LocomotionState = ELocomotionState.Foot;

    private void Awake()
    {
        m_InputActions = new InputSystem_Actions();
        GlobalIkWeight.onChanged += OnGlobalIkWeightChanged; 
    }
    
    void Start()
    {
        
    }

    private void OnGlobalIkWeightChanged(float obj)
    {
        Debug.Log("Ik WeightChanged");
        m_RightHandIKConstraint.weight = obj;
        m_LeftHandIKConstraint.weight = obj;
        m_LeftFootIKConstraint.weight = obj;
        m_RightFootIKConstraint.weight = obj;
    }

    // Update is called once per frame
    void Update()
    {
        CheckUsable(); 
        LocomotionStateUpdate(); 

        //

    }

    private void FixedUpdate()
    {
        FixedLocomotionStateUpdate();
    }

    private void OnEnable()
    {
        m_InputActions.Player.Enable();
        m_InputActions.Player.Interact.performed += OnInteract; 
        m_InputActions.Player.Crouch.performed += OnCrouch; 
        
        m_InputActions.Player.Pause.performed += OnTogglePauseMenu;
        


    }

    private void OnCrouch(InputAction.CallbackContext obj)
    {
        if(m_CurrentVehicule)
        {
            m_PlayerEntity.ExitVehicle(); 
        }
    }

    private void OnTogglePauseMenu(InputAction.CallbackContext context)
    {
        
        TogglePauseMenu();
        
        
    }

    void TogglePauseMenu()
    {
        m_PauseMenuGO.SetActive(!IsPauseOpen);
        IsPauseOpen = !IsPauseOpen;
        Cursor.visible = IsPauseOpen; 
        Cursor.lockState = IsPauseOpen ? CursorLockMode.Confined : CursorLockMode.Locked;
    }

    public void PauseRespawnButton()
    {
        Debug.Log("pause respawn button");
        //transform.SetPositionAndRotation(GameManager.Instance.PlayerSpawnPoint.position, Quaternion.identity);
        TogglePauseMenu();
        transform.GetComponent<PlayerEntity>().PlayerRespawn();
    }
    
    public void PauseDisconnectButton()
    {
        //transform.SetPositionAndRotation(GameManager.Instance.PlayerSpawnPoint.position, Quaternion.identity);
        //TogglePauseMenu();
        transform.GetComponent<PlayerEntity>().PlayerDisconnect();
        //SceneUIManager.Instance.ConnectionWindow.SetActive(true);
        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.Confined;
        
    }
    


    private void OnDisable()
    {
        m_InputActions.Player.Interact.performed -= OnInteract;
        m_InputActions.Player.Pause.performed -= OnTogglePauseMenu;
        m_InputActions.Player.Crouch.performed -= OnCrouch; 
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

                
                //m_PlayerMovement.StopMovement();
                m_CurrentVehicule.m_VehicleMovement.braking = m_InputActions.Player.Jump.IsPressed();
                
                SetVehicleValue(m_InputActions.Player.Move.ReadValue<Vector2>());
                m_PlayerMovement.MouseDelta = m_InputActions.Player.Look.ReadValue<Vector2>();
                m_PlayerCamera.MouseDelta = m_InputActions.Player.Look.ReadValue<Vector2>();
                break;
            default:
                break;
        }
    }
    void FixedLocomotionStateUpdate()
    {
        switch (m_LocomotionState)
        {
            case ELocomotionState.Foot:
                
                break;
            case ELocomotionState.Bicycle:
                RightHandIK.transform.position = Vector3.Lerp(RightHandIK.transform.position, m_CurrentVehicule.HandAnchorRight.position, 15 * Time.fixedDeltaTime);
                LeftHandIK.transform.position = Vector3.Lerp(LeftHandIK.transform.position, m_CurrentVehicule.HandAnchorLeft.position, 15 * Time.fixedDeltaTime);
                RightHandIK.transform.rotation = Quaternion.Slerp(RightHandIK.transform.rotation, m_CurrentVehicule.HandAnchorRight.rotation, 15 * Time.fixedDeltaTime);
                LeftHandIK.transform.rotation = Quaternion.Slerp(LeftHandIK.transform.rotation, m_CurrentVehicule.HandAnchorLeft.rotation, 15 * Time.fixedDeltaTime);
                
                //foots
                
                RightFootIK.transform.position = Vector3.Lerp(RightFootIK.transform.position, m_CurrentVehicule.FootAnchorRight.position, 15 * Time.fixedDeltaTime);
                LeftFootIK.transform.position = Vector3.Lerp(LeftFootIK.transform.position, m_CurrentVehicule.FootAnchorLeft.position, 15 * Time.fixedDeltaTime);
                //RightFootIK.transform.rotation = Quaternion.Slerp(RightFootIK.transform.rotation, m_CurrentVehicule.transform.rotation, 15 * Time.fixedDeltaTime);
                //LeftFootIK.transform.rotation = Quaternion.Slerp(LeftFootIK.transform.rotation, m_CurrentVehicule.transform.rotation, 15 * Time.fixedDeltaTime);
                
                
                
                
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
                //bicycle
                
                //m_PlayerMovement.StopMovement();
                m_PlayerMovement.SetOnBike(true);
                
                
                
                GlobalIkWeight.value = 1f; 
           
                //
                
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

                if (m_CurrentVehicule)
                {
                    transform.position = m_CurrentVehicule.transform.position + m_CurrentVehicule.transform.right * 2f;
                    //m_CurrentVehicule.GetComponent<BoxCollider>().isTrigger = false;
                    //transform.rotation.SetLookRotation(m_CurrentVehicule.transform.position);
                }
                m_PlayerMovement.SetOnBike(false);
                GlobalIkWeight.value = 0f;
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
