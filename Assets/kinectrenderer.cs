using Akvfx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kinectrenderer : MonoBehaviour
{



    public DeviceController m_kinect ;
    private MeshRenderer m_renderer;

    void Start()
    {
        m_renderer = GetComponentInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        m_renderer.sharedMaterial.SetTexture("_PositionTex", m_kinect.PositionMap);
        m_renderer.sharedMaterial.SetTexture("_ColorTex", m_kinect.ColorMap);

    }
}
