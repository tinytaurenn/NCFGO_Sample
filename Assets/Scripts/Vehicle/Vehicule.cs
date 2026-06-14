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
    public Rigidbody m_RigidBody;

    private void Awake()
    {
        m_VehicleMovement = GetComponent<VehiculeMovement>();
        m_RigidBody = GetComponent<Rigidbody>();
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

    private void OnCollisionEnter(Collision collision)
    {
        if (HasDriver.value)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Vehicle"))
            {
                Debug.Log("Collision with player");
                if(collision.transform.TryGetComponent<Vehicule>(out Vehicule vehicle))
                {
                    if (vehicle.owner != null)
                    {
                        vehicle.BumpBike(vehicle.owner.Value, (collision.transform.position - transform.position).normalized + Vector3.up *0.5f, m_VehicleMovement.currentSpeed);
                        return; 
                    }
                    
                }
                
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                //Debug.Log("Collision with player");
                if (collision.transform.TryGetComponent<PlayerEntity>(out PlayerEntity player))
                {
                    
                    //Debug.Log(player.GetNetworkID(false));
                    Debug.Log(player.owner);
                    if (player.owner != null)
                    {
                        player.BumpPlayer(player.owner.Value, (collision.transform.position - transform.position).normalized + Vector3.up *0.35f, m_VehicleMovement.currentSpeed);
                        return; 
                    }
                
               
                
                
                }
            }
            
        }
        
    }
    
    [ServerRpc(requireOwnership: false)]
    public void BumpBike(PlayerID target, Vector3 normalizedDirection, float force)
    {
        Bump(target, normalizedDirection, force);
    }
    
    [TargetRpc]
    void Bump(PlayerID target,Vector3 normalizedDirection, float force)
    {
        Debug.Log("Bumping");
        m_RigidBody.AddForce(force*50* normalizedDirection,ForceMode.Impulse);
        

    }
}
