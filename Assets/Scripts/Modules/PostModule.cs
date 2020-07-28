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
                        m_parameters.Add(new GUIFloat(effectName + "." + thisVar.Name, item.min, item.max, 0, delegate (float v)
                        {
                            item.value = v;
                        }));
                    }
                }

               /* if (typeof(T) == typeof(Overlay))
                {
                    Overlay ovr = cast as Overlay;
                    m_triggers.Add(new GUITrigger()
                    {
                        effect = delegate {
                            ovr.gradient.value.colorKeys[0] = new GradientColorKey( UnityEngine.Random.ColorHSV(), 0.0f);
                        },
                        name = "ColorSwitch"
                    });
                }

                */
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

    }
}
