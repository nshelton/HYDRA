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

        }

        m_parameters.Add(new GUIFloat()
        {
            effect = v =>
            {
                m_hdri.multiplier.value = v;
                m_gradientSky.multiplier.value = v;
            },
            min = 0,
            max = 3,
            value = 1,
            name = "Ambient"
        });

        m_parameters.Add(new GUIFloat()
        {
            effect = v => m_dirLight.SetIntensity(v),
            min = 0,
            max = 3,
            value = 1,
            name = "Directional"
        });

        m_parameters.Add(new GUIFloat()
        {
            effect = v => Time.timeScale = v,
            min = 0,
            max = 2,
            value = 1,
            name = "Time"
        });

        m_parameters.Add(new GUIFloat()
        {
            effect = v => 
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
            },
            min = 0,
            max = 3,
            value = 3,
            name = "Environment"
        });



    }

}
