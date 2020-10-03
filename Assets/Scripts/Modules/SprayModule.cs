using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayModule : BaseGUIModule
{
    public override string Name() { return "spray"; }

    private Kvant.SprayMV[] m_spray;
    public override void InitInternal()
    {
        m_spray = GetComponentsInChildren<Kvant.SprayMV>();

        Parameters.Add(new GUIFloat("throttle", 0, 1, 1,
            delegate (float v) { Array.ForEach(m_spray, s => s.throttle = v); }));

        Parameters.Add(new GUIFloat("scale", 0, 1, 0.1f,
            delegate (float v) { Array.ForEach(m_spray, s => s.scale = v); }));

        Parameters.Add(new GUIFloat("drag", 0, 4, 1,
            delegate (float v) { Array.ForEach(m_spray, s => s.drag = v); }));

        Parameters.Add(new GUIFloat("amp", 0, 1, 0.5f,
            delegate (float v) { Array.ForEach(m_spray, s => s.noiseAmplitude = v); }));

        Parameters.Add(new GUIFloat("freq", 0, 1, 0.1f,
            delegate (float v) { Array.ForEach(m_spray, s => s.noiseFrequency = v); }));

        foreach (var p in Parameters)
        {
            var r = new GUIRow();
            r.Items.Add(p);
            GUIRows.Add(r);
        }
    }
}
