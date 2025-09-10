using PurrNet;
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

    public void EnterVehicle(Vehicule vehicle)
    {
        Debug.Log("Entering vehicle"); 
        vehicle.GiveOwnership(localPlayer);
        vehicle.HasDriver.value = true;
        transform.SetParent(vehicle.transform, false); 
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        m_Rigibody.isKinematic = true;
        //m_PlayerControls.SwitchLocomotionState(PlayerControls.ELocomotionState.Bicycle);
        m_PlayerControls.SwitchToBicycle(vehicle);

    }


}
