using PurrNet;
using UnityEngine;
using UnityEngine.InputSystem;

public class Vehicule : NetworkBehaviour, IUsable
{
    public SyncVar<bool> HasDriver = new SyncVar<bool>(ownerAuth: true);
    public VehiculeMovement m_VehicleMovement;
    public Transform PivotAnchor; 

    private void Awake()
    {
        m_VehicleMovement = GetComponent<VehiculeMovement>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

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
