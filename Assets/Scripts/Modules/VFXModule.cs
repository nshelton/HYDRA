using Kino.PostProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class VFXModule : BaseGUIModule
{

    private VisualEffect m_effect;

    private string m_name;
    public override string Name() { return m_name; }

    public override void Init()
    {

        m_effect = GetComponent<VisualEffect>();

        m_name = "VFX." + m_effect.name;

        List<VFXExposedProperty> exposedProperties = new List<VFXExposedProperty>();

         m_effect.visualEffectAsset.GetExposedProperties(exposedProperties);

        foreach( var prop in exposedProperties)
        {
            if (prop.type == typeof(float))
            {
                m_parameters.Add(new GUIFloat(prop.name, 0, 1, 0, delegate (float v)
             {
                 m_effect.SetFloat(prop.name, v);
             }));
            }
        }
    }
}
