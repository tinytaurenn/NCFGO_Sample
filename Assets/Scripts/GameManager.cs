using System;
using PurrNet;
using PurrNet.Transports;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private Transform m_PlayerRespawnPoint; 
    public Transform PlayerSpawnPoint => m_PlayerRespawnPoint;
    public PlayerEntity LocalPlayer;

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
        if (isOwner)
        {
            Debug.Log("server something");
        }
        
        
    }

    private void OnEnable()
    {
        Debug.Log("server something 1 ");
        if (isOwner)
        {
            Debug.Log("server something");
        }

        
    }

    protected override void OnSpawned()
    {
        base.OnSpawned();
        Debug.Log("server something 2 ");
        if(isOwner)
        {
            Debug.Log("server owner");
        }
        if(isServer)
        {
            Debug.Log("server isServer");

            networkManager.onPlayerLeft += OnPlayerLeft;

        }
        if(isHost)
        {
            Debug.Log("server host");
        }
    }

    private void OnPlayerLeft(PlayerID player, bool asServer)
    {
        Debug.Log("player left :  " + player);
        
    }


    // Update is called once per frame
    void Update()
    {
        return;
        
        if(Keyboard.current.numpad1Key.isPressed)
        {
           TeleportingPlayersToSpawn();
            
        }
        if(Keyboard.current.numpad2Key.isPressed)
        {
            GetAllPlayers();
            
        }
        
    }
    [ServerRpc(requireOwnership:false)]
    public void TeleportingPlayersToSpawn()
    {
        Debug.Log("Telepôrting Players");
        TeleportingLocalPlayerToSpawn(); 
    }

    [ObserversRpc]
    void TeleportingLocalPlayerToSpawn()
    {
        Debug.Log("teleporting local  Players");
        //PlayerEntity.TryGetLocal(out PlayerEntity newlocalPlayer);
        if (LocalPlayer)
        {
            LocalPlayer.PlayerRespawn();
        }
    }
    
    [ServerRpc(requireOwnership:false)]
    public void GetAllPlayers()
    {
        int num = PlayerEntity.allPlayers.Count; 
        Debug.Log("number of players : " + num);
    }
    
}
