using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;


public class LightingModule : BaseGUIModule
{
    public HDAdditionalLightData m_dirLight;
    public VolumeProfile m_volume;

    private GradientSky m_gradientSky;
    private HDRISky m_hdri;
    private VisualEnvironment m_env;

    public override void Init()
    {
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
                m_env.skyType.value = v > 0.5 ? 1 : 3;
            },
            min = 0,
            max = 1,
            value = 1,
            name = "Environment"
        });

    }

}
