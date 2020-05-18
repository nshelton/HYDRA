using Klak.Motion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{

    public SmoothFollow m_follow;


    public Transform[] m_transforms;


    public void SetTarget(int i)
    {
        m_follow.target = m_transforms[i];
    }

}
