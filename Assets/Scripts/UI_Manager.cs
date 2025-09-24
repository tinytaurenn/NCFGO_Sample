using TMPro;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance { get; private set;}
    [SerializeField] TextMeshProUGUI m_UseText;

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
    public void SetUseText(string text)
    {
        m_UseText.text = text; 
    }
    public void ShowText(bool show)
    {
        m_UseText.gameObject.SetActive(show);
    }
    
}
