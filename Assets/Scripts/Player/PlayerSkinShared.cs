using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerSkinShared : MonoBehaviour
{
    public float RightArmFLag = 0f;
    public float LeftArmFlag = 0f;
    [SerializeField] TwoBoneIKConstraint m_RightHandIKConstraint;
    [SerializeField] TwoBoneIKConstraint m_LeftHandIKConstraint;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(m_RightHandIKConstraint.weight - RightArmFLag) > 0.01f)
        {
            m_RightHandIKConstraint.weight = Mathf.Lerp(m_RightHandIKConstraint.weight, RightArmFLag, Time.fixedDeltaTime * 8);
        }

        if (Mathf.Abs(m_LeftHandIKConstraint.weight - LeftArmFlag) > 0.01f)
        {
            m_LeftHandIKConstraint.weight = Mathf.Lerp(m_LeftHandIKConstraint.weight, LeftArmFlag, Time.fixedDeltaTime * 8);
        }
        
        
        
    }

  
}
