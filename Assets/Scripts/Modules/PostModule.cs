using Kino.PostProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class PostModule : BaseGUIModule
{

    private VolumeProfile m_volume;
    public Gradient m_randomGradient;

    public override string Name() { return "postfx"; }

    private void RandomizeGradient()
    {
        m_randomGradient.colorKeys[0].color.r = UnityEngine.Random.value;
        m_randomGradient.colorKeys[1].color.r = UnityEngine.Random.value;
    }

    private void AddParameters<T>(VolumeProfile volume) where T : VolumeComponent
    {
        foreach (var effect in volume.components)
        {
            var effectName = effect.GetType().Name;
            var cast = effect as T;

            if (cast != null)
            {
                foreach (var thisVar in cast.GetType().GetFields())
                {
                    ClampedFloatParameter item = thisVar.GetValue(cast) as ClampedFloatParameter;
                    if (item != null)
                    {
                        var row = new GUIRow();
                        var parameter = new GUIFloat(effectName + "." + thisVar.Name,
                            item.min, item.max, 0, delegate (float v)
                            {
                                item.value = v;
                            });
                        Parameters.Add(parameter);
                        row.Items.Add(parameter);
                        GUIRows.Add(row);
                    }
                }
                // add some spacing
                GUIRows.Add(new GUIRow());
            }
        }
    }

    public override void Init()
    {
        m_volume = GetComponent<Volume>().profile;
        AddParameters<Sharpen>(m_volume);
        AddParameters<Glitch>(m_volume);
        AddParameters<Slice>(m_volume);
        AddParameters<Overlay>(m_volume);
        AddParameters<Chroma>(m_volume);
        AddParameters<Mirror>(m_volume);
        base.Init();

    }
}
