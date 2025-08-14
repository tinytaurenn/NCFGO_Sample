using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class ClientAuthMovement : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float Speed = 5;
    

    void Update()
    {
        // IsOwner will also work in a distributed-authoritative scenario as the owner 
        // has the Authority to update the object.
        if (!IsOwner || !IsSpawned) return;


        var multiplier = Speed * Time.deltaTime;

        if (Keyboard.current.aKey.isPressed)
        {
            transform.position += new Vector3(-multiplier, 0, 0);
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            transform.position += new Vector3(multiplier, 0, 0);
        }
        else if (Keyboard.current.wKey.isPressed)
        {
            transform.position += new Vector3(0, 0, multiplier);
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            transform.position += new Vector3(0, 0, -multiplier);
        }
    }

}

