using PurrNet;
using PurrNet.Transports;
using UnityEngine;


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
        
    }

    public void EnterVehicle(Vehicule vehicle)
    {
        Debug.Log("Entering vehicle"); 
        vehicle.GiveOwnership(localPlayer);
        vehicle.HasDriver.value = true;
        
        transform.SetParent(vehicle.PivotAnchor, false); 
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        Debug.Log("vehcilule owner : " );

        
        //m_PlayerControls.SwitchLocomotionState(PlayerControls.ELocomotionState.Bicycle);
        m_PlayerControls.SwitchToBicycle(vehicle);

    }

 

    public void PlayerRespawn()
    {
        
        m_PlayerControls.SwitchLocomotionState(PlayerControls.ELocomotionState.Foot);
        transform.SetPositionAndRotation(GameManager.Instance.PlayerSpawnPoint.position, Quaternion.identity);
    }

    protected override void OnSpawned(bool asServer)
    {
        base.OnSpawned(asServer);
    }
}
