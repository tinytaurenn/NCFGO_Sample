using PurrNet;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkin : NetworkBehaviour
{
    [SerializeField] Renderer[] m_Renderers; 
    [SerializeField] Material[] m_Materials;
    public SyncVar<int> m_SkinIndex = new SyncVar<int>(ownerAuth : true);

    private void Awake()
    {
        m_SkinIndex.onChanged += OnSkinIndexChanged; 
    }
    
    private void OnSkinIndexChanged(int index)
    {
        foreach (var m_Renderer in m_Renderers)
        {
            m_Renderer.material = m_Materials[index];
        }
        
    }

    void Start()
    {
        int index = Random.Range(0, m_Materials.Length);
        m_SkinIndex.value = index;
        //Debug.Log("index  is " + index);


    }

    // Update is called once per frame
    void Update()
    {

        if (Keyboard.current.numpad0Key.isPressed)
        {
            SendMessage(1); 
        }
    }

    [ServerRpc(requireOwnership: false)]
    void SendMessage(int text,RPCInfo info = default)
    {
        ReceiveMessage(info.sender, info.sender.ToString());
    }

    [TargetRpc]
    void ReceiveMessage(PlayerID target,string id)
    {

        Debug.Log("info is " + id);
    }
}
