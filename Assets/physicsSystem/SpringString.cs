﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringString : PhysicsSystem
{

    [SerializeField]
    private Rigidbody m_anchor1;

    [SerializeField]
    private SpringJoint m_anchor2;

    private class RopeNode
    {
        public Transform Transform;
        public Rigidbody Rigidbody;
        public SpringJoint SpringJoint;
        public Renderer Renderer;
    }

    private List<RopeNode> m_nodes = new List<RopeNode>();
    private MaterialPropertyBlock m_materialPropertyBlock;

    void Start()
    {
        var nodeObj0 = GameObject.Instantiate(m_physicsPrefab, transform);
        var node0 = new RopeNode();
        node0.SpringJoint = nodeObj0.GetComponent<SpringJoint>();
        node0.Rigidbody = nodeObj0.GetComponent<Rigidbody>();
        node0.Transform = nodeObj0.transform;
        node0.SpringJoint.connectedBody = m_anchor1;
        node0.Renderer = nodeObj0.GetComponent<Renderer>();
        m_nodes.Add(node0);


        for (int i = 1; i < m_numParticles; i ++)
        {
            var nodeObj = GameObject.Instantiate(m_physicsPrefab, transform);
            var node = new RopeNode();
            node.SpringJoint = nodeObj.GetComponent<SpringJoint>();
            node.Rigidbody = nodeObj.GetComponent<Rigidbody>();
            node.Transform = nodeObj.transform;
            node.Renderer = nodeObj.GetComponent<Renderer>();

            node.SpringJoint.connectedBody = m_nodes[i - 1].Rigidbody ;

            m_nodes.Add(node);
        }

        m_anchor2.connectedBody = m_nodes[m_nodes.Count-1].Rigidbody;
        m_materialPropertyBlock = new MaterialPropertyBlock();

    }

    private void Update()
    {

        for (int i = 0; i < m_nodes.Count; i ++)
        {
            float percent = ((float)i) / m_nodes.Count;
            float binNum = m_frequencyCurve.Evaluate(percent) * m_audioAnalyzer.spectrumArray.Length;

            float rawValue = m_audioAnalyzer.spectrumArray[(int)binNum % m_audioAnalyzer.spectrumArray.Length];
            float mappedValue = rawValue * m_frequencyAmpCurve.Evaluate(percent);

            float remap = m_scaleCurve.Evaluate(mappedValue);

            m_nodes[i].SpringJoint.spring = m_spring * 5000f;
            m_nodes[i].Rigidbody.mass = m_mass;
            m_nodes[i].Rigidbody.drag = m_drag;
             
            m_nodes[i].Transform.localScale = Vector3.one * remap;

            m_materialPropertyBlock.SetColor("_BaseColor", m_colorGrad.Evaluate(mappedValue));
            m_nodes[i].Renderer.SetPropertyBlock(m_materialPropertyBlock);
        }

    }

}
