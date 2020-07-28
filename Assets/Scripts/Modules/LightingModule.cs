using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;


public class LightingModule : BaseGUIModule
{
    public HDAdditionalLightData m_dirLight;
    public HDAdditionalCameraData m_camera;
    public VolumeProfile m_volume;
    public ColorAdjustments m_color;

    private GradientSky m_gradientSky;
    private HDRISky m_hdri;
    private VisualEnvironment m_env;

    public override string Name() { return "lighting"; }

    public override void Init()
    {
        m_camera = Camera.main.GetComponent<HDAdditionalCameraData>();

        foreach (var component in m_volume.components)
        {
            if (component is GradientSky)
                m_gradientSky = component as GradientSky;
            if (component is VisualEnvironment)
                m_env = component as VisualEnvironment;
            if (component is HDRISky)
                m_hdri = component as HDRISky;
            if (component is ColorAdjustments)
                m_color = component as ColorAdjustments;

        }


        m_parameters.Add(new GUIFloat("Exposure", -3, 4, 0,
            delegate (float v) { m_color.postExposure.value = v; }));

        m_parameters.Add(new GUIFloat("Saturation", -100, 100, 0, delegate (float v) 
        { 
            m_color.saturation.value = v; 
        }));

        m_parameters.Add(new GUIFloat("Ambient", 0, 3, 1, delegate (float v)
        {
            m_hdri.multiplier.value = v;
            m_gradientSky.multiplier.value = v;
        }));

        m_parameters.Add(new GUIFloat("Directional", 0, 3, 1, delegate (float v)
        {
            m_dirLight.SetIntensity(v);
        }));


        m_parameters.Add(new GUIFloat("Time", 0, 2, 1, delegate (float v)
        {
            Time.timeScale = v;
        }));


        m_parameters.Add(new GUIFloat("Environment", 0, 3, 3, delegate (float v) 
        {
            if ( v < 1)       {

                m_camera.clearColorMode = HDAdditionalCameraData.ClearColorMode.Sky;
                m_env.skyType.value = 1;
            } else if (v < 2)  {

                m_camera.clearColorMode = HDAdditionalCameraData.ClearColorMode.Sky;
                m_env.skyType.value = 3;
            }
            else {
                m_camera.clearColorMode = HDAdditionalCameraData.ClearColorMode.Color;
            }
        }));
    }   

}
