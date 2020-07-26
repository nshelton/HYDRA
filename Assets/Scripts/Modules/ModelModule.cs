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
        m_parameters.Add(new GUIFloat()
        {
            name = "scale",
            effect = v => m_scaleTransform.localScale = Vector3.one * v,
            min = 0,
            max = 3,
            value = 1,
        });

        m_parameters.Add(new GUIFloat()
        {
            name = "move",
            effect = v => m_brownianMotion.positionAmount = Vector3.one * v,
            min = 0,
            max = 3,
            value = 1,
        });

        m_parameters.Add(new GUIFloat()
        {
            name = "rotation",
            effect = v => m_brownianMotion.rotationAmount = Vector3.one * v,
            min = 0,
            max = 180,
            value = 1,
        });

        m_parameters.Add(new GUIFloat()
        {
            name = "frequency",
            effect = v => m_brownianMotion.frequency = v,
            min = 0,
            max = 1,
            value = 0.1f,
        });
    }

}
