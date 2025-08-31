using PurrNet;
using UnityEngine;

public class SpawnedCube : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnOwnerDisconnected(PlayerID ownerId)
    {
        Debug.Log("Cube Owner disconnected");
        base.OnOwnerDisconnected(ownerId);
    }


    protected override void OnDestroy()
    {
        Debug.Log("Cube destroyed");
        base.OnDestroy();
    }

    protected override void OnDespawned()
    {
        Debug.Log("Cube Despawned");
        base.OnDespawned();
    }
}
