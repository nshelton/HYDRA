using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FFModule : BaseGUIModule
{
    public override string Name() { return "fractal flames"; }

    public FreeParticle m_ff;

    public override void InitInternal()
    {
        Parameters.Add(new GUIFloat("x", -10, 10, 1, delegate (float v) { m_ff.OffsetX = v; }));
        Parameters.Add(new GUIFloat("y", -10, 10, 1, delegate (float v) { m_ff.OffsetY = v; }));
        Parameters.Add(new GUIFloat("z", -10, 10, 1, delegate (float v) { m_ff.OffsetZ = v; }));

        foreach (var p in Parameters)
        {
            var row = new GUIRow();
            row.Items.Add(p);
            GUIRows.Add(row);
        }
    }

}
