using System;
using PurrNet;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private Transform m_PlayerRespawnPoint; 
    public Transform PlayerSpawnPoint => m_PlayerRespawnPoint;

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
}
