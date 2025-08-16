using PurrNet;
using UnityEngine;
using UnityEngine.InputSystem;

public class RPC_Box : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
}
