using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class physarumModule : BaseGUIModule
{
    public override string Name() { return "physarum"; }

    public physarum m_physarum;

    public override void InitInternal()
    {
        Parameters.Add(new GUIFloat("speed", 0, 7, 1, delegate (float v) { m_physarum.speed = v; }));
        Parameters.Add(new GUIFloat("iter", 0, 14, 1, delegate (float v) { m_physarum.iterations = v; }));
        Parameters.Add(new GUIFloat("sensorDist", 0, 100, 1, delegate (float v) { m_physarum.sensorDist = v; }));
        Parameters.Add(new GUIFloat("sensorDeg", 0, 100, 20, delegate (float v) { m_physarum.sensorDegrees = v; }));
        Parameters.Add(new GUIFloat("noiseAmount", 0, 0.01f, 0, delegate (float v) { m_physarum.noiseAmount = v; }));
        Parameters.Add(new GUIFloat("noiseScroll", 0, 0.2f, 0, delegate (float v) { m_physarum.noiseScroll = v; }));
        Parameters.Add(new GUIFloat("noiseFreq", 0, 8, 0, delegate (float v) { m_physarum.noiseFreq = v; }));
        Parameters.Add(new GUIFloat("linearForce", -0.01f, 0.01f, 0, delegate (float v) { m_physarum.linearForce = new Vector2(0, v); }));
        Parameters.Add(new GUIFloat("radialForce", -0.01f, 0.01f, 0, delegate (float v) { m_physarum.RadialForce = v; }));

        foreach (var p in Parameters)
        {
            var row = new GUIRow();
            row.Items.Add(p);
            GUIRows.Add(row);
        }
    }

}
