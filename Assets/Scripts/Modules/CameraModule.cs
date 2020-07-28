using Klak.Motion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModule : BaseGUIModule
{
    public Camera m_camera;

    public LinearMotion m_linear;
    public BrownianMotion m_brownian;
    public SmoothFollow m_follow;
    public RandomJump m_cameraJump;

    public Transform[] m_transforms;
    public Transform[] m_zOffsetTransforms;

    public override string Name() { return "camera"; }

    public override void Init()
    {

        m_parameters.Add(new GUIFloat("fov", 25, 120, 80, delegate(float v)
        {
            m_camera.fieldOfView = v;
        }));


        m_parameters.Add(new GUIFloat("offset", 0, 10, 2, delegate (float v) 
        {
            foreach (var off in m_zOffsetTransforms)
            {
                off.localPosition = Vector3.back * v;
            }
        }));

        m_parameters.Add(new GUIFloat("Intensity", 0, 1, 0.5f, delegate(float v)
        {
                m_linear.angularVelocity = Vector3.up * v * 90f;
                m_brownian.rotationAmount = new Vector3(90, 180, 90) * v;
        }));

        m_triggers.Add(new GUITrigger()
        {
            name = "Reset",
            effect = delegate { m_follow.target = m_transforms[0]; },
        });

        m_triggers.Add(new GUITrigger()
        {
            name = "Noise",
            effect = delegate { m_follow.target = m_transforms[1]; },
        });

        m_triggers.Add(new GUITrigger()
        {
            name = "spinner",
            effect = delegate { m_follow.target = m_transforms[2]; },
        });

        m_triggers.Add(new GUITrigger()
        {
            name = "jump", effect = delegate { m_cameraJump.Jump(); },
        });
    }
     
    
}
