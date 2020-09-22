using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class LightingModule : BaseGUIModule
{

    public Light m_dirLight;
    public PostProcessProfile m_profile;

    public Material m_gradientSkybox;
    public Material m_hdriSkybox;

    public Cubemap m_hdri;
    public Cubemap m_gradient;
    

    private ColorGrading m_colorGrade;
    private AmbientOcclusion m_ao;
    private Bloom m_bloom;
    private Grain m_grain;
    private Camera m_camera;

    public override string Name() { return "lighting"; }

    public override void Init()
    {
        m_camera = Camera.main;

        m_profile = GetComponent<PostProcessVolume>().profile;
        m_colorGrade = m_profile.GetSetting<ColorGrading>();
        m_ao = m_profile.GetSetting<AmbientOcclusion>();
        m_bloom = m_profile.GetSetting<Bloom>();
        m_grain = m_profile.GetSetting<Grain>();


        Parameters.Add(new GUIFloat("Exposure", -3, 4, 0,
             delegate (float v) { m_colorGrade.postExposure.value = v; }));

        Parameters.Add(new GUIFloat("Contrast", -100, 100, 0,
            delegate (float v) { m_colorGrade.contrast.value = v; }));
        
        Parameters.Add(new GUIFloat("Saturation", -100, 100, 0,
            delegate (float v) { m_colorGrade.saturation.value = v; }));

        Parameters.Add(new GUIFloat("ao", 0, 2, 0.5f,
          delegate (float v) { m_ao.intensity.value = v; }));

        Parameters.Add(new GUIFloat("bloom", 0, 2, 0,
            delegate (float v) { m_bloom.intensity.value = v; }));

        Parameters.Add(new GUIFloat("grain", 0, 1, 0,
            delegate (float v) { m_grain.intensity.value = v; }));

        Parameters.Add(new GUIFloat("Ambient", 0, 3, 1, delegate (float v)
        {
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = Color.white * v;

        }));

        Parameters.Add(new GUIFloat("Directional", 0, 3, 1, delegate (float v)
        {
            m_dirLight.intensity = v;
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
            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
            RenderSettings.customReflection = m_hdri;
            m_camera.clearFlags = CameraClearFlags.Skybox;
            RenderSettings.skybox = m_hdriSkybox;
        }));

        row.Items.Add(Parameters[Parameters.Count - 1]);

        Parameters.Add(new GUITrigger("gradientsky", delegate
        {
            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
            RenderSettings.customReflection = m_gradient;
            m_camera.clearFlags = CameraClearFlags.Skybox;
            RenderSettings.skybox = m_gradientSkybox;
        }));

        row.Items.Add(Parameters[Parameters.Count - 1]);

        Parameters.Add(new GUITrigger("color", delegate
        {
            m_camera.clearFlags = CameraClearFlags.SolidColor;
            m_camera.backgroundColor = Color.white * 0.1f;

        }));

        row.Items.Add(Parameters[Parameters.Count - 1]);

        GUIRows.Add(row);
          
        base.Init();
    }
}
