using System;
using UnityEngine;

public class SceneUIManager : MonoBehaviour
{
    public static SceneUIManager Instance;
    public GameObject ConnectionWindow; 

    private void Awake()
    {
        
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this; 
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
