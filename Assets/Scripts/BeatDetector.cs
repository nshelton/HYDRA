using Lasp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircularBuffer;
using UnityEditor.Rendering;
using TreeEditor;
using System;
using System.Dynamic;

public class BeatDetector : MonoBehaviour
{

    int RMS_HISTORY_LENGTH = 512;

    int MAX_SHIFT = 128;
    int DEBUG_HEIGHT = 64;

    [SerializeField] AudioLevelTracker m_level;
    [SerializeField] AudioLevelTracker m_bass;

    Texture2D m_debugTexture;
    [SerializeField] RenderTexture m_fftTexture;

    CircularBuffer<float> m_levelBuffer;

    [SerializeField] float m_correlationLerp = 0.99f;

    public float[] m_weights;
    public float[] m_currentWeights;
    public float[] m_currentLevels;
    
    public int CurrentOffset;
    public float CurrentOffsetLagrange;
    public float CurrentBPM;
    public float CurrentBPMDouble;
    public float CurrentBPMLagrange;
    public float CurrentBPMHalf;

    int frameNum = 0;

    void Start()
    {
        m_weights = new float[MAX_SHIFT]; 
        m_currentWeights = new float[m_weights.Length];
        m_currentLevels = new float[RMS_HISTORY_LENGTH];
        m_levelBuffer = new CircularBuffer<float>(RMS_HISTORY_LENGTH);
        m_debugTexture = new Texture2D(RMS_HISTORY_LENGTH, DEBUG_HEIGHT);
        m_debugTexture.filterMode = FilterMode.Point;

        for (int y = 0; y < m_debugTexture.height; y++)
        {
            for (int x = 0; x < m_debugTexture.width; x++)
            {
                m_debugTexture.SetPixel(x, y, Color.black);
            }
        }

        m_debugTexture.Apply();
    }

    /*int getBPMBin(float bpm)
    {
        return (int)(bpm - BPM_MIN) * BPM_RESOLUTION; 
    }*/


    // Update is called at 100 FPS
    void FixedUpdate()
    {
        m_levelBuffer.PushBack(m_level.normalizedLevel);

        var arr = m_levelBuffer.ToArray();
        Array.Copy(arr, m_currentLevels, arr.Length);

        for (int s = 0; s < MAX_SHIFT; s ++)
        {
            float sad = 0;
            int numSamples = 0;
            for ( int i = 0; i < arr.Length; i ++)
            {
                // This could be optimised to skip these by looping smarter maybe
                if (  i + s < arr.Length )
                {
                    sad += Mathf.Abs(arr[i] - arr[i + s]);
                    numSamples++;
                }
            }
            if ( numSamples > 0)
                m_currentWeights[s] = sad / numSamples;
        }

        float alpha = m_correlationLerp;

        for (int  i = 0; i < m_weights.Length; i++)
        {
            m_weights[i] = alpha * m_weights[i] + (1.0f - alpha) * m_currentWeights[i];
        }

        float minval = 9999;
        int min_i = 0;

        for (int i =32; i < m_weights.Length; i++)
        {
            if (m_weights[i] < minval)
            {
                min_i = i;
                minval = m_weights[i];
            }
        }

        CurrentOffset = min_i;
        CurrentBPM = GetBPMFromOffset(CurrentOffset);
        CurrentOffsetLagrange = getLagrangeMinimum(CurrentOffset, m_weights);
        CurrentBPMLagrange = GetBPMFromOffset(CurrentOffsetLagrange);
        CurrentBPMDouble = CurrentBPM * 2;
        CurrentBPMHalf = CurrentBPM / 2;

        frameNum++;
    }

    public float getLagrangeMinimum(int offset, float[] m_values)
    {
        return offset + 1;
    }

    public float GetBPMFromOffset(float offset)
    {
        return 60f * 50f / offset;
    }

}
