using Lasp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSystem : MonoBehaviour
{

    [SerializeField]
    protected GameObject m_physicsPrefab;

    [SerializeField]
    protected int m_numParticles;

    [SerializeField]
    protected AnimationCurve m_scaleCurve;

    [SerializeField]
    protected AnimationCurve m_frequencyCurve;

    [SerializeField]
    protected AnimationCurve m_frequencyAmpCurve;

    [SerializeField]
    protected Gradient m_colorGrad;

    [SerializeField] protected float m_mass = 0.2f;
    [SerializeField] protected float m_drag = 1;
    [SerializeField] protected float m_spring = 1;
    [SerializeField] protected float m_lerp = 1;
    [SerializeField] protected float m_scale = 1;
    protected SpectrumAnalyzer m_audioAnalyzer;

    public float Mass
    {
        get { return m_mass / 4f; }
        set { m_mass = value * 4; }
    }

    public float Drag
    {
        get { return m_drag / 4f; }
        set { m_drag = value * 4; }
    }

    public float Spring
    {
        get { return m_spring / 4f; }
        set { m_spring = value * 4; }
    }

    public float Scale
    {
        get { return m_scale / 4f; }
        set { m_scale = value * 4; }
    }


    private void Awake()
    {
        m_audioAnalyzer = GameObject.FindObjectOfType<SpectrumAnalyzer>();
    }

}
