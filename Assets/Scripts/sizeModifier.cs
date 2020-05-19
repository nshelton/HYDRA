using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sizeModifier : MonoBehaviour
{

    private ParticleSystemForceField m_forceField;

    void Start()
    {
        m_forceField = GetComponent<ParticleSystemForceField>();
    }

    // Update is called once per frame
    void Update()
    {
        m_forceField.gravity = -transform.localScale.x * 0.5f ;
    }
}
