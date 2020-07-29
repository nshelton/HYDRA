using Klak.Motion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelModule : BaseGUIModule
{

    public BrownianMotion m_brownianMotion;
    public Transform m_scaleTransform;

    public override string Name() { return "model"; }

    public override void Init()
    {
        Parameters.Add(new GUIFloat("scale", 0, 3, 1, delegate (float v)
        {
            m_scaleTransform.localScale = Vector3.one * v;
        }));

        Parameters.Add( new GUIFloat("position", 0, 3, 1, delegate (float v)
        {
            m_brownianMotion.positionAmount = Vector3.one * v;
        }));

        Parameters.Add( new GUIFloat("rotation", 0, 180, 1, delegate (float v)
        {
            m_brownianMotion.rotationAmount = Vector3.one * v;
        }));

        Parameters.Add(new GUIFloat("frequency", 0, 1, 0.1f, delegate (float v)
        {
           m_brownianMotion.frequency = v;
        }));

        foreach(var p in Parameters)
        {
            var row = new GUIRow();
            row.Items.Add(p);
            GUIRows.Add(row);
        }
        base.Init();
    }
}
