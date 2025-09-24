using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Vector2 MouseDelta { get; set; }
    [SerializeField] float m_MouseSensivity = 8f; // mouse sensitivity
    [SerializeField] float rotationClamp = 30f; 
    float currentXRotation = 0f;
    private void FixedUpdate()
    {
        if (MouseDelta.magnitude > 0)
        {
            // Apply the rotation
            currentXRotation -= MouseDelta.y * m_MouseSensivity * Time.fixedDeltaTime;

            // Clamp the new rotation value directly
            currentXRotation = Mathf.Clamp(currentXRotation, -rotationClamp, rotationClamp);

            // Apply the new rotation to the transform's local Euler angles
            transform.localEulerAngles = new Vector3(currentXRotation, transform.localEulerAngles.y, 0);
        }
    }

    private void Update()
    {
        
    }



    private void OnDrawGizmos()
    {
        
    }
}
