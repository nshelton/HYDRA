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


    public override void Init()
    {
        m_parameters.Add(new GUIFloat()
        {
            name = "fov",
            effect = v => m_camera.fieldOfView = v,
            min = 25,
            max = 120,
            value = 80,
        });


        m_parameters.Add(new GUIFloat()
        {
            name = "Offset",
            effect = v =>
            {
                foreach (var off in m_zOffsetTransforms)
                {
                    off.localPosition = Vector3.back * v;
                }
            },
            min = 0,
            max = 10,
            value = 2f,
        });

        m_parameters.Add(new GUIFloat()
        {
            name = "Intensity",
            effect = v =>
            {
                m_linear.angularVelocity = Vector3.up * v * 90f;
                m_brownian.rotationAmount = new Vector3(90, 180, 90) * v;
            },
            min = 0,
            max = 1,
            value = 0.5f,
        });

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
