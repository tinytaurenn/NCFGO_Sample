using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    InputSystem_Actions m_InputActions;
    [SerializeField] PlayerMovement m_PlayerMovement; 

    Vector2 m_MoveValue;

    private void Awake()
    {
        m_InputActions = new InputSystem_Actions();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetMovementValue(m_InputActions.Player.Move.ReadValue<Vector2>());
    }
    private void OnEnable()
    {
        m_InputActions.Player.Enable();


    }
    private void OnDisable()
    {
        m_InputActions.Player.Disable();
        
    }

    void SetMovementValue(Vector2 moveInput)
    {
        m_MoveValue = moveInput;
        //Debug.Log("Movement Input: " + moveInput);
        Vector3 forward = transform.forward; 
        Vector3 right = transform.right;

        m_PlayerMovement.MoveInput = right * m_MoveValue.x + forward * m_MoveValue.y;



    }
}
