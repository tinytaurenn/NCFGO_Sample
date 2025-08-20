using PurrNet.Logging;
using PurrNet.Transports;
using PurrNet;
using UnityEngine;
using System.Collections;
using PurrLobby;
using PurrNet.Steam;
using Steamworks;

public class ConnectionStarter_2 : MonoBehaviour
    {
        PurrTransport m_PurrTransport;
    SteamTransport m_SteamTransport; 
        UDPTransport m_UDPTransport; 
        
        private NetworkManager _networkManager;
        private LobbyDataHolder _lobbyDataHolder;


        bool m_IsFromLobby;

        private void Awake()
        {
            if (!TryGetComponent(out _networkManager))
            {
                PurrLogger.LogError($"Failed to get {nameof(NetworkManager)} component.", this);
            }
            if (!TryGetComponent(out m_UDPTransport))
            {
                PurrLogger.LogError($"Failed to get {nameof(UDPTransport)} component.", this);
            }
            if(!TryGetComponent(out m_SteamTransport))
            {
                PurrLogger.LogError($"Failed to get {nameof(SteamTransport)} component.", this);
            }
            

            _lobbyDataHolder = FindFirstObjectByType<LobbyDataHolder>();
            if (!_lobbyDataHolder)
            {
                PurrLogger.LogError($"Failed to get {nameof(LobbyDataHolder)} component.", this);
            }
               
            else
            {
                m_IsFromLobby = true; 
            }
        }

        private void Start()
        {
            if (!_networkManager)
            {
                PurrLogger.LogError($"Failed to start connection. {nameof(NetworkManager)} is null!", this);
                return;
            }
            if(!m_SteamTransport)
            {
                PurrLogger.LogError($"Failed to start connection. {nameof(SteamTransport)} is null!", this);
                return;
            }

            if (m_IsFromLobby)
            {
                StartFromLobby();
            }
            else
            {
                StartNormal(); 
            }

            

        }
        void StartNormal()
        {
            _networkManager.transport = m_UDPTransport;
            //start server if this is the host
            _networkManager.StartServer();



            _networkManager.StartClient(); 
        }

        void StartFromLobby()
        {
            _networkManager.transport = m_SteamTransport; 
            if (!_lobbyDataHolder)
            {
                PurrLogger.LogError($"Failed to start connection. {nameof(LobbyDataHolder)} is null!", this);
                return;
            }

            if (!_lobbyDataHolder.CurrentLobby.IsValid)
            {
                PurrLogger.LogError($"Failed to start connection. Lobby is invalid!", this);
                return;
            }
            if(!ulong.TryParse(_lobbyDataHolder.CurrentLobby.LobbyId, out ulong lobbyId))
        {
                Debug.LogError($"Failed to start connection. Lobby ID is invalid!", this);
                return;
        }
            var lobbyOwner =  SteamMatchmaking.GetLobbyOwner(new CSteamID(lobbyId));
            if (!lobbyOwner.IsValid())
            {
            Debug.LogError($"error, Lobby Owner: {lobbyOwner}, Lobby ID: {lobbyId}", this);
            }

            m_SteamTransport.address = lobbyOwner.ToString();

            //#if UTP_LOBBYRELAY
            //            else if(_networkManager.transport is UTPTransport) {
            //                if(_lobbyDataHolder.CurrentLobby.IsOwner) {
            //                    (_networkManager.transport as UTPTransport).InitializeRelayServer((Allocation)_lobbyDataHolder.CurrentLobby.ServerObject);
            //                }
            //                (_networkManager.transport as UTPTransport).InitializeRelayClient(_lobbyDataHolder.CurrentLobby.Properties["JoinCode"]);
            //            }
            //#else
            //                //P2P Connection, receive IP/Port from server
            //#endif

            if (_lobbyDataHolder.CurrentLobby.IsOwner)
                _networkManager.StartServer();
            StartCoroutine(StartClient());
        }

        private IEnumerator StartClient()
        {
            yield return new WaitForSeconds(1f);
            _networkManager.StartClient();
        }
     
    }

