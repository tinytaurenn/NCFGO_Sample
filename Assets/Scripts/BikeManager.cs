using System;
using System.Collections.Generic;
using System.Linq;
using PurrNet;
using UnityEngine;
using UnityEngine.InputSystem;

public class BikeManager : NetworkBehaviour
{
    [SerializeField] List<GameObject> m_AvailableVehicles = new List<GameObject>();
    [SerializeField] List<Vehicule>  m_Vehicles = new List<Vehicule>();

    public static BikeManager Instance;

    private void Awake()
    {
        if(!Instance)
        {
            Instance = this; 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnSpawned()
    {
        base.OnSpawned();
        if (isServer)
        {
            GetAllBikes();
        }
    }

    void GetAllBikes()
    {
        List<Vehicule> newList = FindObjectsByType<Vehicule>().ToList();
        m_Vehicles = newList; 

    }

    [ServerRpc(requireOwnership: false)]
    public void GiveRandomVehicle(Vector3 position)
    {
        int randomIndex = m_AvailableVehicles.Count > 0 ? UnityEngine.Random.Range(0, m_AvailableVehicles.Count) : -1;
        if(randomIndex >= 0)
        {
            GameObject bike = Instantiate(m_AvailableVehicles[randomIndex], position, m_AvailableVehicles[randomIndex].transform.rotation);
            m_Vehicles.Add(bike.GetComponent<Vehicule>());
            
           
        }

    }
}
