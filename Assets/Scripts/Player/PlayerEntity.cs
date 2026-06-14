using PurrNet;
using PurrNet.Transports;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerEntity : PlayerIdentity<PlayerEntity>
{
    PlayerControls m_PlayerControls;
    
    Rigidbody m_Rigibody;
    CapsuleCollider m_collider;
    


    private void Awake()
    {
        m_PlayerControls = GetComponent<PlayerControls>();

        m_Rigibody = GetComponent<Rigidbody>();
        m_collider = GetComponent<CapsuleCollider>();

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void LateUpdate()
    {
        //Debug.Log("player count"+  PlayerEntity.allPlayers.Count); 
    }

    protected override void OnSpawned()
    {
        base.OnSpawned();
        if (isOwner)
        {
            GameManager.Instance.LocalPlayer = this;
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }

    public void EnterVehicle(Vehicule vehicle)
    {
        Debug.Log("Entering vehicle"); 
        
        Physics.IgnoreCollision(vehicle.GetComponent<Collider>(), m_collider, true);
        vehicle.GiveOwnership(localPlayer);
        vehicle.HasDriver.value = true;
        
        transform.SetParent(vehicle.PivotAnchor, false); 
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        Debug.Log("vehcilule owner : " );

        
        //m_PlayerControls.SwitchLocomotionState(PlayerControls.ELocomotionState.Bicycle);
        m_PlayerControls.SwitchToBicycle(vehicle);

    }

    public void ExitVehicle()
    {

        Debug.Log("Exit vehicle"); 
        Physics.IgnoreCollision(m_PlayerControls.m_CurrentVehicule.GetComponent<Collider>(), m_collider, false);
        m_PlayerControls.SwitchLocomotionState(PlayerControls.ELocomotionState.Foot);
    }

    [ServerRpc(requireOwnership: false)]
    public void BumpPlayer(PlayerID target, Vector3 normalizedDirection, float force)
    {
        Bump(target, normalizedDirection, force);
    }
    
    [TargetRpc]
    void Bump(PlayerID target,Vector3 normalizedDirection, float force)
    {
        Debug.Log("Bumping");
        if (m_PlayerControls.m_CurrentVehicule)
        {
            m_PlayerControls.m_CurrentVehicule.m_RigidBody.AddForce(force*5* normalizedDirection,ForceMode.Impulse);
            return; 
        }
        m_Rigibody.AddForce(force*10* normalizedDirection,ForceMode.Impulse);

    }



 

    public void PlayerRespawn()
    {
        
        m_PlayerControls.SwitchLocomotionState(PlayerControls.ELocomotionState.Foot);
        transform.SetPositionAndRotation(GameManager.Instance.PlayerSpawnPoint.position, Quaternion.identity);
    }

    public void PlayerDisconnect()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //return; 
        // if (isServer)
        // {
        //     networkManager.StopServer();
        // }
        // if (isClient)
        // {
        //     networkManager.StopClient();
        // }
        
    }

    protected override void OnSpawned(bool asServer)
    {
        base.OnSpawned(asServer);
    }
}
