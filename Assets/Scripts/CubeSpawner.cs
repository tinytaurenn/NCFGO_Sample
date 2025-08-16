using PurrNet;
using UnityEngine;
using UnityEngine.InputSystem;

public class CubeSpawner : NetworkBehaviour
{
    [SerializeField] GameObject m_CubePrefab;
    [SerializeField] GameObject m_SpawnedCube; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.eKey.isPressed)
        {
            Debug.Log("spawning ");
            if(m_SpawnedCube != null) Destroy(m_SpawnedCube); m_SpawnedCube = null; 
            m_SpawnedCube =  Instantiate(m_CubePrefab,transform.position, transform.rotation);
        }
    }
    protected override void OnSpawned(bool asServer)
    {
        base.OnSpawned(asServer);
        enabled = isOwner;
    }
}
