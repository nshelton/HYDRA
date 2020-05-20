using Lasp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircularBuffer;
using UnityEditor.Rendering;
using TreeEditor;
using System;
using System.Dynamic;
using NAudio.Dsp;

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
    public float[] m_bpmHistogram;

    int frameNum = 0;
    public float BPM = 0;
    public float BEAT = 0;


    int MIN_BPM = 60;
    int MAX_BPM = 180;
    int BPM_DIVISIONS = 4;


    CircularBuffer<float> m_beatBuffer;
    CircularBuffer<float> m_BPMBuffer;
    CircularBuffer<float> m_LFOBuffer;


    public float[] BeatHistory
    {
        get { return m_beatBuffer.ToArray(); }
    }

    public float[] BPMHistory
    {
        get { return m_BPMBuffer.ToArray(); }
    }
    public float[] LFOHistory
    {
        get { return m_LFOBuffer.ToArray(); }
    }

    void Start()
    {
        m_weights = new float[MAX_SHIFT]; 
        m_currentWeights = new float[m_weights.Length];
        m_currentLevels = new float[RMS_HISTORY_LENGTH];

        //m_bpmHistogram = new float[(MAX_BPM - MIN_BPM) * BPM_DIVISIONS];
        m_bpmHistogram = new float[256];
        m_levelBuffer = new CircularBuffer<float>(RMS_HISTORY_LENGTH);
        m_beatBuffer = new CircularBuffer<float>(RMS_HISTORY_LENGTH);
        m_BPMBuffer = new CircularBuffer<float>(RMS_HISTORY_LENGTH);
        m_LFOBuffer = new CircularBuffer<float>(RMS_HISTORY_LENGTH);
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

    [NonSerialized]  public float[] currentPeaks = new float[8];
    [NonSerialized]  public float[] currentPeakWeights = new float[8];
    [NonSerialized]  public float[] harmonicPeaks = new float[4];
    [NonSerialized]  public float[] goodHarmonics = new float[4];

    float lastPredictedBeatTime = 0;
    float lastBeatTime = 0;
    float m_lastLevel = 0;

    float phaseOffset = 0;
    
    
    [NonSerialized] public float[] m_fftreals = new float[512];
    [NonSerialized] public float[] m_fftcomplex = new float[512];
    [NonSerialized] public float[] m_fftMag = new float[128];
    [NonSerialized] public float[] m_fftMagAvg = new float[128];


    public int fftSkip = 0;

    // Update is called at 100 FPS
    void FixedUpdate()
    {
        if (!m_levelBuffer.IsEmpty)
        {
            m_lastLevel = m_levelBuffer.Back();
        }

        m_levelBuffer.PushBack(m_level.normalizedLevel);
        m_beatBuffer.PushBack(BEAT);
        m_BPMBuffer.PushBack(BPM);

        var arr = m_levelBuffer.ToArray();
        Array.Copy(arr, m_currentLevels, arr.Length);
        Array.Copy(arr, m_fftreals, arr.Length);
        Array.Copy(arr, m_fftcomplex, arr.Length);

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

        for (int i = 0; i <currentPeaks.Length; i++)
        {
            currentPeaks[i] = 0;
            currentPeakWeights[i] = 0;
        }

        GetPeaks(m_weights, 15);
        //FindHarmonics(ref currentPeaks, ref harmonicPeaks);
        //GetGoodHarmonics(ref harmonicPeaks, ref goodHarmonics);

        AddInHarmonicPeaks(currentPeaks, currentPeakWeights);

        var lastBPM = BPM;

        GetMaxBPM();

        // impulse detection
        if (Time.time - lastBeatTime >( BPM/60) /2)
        {
            if (m_lastLevel < m_levelBuffer.Back() && m_levelBuffer.Back() > 0.99)
            {
                lastBeatTime = Time.time;
                BEAT = 1f;
            }
        }


        FastFourierTransform.FFT(true, 9, m_fftreals, m_fftcomplex);

        for (int i = 0; i < fftSkip; i++)
        {
            m_fftMag[i] = 0;
            m_fftreals[i] = 0;
            m_fftcomplex[i] = 0;
            m_fftMagAvg[i] = 0;
        }

        for (int i =fftSkip; i < m_fftMag.Length; i ++)
        {
            m_fftMag[i] = Mathf.Sqrt( m_fftreals[i] * m_fftreals[i] + m_fftcomplex[i] * m_fftcomplex[i]);
            m_fftMagAvg[i] = Mathf.Lerp(m_fftMag[i], m_fftMagAvg[i], m_correlationLerp);
        }

        BEAT *= 0.9f;

        var beat = Mathf.Round(BPM * 2)/8 * (Time.time - lastPredictedBeatTime) / 60f ;

        if (( BEAT > 0.9 && beat > 0.9) || lastPredictedBeatTime > 5)
        {
            lastPredictedBeatTime = Time.time;
            m_LFOBuffer.PushBack(1);
        }


        phaseOffset = 0;

        float lfo = Mathf.Pow(0.5f + 0.5f * Mathf.Cos((beat - phaseOffset) * Mathf.PI * 2f), 1f);
        m_LFOBuffer.PushBack(lfo);


        Camera.main.backgroundColor = Color.blue *lfo ;
        Camera.main.backgroundColor += Color.red * BEAT;
       
        frameNum++;
    }

    private void GetMaxBPM()
    {
        float maxVal = 0;
        float maxBpm = 0;
        for (int i = 0; i < m_bpmHistogram.Length; i ++)
        {
            if ( m_bpmHistogram[i] > maxVal)
            {
                maxVal = m_bpmHistogram[i];
                maxBpm = getLagrangeMinimum(i, m_bpmHistogram);
            }
        }
        BPM = maxBpm;
    }

    int WINDOW_SIZE = 4;

    private void AddInHarmonicPeaks(float[] harmonicPeaks, float[] weights)
    {
        for(int i = 0; i < harmonicPeaks.Length; i ++)
        {
            float floatOffset = harmonicPeaks[i];
            if ( floatOffset > 10 && floatOffset < m_bpmHistogram.Length - WINDOW_SIZE)
            {
                float bpm = GetBPMFromOffset(floatOffset);

                for (float multiplier = 1; multiplier <= 4.0; multiplier ++)
                {
                    float tgtbpm = bpm* multiplier;
                    if (tgtbpm > MIN_BPM && tgtbpm < MAX_BPM)
                    {
                         for (int d = -WINDOW_SIZE; d <= WINDOW_SIZE; d++)
                         {
                             int d_i = (int)tgtbpm + d;
                             float weight = 1 - Mathf.Abs((tgtbpm - d_i) / WINDOW_SIZE);
                             m_bpmHistogram[d_i] += weight * weights[i];

                         }
                    }
                }
                for (float multiplier = 2; multiplier <= 4.0; multiplier++)
                {
                    float tgtbpm = bpm / multiplier;
                    if (tgtbpm > MIN_BPM && tgtbpm < MAX_BPM)
                    {
                        for (int d = -WINDOW_SIZE; d <= WINDOW_SIZE; d++)
                        {
                            int d_i = (int)tgtbpm + d;
                            float weight = 1 - Mathf.Abs((tgtbpm - d_i) / WINDOW_SIZE);
                            m_bpmHistogram[d_i] += weight * weights[i];

                        }
                    }
                }
            }
        }

        for (int i = 0; i < m_bpmHistogram.Length; i++)
        {
            m_bpmHistogram[i] *= 0.99f;
        }
    }

    private float Fract(float i)
    {
        return i - Mathf.Floor(i);
    }

    private void GetGoodHarmonics(ref float[] harmonicPeaks, ref float[] goodHarmonics)
    {
        for (int i = 0; i < goodHarmonics.Length; i++)
            goodHarmonics[i] = 0;


        Array.Sort(harmonicPeaks);
        int goodIndex = 1;
        goodHarmonics[0] = harmonicPeaks[0];
        for(int i = 1; i < harmonicPeaks.Length; i ++)
        {
            if (Fract(harmonicPeaks[i] / harmonicPeaks[0]) < 0.15f)
            {
                goodHarmonics[goodIndex] = harmonicPeaks[i];
                goodIndex++;
                if (goodIndex >= goodHarmonics.Length)
                    break;
            }

        }
    }

    private void FindHarmonics(ref float[] currentPeaks, ref float[] harmonicPeaks)
    {
        for(int i = 0; i < harmonicPeaks.Length; i++)
            harmonicPeaks[i] = 0;

        int harmonicNum = 0;
        for(int i = 0; i <  currentPeaks.Length; i++ )
        {
            int h = 0;
            for (int j = 0; j < currentPeaks.Length; j++)
            {
                if ( Mathf.Abs(currentPeaks[i] - currentPeaks[j]*2) < 1f ||
                    Mathf.Abs(currentPeaks[i] - currentPeaks[j] / 2) < 1f)
                {
                    h++;
                }
            }
            if ( h > 0)
            {
                harmonicPeaks[harmonicNum] = currentPeaks[i];
                harmonicNum++;
                if (harmonicNum == harmonicPeaks.Length)
                    break;
            }
        }
    }

    public void  GetPeaks(float[] data, int start)
    {
        var peakIndex = 0;
        for (int i = start+1; i < data.Length - 1; i ++)
        {
            if ( data[i-1] > data[i] && data[i+1] > data[i])
            {
                currentPeaks[peakIndex] = getLagrangeMinimum(i, data);
                currentPeakWeights[peakIndex] = 
                    Mathf.Abs(data[i - 1] - data[i]) + 
                    Mathf.Abs(data[i + 1] - data[i]) ;

                peakIndex++;

                if (peakIndex == currentPeaks.Length)
                    break;
            }
        }
    }

    public float getLagrangeMinimum(int offset, float[] m_values)
    {
        // shouldn't really happen
        if (offset == 0 || offset == m_values.Length-1)
        {
            return m_values[offset];
        }

        var a = m_values[offset - 1];
        var b = m_values[offset];
        var c = m_values[offset + 1];

        // analytic min/max of the 2nd order lagrange interpolation polynomial 
        float y = (a - c) / (2 * (a - 2 * b + c));

        return offset + y;
    }

    public float GetBPMFromOffset(float offset)
    {
        return 60f * 50f / offset;
    }

}
