using PurrNet;
using UnityEngine;

public class AutoSpawner : NetworkBehaviour
{
    [SerializeField] private float m_Timer = 0f;
    [SerializeField] private float m_Time = 5f;
    [SerializeField] private float m_Radius = 5f;
    [SerializeField] private Transform m_SpawnAnchor; 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_Timer += Time.deltaTime;
        if (m_Timer >= m_Time)
        {
            m_Timer = 0f;
            TrySpawnVehicle(); 

        }
    }

    public void TrySpawnVehicle()
    {
        if (networkManager.isOffline)
        {
            return;
        }
        if (!isServer)
        {
            
            return; 
            
        }
        if (Physics.CheckSphere(transform.position, m_Radius, LayerMask.GetMask("Vehicle")))
        {
            return; 
        }
        BikeManager.Instance.GiveRandomVehicle(m_SpawnAnchor.position);
        
    }
}
