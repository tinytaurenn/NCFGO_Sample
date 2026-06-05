using PurrNet.Logging;
using PurrNet.Transports;
using PurrNet;
using UnityEngine;
using System.Collections;
using PurrLobby;
using PurrNet.Steam;
using Steamworks;
using TMPro;
using WebSocketSharp;


public class ConnectionStarter_3 : MonoBehaviour
    {
        PurrTransport m_PurrTransport;
        SteamTransport m_SteamTransport; 
        UDPTransport m_UDPTransport;
        [SerializeField] private bool m_IsLocalPlaying = true; 
        [SerializeField]string m_ServerAdress = "127.0.0.1";
        [SerializeField]  ushort m_ServerPort = 5000; 
        private NetworkManager _networkManager;
        
        // UI Connection
        
        [SerializeField] TextMeshProUGUI m_ServerAdressText;
        [SerializeField] TextMeshProUGUI m_ServerPortText;
        [SerializeField] TMP_InputField m_ServerAdressInputField;
        [SerializeField] TMP_InputField m_ServerPortInputField;
        [SerializeField]  GameObject m_ConnectionUI; 
         
 

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
            

            //_lobbyDataHolder = FindFirstObjectByType<LobbyDataHolder>();
        }

        private void Start()
        {
            if (!_networkManager)
            {
                PurrLogger.LogError($"Failed to start connection. {nameof(NetworkManager)} is null!", this);
                return;
            }
            //StartNormal(); 

            

        }
        void StartNormal()
        {
            //networkManager.transport = m_UDPTransport;
            //start server if this is the host
            //_networkManager.StartServer();

            _networkManager.startClientFlags = StartFlags.ClientBuild; 
            if (m_IsLocalPlaying)
            {
                _networkManager.startClientFlags = (StartFlags)(-1);
                _networkManager.startServerFlags = StartFlags.ServerBuild | StartFlags.Editor; 
                m_UDPTransport.address = "127.0.0.1";
                m_UDPTransport.serverPort = 5000;
                _networkManager.StartServer();
            }
            else
            {
                _networkManager.startClientFlags = (StartFlags)(-1);
                _networkManager.startServerFlags = StartFlags.ServerBuild;
                //m_UDPTransport.address = m_ServerAdress;
                //m_UDPTransport.serverPort = m_ServerPort;
                //doesnt change adress
            }
            

            _networkManager.StartClient(); 
            
            m_ConnectionUI.SetActive(false);
        }
        
        public void StartLocalConnectionButton()
        {
            m_IsLocalPlaying = true;
            StartNormal();
        }

        public void StartServerButton()
        {
            if (m_ServerAdressInputField.text.IsNullOrEmpty() || m_ServerPortInputField.text.IsNullOrEmpty())
            {
                Debug.LogError("Server address or port is empty!");
                return;
            }
            m_UDPTransport.address = m_ServerAdressInputField.text;
            ushort serverPort =  ushort.Parse(m_ServerPortInputField.text);
            Debug.Log("server port is " + serverPort);
            m_UDPTransport.serverPort = serverPort;
            m_IsLocalPlaying = false;
            StartNormal();
        }

        
     
    }

