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


        Parameters.Add(new GUIFloat("Exposure", -3, 4, 0,
            delegate (float v) { m_color.postExposure.value = v; }));

        Parameters.Add(new GUIFloat("Saturation", -100, 100, 0, delegate (float v)
        {
            m_color.saturation.value = v;
        }));

        Parameters.Add(new GUIFloat("Ambient", 0, 3, 1, delegate (float v)
        {
            m_hdri.multiplier.value = v;
            m_gradientSky.multiplier.value = v;
        }));

        Parameters.Add(new GUIFloat("Directional", 0, 3, 1, delegate (float v)
        {
            m_dirLight.SetIntensity(v);
        }));


        Parameters.Add(new GUIFloat("Time", 0, 2, 1, delegate (float v)
        {
            Time.timeScale = v;
        }));

        foreach (var p in Parameters)
        {
            var r = new GUIRow();
            r.Items.Add(p);
            GUIRows.Add(r);
        }

        var row = new GUIRow();

        Parameters.Add(new GUITrigger("hdri", delegate
        {
            m_camera.clearColorMode = HDAdditionalCameraData.ClearColorMode.Sky;
            m_env.skyType.value = 1;
        }));

        row.Items.Add(Parameters[Parameters.Count - 1]);

        Parameters.Add(new GUITrigger("gradientsky", delegate
        {
            m_camera.clearColorMode = HDAdditionalCameraData.ClearColorMode.Sky;
            m_env.skyType.value = 3;
        }));

        row.Items.Add(Parameters[Parameters.Count - 1]);

        Parameters.Add(new GUITrigger("color", delegate
        {
            m_camera.clearColorMode = HDAdditionalCameraData.ClearColorMode.Color;
        }));

        row.Items.Add(Parameters[Parameters.Count - 1]);

        GUIRows.Add(row);
        base.Init();

    }

}
