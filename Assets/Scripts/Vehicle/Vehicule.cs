using System;
using PurrNet;
using UnityEngine;
using UnityEngine.InputSystem;

public class Vehicule : NetworkBehaviour, IUsable
{
    public SyncVar<bool> HasDriver = new SyncVar<bool>(ownerAuth: true);
    public VehiculeMovement m_VehicleMovement;
    public Transform PivotAnchor;
    public Transform HandAnchorRight; 
    public Transform HandAnchorLeft; 
    public Transform FootAnchorLeft; 
    public Transform FootAnchorRight;

    [SerializeField] private Transform m_RotorTransform; 

    private void Awake()
    {
        m_VehicleMovement = GetComponent<VehiculeMovement>();
    }
    void Start()
    {
        Debug.Log("has Owner" + hasConnectedOwner);
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void OnSpawned()
    {
        base.OnSpawned();
        
    }

    protected override void OnOwnerDisconnected(PlayerID ownerId)
    {
        base.OnOwnerDisconnected(ownerId);
        Debug.Log("owner disconected");
        HasDriver.value = false;
    }

    private void FixedUpdate()
    {
        //m_RotorTransform.Rotate(transform.forward, m_VehicleMovement.currentSpeed * Time.fixedDeltaTime * 30f);
        Quaternion deltaRot = Quaternion.Euler(0f, -m_VehicleMovement.currentSpeed * Time.fixedDeltaTime * 60f, 0f);
        m_RotorTransform.rotation *=  deltaRot;
    }

    public void TryUse(PlayerEntity playerEntity)
    {
        if (HasDriver.value)
        {
            Debug.Log("already used as driver");
            return;
        }
        

        if (playerEntity)
        {

            playerEntity.EnterVehicle(this);
        }
        else
        {
            Debug.Log("no local player found");
        }
    }
}
