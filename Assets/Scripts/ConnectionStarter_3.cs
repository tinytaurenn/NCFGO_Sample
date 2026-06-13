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
        [Space(10)]
        [Header("ENABLE THIS TO PUSH ONLINE")]
        [Space(10)]
        [SerializeField] private bool IsDedicatedServer = false; 
         
 

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

            _networkManager.onNetworkShutdown += OnNetworkShutDown;
            _networkManager.onNetworkStarted += OnNetworkStarted;

            //_lobbyDataHolder = FindFirstObjectByType<LobbyDataHolder>();
        }

        private void OnNetworkStarted(NetworkManager manager, bool asServer)
        {
            Debug.Log("Network Started");
            
        }

        private void OnNetworkShutDown(NetworkManager manager, bool asServer)
        {
            Debug.Log("Network ShutDown");
            if (!m_ConnectionUI) return; 
            m_ConnectionUI.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            if (_networkManager) StartCoroutine(WaitForAnalyzeNetworkStatus());
            
            
            
        }
        
        

        private void Start()
        {
            
            if (!_networkManager)
            {
                PurrLogger.LogError($"Failed to start connection. {nameof(NetworkManager)} is null!", this);
                return;
            }

            if (IsDedicatedServer)
            {
                _networkManager.startClientFlags = (StartFlags)(-1);
                _networkManager.startServerFlags = StartFlags.ServerBuild | StartFlags.Editor; 
                _networkManager.StartServer();
                
            }
            
            
            //StartNormal(); 

            

        }

        void StartLocalClient()
        {
            _networkManager.startClientFlags = (StartFlags)(-1);
            _networkManager.startServerFlags = StartFlags.ServerBuild | StartFlags.Editor; 
            m_UDPTransport.address = "127.0.0.1";
          
            m_UDPTransport.serverPort = 5000;
            _networkManager.StartServer();
            _networkManager.StartClient(); 
            
            m_ConnectionUI.SetActive(false);
            StartCoroutine(WaitForAnalyzeNetworkStatus());
        }

        void StartNetworkClient()
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
            
            _networkManager.startClientFlags = (StartFlags)(-1);
            _networkManager.startServerFlags = 0; //StartFlags.ServerBuild;
            _networkManager.StartClient(); 
            
            m_ConnectionUI.SetActive(false);
            StartCoroutine(WaitForAnalyzeNetworkStatus());
        }
        
        
        public void StartLocalConnectionButton()
        {
            StartLocalClient();
            m_IsLocalPlaying = true;
        }

        public void StartServerButton()
        {
            
            StartNetworkClient();
            m_IsLocalPlaying = false;
        }

        IEnumerator WaitForAnalyzeNetworkStatus()
        {
            yield return new WaitForSeconds(0.5f);
            if (_networkManager.isOffline)
            {
                m_ConnectionUI.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                m_ConnectionUI.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        

        
     
    }

