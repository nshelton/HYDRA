using Klak.Motion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{

    public LinearMotion m_linear;
    public BrownianMotion m_brownian;
    public SmoothFollow m_follow;

    public Transform[] m_transforms;
    public Transform[] m_zOffsetTransforms;

    public float CameraOffset
    {
        set
        {
            for (int i = 0; i < m_zOffsetTransforms.Length; i++)
            {
                m_zOffsetTransforms[i].localPosition = Vector3.back * value;
            }
        }
    }

    public float Intensity
    {
        set
        {
            m_linear.angularVelocity = Vector3.up * value * 90f;
            m_brownian.rotationAmount = new Vector3(90, 180, 90) * value;
        }
    }

    public void SetTarget(int i)
    {
        m_follow.target = m_transforms[i];
    }

}
