﻿using Klak.Motion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModule : BaseGUIModule
{
    public Camera m_camera;

    public MeshRenderer m_spoutRenderer;
    public LinearMotion m_linear;
    public BrownianMotion m_brownian;
    public SmoothFollow m_follow;
    public RandomJump m_cameraJump;

    public Transform[] m_transforms;
    public Transform[] m_zOffsetTransforms;

    private float m_spoutScale = 1;

    public override string Name() { return "camera"; }

    public override void InitInternal()
    {
        Parameters.Add(new GUIFloat("spout", 0, 1, 1, delegate (float v)
        {
            m_spoutRenderer.sharedMaterial.SetColor("_Color", Color.white * v);
            m_spoutRenderer.sharedMaterial.SetColor("_EmissionColor", Color.white * v);
        }));

        Parameters.Add(new GUIFloat("spoutScale", 0, 2, 1, delegate (float v)
        {
            m_spoutScale = v;
        }));


        Parameters.Add(new GUIFloat("fov", 25, 120, 80, delegate(float v)
        {
            m_camera.fieldOfView = v;
            m_spoutRenderer.transform.localScale = 
                new Vector3(16, 9, 1) * 
                5/4 * m_spoutScale * Mathf.Tan(Mathf.Deg2Rad * v/2) ; 

        }));

        Parameters.Add(new GUIFloat("offset", 0, 10, 2, delegate (float v) 
        {
            foreach (var off in m_zOffsetTransforms)
            {
                off.localPosition = Vector3.back * v;
            }
        }));

        Parameters.Add(new GUIFloat("Intensity", 0, 0.3f, 0.1f, delegate(float v)
        {
                m_linear.angularVelocity = Vector3.up * v * 90f;
                m_brownian.rotationAmount = new Vector3(90, 180, 90) * v;
        }));

        foreach (var p in Parameters)
        {
            var r = new GUIRow();
            r.Items.Add(p);
            GUIRows.Add(r);
        }

        var row = new GUIRow();

        Parameters.Add( new GUITrigger( "Reset", delegate { 
            m_follow.target = m_transforms[0]; 
        }));

        row.Items.Add(Parameters[Parameters.Count - 1]);

        Parameters.Add(new GUITrigger("Noise", delegate {
            m_follow.target = m_transforms[1];
        }));

        row.Items.Add(Parameters[Parameters.Count - 1]);

        Parameters.Add(new GUITrigger("spinner", delegate {
            m_follow.target = m_transforms[2];
        }));

        row.Items.Add(Parameters[Parameters.Count - 1]);

        Parameters.Add(new GUITrigger("jump", delegate {
            m_cameraJump.Jump();
        }));

        row.Items.Add(Parameters[Parameters.Count - 1]);

        GUIRows.Add(row);
    }
}
