using Kino.PostProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PostModule : BaseGUIModule
{

    private VolumeProfile m_volume;

    private Kino.PostProcessing.Glitch m_glitch;

    public override string Name() { return "postfx"; }


    private void AddParameters<T>(VolumeProfile volume) where T : VolumeComponent
    {
        foreach (var effect in volume.components)
        {
            Debug.Log(effect.GetType().Name);
            var cast = effect as T;

            if (cast != null)
            {
                foreach (var thisVar in cast.GetType().GetFields())
                {
                    ClampedFloatParameter item = thisVar.GetValue(cast) as ClampedFloatParameter;
                    if (item != null)
                    {
                        m_parameters.Add(new GUIFloat()
                        {
                            effect = v => item.value = v,
                            min = item.min,
                            max = item.max,
                            value = 0,
                            name = thisVar.Name
                        });

                    }
                }
            }
        }
    }

    public override void Init()
    {
        m_volume = GetComponent<Volume>().profile;
        AddParameters<Glitch>(m_volume);
        AddParameters<Slice>(m_volume);

    }
}
