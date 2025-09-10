using PurrNet;
using UnityEngine;
using UnityEngine.InputSystem;

public class Vehicule : NetworkBehaviour
{
    public SyncVar<bool> HasDriver = new SyncVar<bool>(ownerAuth: true);
    public VehiculeMovement m_VehicleMovement;

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
        if (Keyboard.current.numpad3Key.isPressed)
        {
            DoSomething(); 
        }
    }

    void DoSomething()
    {
        if (HasDriver.value)
        {
            Debug.Log("already used as driver");
            return; 
        }

        if(PlayerEntity.TryGetLocal(out PlayerEntity localPlayer))
        {
            
            localPlayer.EnterVehicle(this); 
        }
        

    }
}
