using PurrNet;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class RPC_Box : NetworkBehaviour
{
    [SerializeField] TextMeshPro m_SimpleText; 

    public SyncVar<int> m_SimpleTextSync = new SyncVar<int>(ownerAuth : true);

    private void Awake()
    {
        m_SimpleTextSync.onChanged += OnSimpleTextChanged; 
    }

    private void OnSimpleTextChanged(int obj)
    {
        m_SimpleText.text = obj.ToString();

    }

    void Start()
    {
        m_SimpleTextSync.value = int.Parse(m_SimpleText.text);
        //string to int method ? 


    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.numpad1Key.isPressed)
        {
           
            SetColor(Color.red);
        }
        if(Keyboard.current.numpad2Key.isPressed)
        {
            SetColor(Color.green);
        }
        if(Keyboard.current.numpad3Key.isPressed)
        {
            SetColor(Color.blue);
        }
        if(Keyboard.current.numpad4Key.isPressed)
        {
            SetColor(Color.yellow);
        }

        //sync var

        if (Keyboard.current.numpad5Key.isPressed)
        {
            SetTextServer(1); //better on server objects
        }
        if (Keyboard.current.numpad6Key.isPressed)
        {
            //better for client objects
            PlayerID? ownerID = m_SimpleTextSync.owner;
            if(ownerID == null ) GiveOwnership(localPlayer); // simple to take ownership if the object is not owned


            Debug.Log("owner id is" +  ownerID);
            SetTargetText(ownerID.Value, -1); // if the item has ownership, just send to them

        }

        if(Keyboard.current.numpad7Key.isPressed) // take ownership and toggle renderer with reflection
        {
            GiveOwnership(localPlayer);
   
            ToggleRenderer();
        }
        if (Keyboard.current.numpad8Key.isPressed) // does need an ownership
        {

            if(owner == null) GiveOwnership(localPlayer); // simple to take ownership if the object is not owned
            ToggleRendererByOwner(owner.Value);
            
        }
    }
    [ServerRpc(requireOwnership:false)]
    void SetColor(Color color)
    {
        SetColor_Observer(color);



    }
    [ObserversRpc()]
    void SetColor_Observer(Color color)
    {
        //create instanced material for this object with material properties
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        //GetComponent<Renderer>().GetPropertyBlock(mpb);
        
        mpb.SetColor("_BaseColor", color);
        GetComponent<Renderer>().SetPropertyBlock(mpb);
    }

    [ServerRpc(requireOwnership:false)]
    void SetTextServer(int text)
    {
        SetTextObserver(text);
    }
    //observers is not needed bc of syncvar
    void SetTextObserver(int text)
    {
        m_SimpleTextSync.value += text;
    }
    [TargetRpc]
    void SetTargetText(PlayerID target,int text)
    {
        m_SimpleTextSync.value += text;
    }
    void ToggleRenderer()
    {
        if(owner != localPlayer)
        {
            Debug.Log("no ownership, cannot toggle renderer");
            return; 
        }
        GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;



    }
    [TargetRpc]
    void ToggleRendererByOwner(PlayerID target)
    {
        GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        m_SimpleTextSync.onChanged -= OnSimpleTextChanged; 
    }


}
