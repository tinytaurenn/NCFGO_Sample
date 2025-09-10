using PurrNet;
using UnityEngine;
using UnityEngine.InputSystem;

public class Vehicule : NetworkBehaviour
{
    public SyncVar<bool> HasDriver = new SyncVar<bool>(ownerAuth: true);
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
