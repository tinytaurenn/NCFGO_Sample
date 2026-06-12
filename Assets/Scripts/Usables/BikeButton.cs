using UnityEngine;

public class BikeButton : MonoBehaviour, IUsable
{
    [SerializeField] private float AcceptableDistance = 5f; 
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TryUse(PlayerEntity playerEntity)
    {
        if (Physics.CheckSphere(transform.position, AcceptableDistance, LayerMask.GetMask("Vehicle")))
        {
            return; 
        }
        BikeManager.Instance.GiveRandomVehicle(transform.position + Vector3.forward * 4 + Vector3.up * 3);
    }
}
