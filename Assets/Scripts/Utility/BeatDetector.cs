using Lasp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircularBuffer;
using UnityEditor.Rendering;
using TreeEditor;
using System;
using System.Dynamic;
using UnityEngine.SocialPlatforms.Impl;

public class BeatDetector : MonoBehaviour
{

    int RMS_HISTORY_LENGTH = 512;

    int MAX_SHIFT = 128;
    int DEBUG_HEIGHT = 64;

    [SerializeField] AudioLevelTracker m_level;

    Texture2D m_debugTexture;

    CircularBuffer<float> m_levelBuffer;

    [SerializeField] float m_correlationLerp = 0.99f;
    [SerializeField] float m_bpmAttentionSpan = 0.999f;

    public float[] m_weights;
    public float[] m_currentWeights;
    public float[] m_currentLevels;
    public float[] m_bpmHistogram;

    int frameNum = 0;
    public float BPM = 0;
    public float rawFreq = 0;
    public float fitFreq = 0;
    public float fitFreqBPM = 0;

    int MIN_BPM = 60;
    int MAX_BPM = 180;


    CircularBuffer<float> m_beatBuffer;
    CircularBuffer<float> m_BPMBuffer;
    CircularBuffer<float> m_LFOBuffer;

    public float GetBeat(int multiplier)
    {
         return 1f - Fract((Time.time - phaseOffset) * (BPM / multiplier) / 60f); 
    }

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

    [NonSerialized]  public float[] currentPeaks = new float[16];
    [NonSerialized]  public float[] currentPeakWeights = new float[16];
    [NonSerialized]  public float[] harmonicPeaks = new float[9];

    float lastPredictedBeatTime = 0;
    float lastBeatTime = 0;
    float m_lastLevel = 0;

    float phaseOffset = 0;
    public float harmonicThreshold = 1f;
    public float offsetError = 1f;


    [NonSerialized] public float[] m_fftreals = new float[512];
    [NonSerialized] public float[] m_fftcomplex = new float[512];
    [NonSerialized] public float[] m_fftMag = new float[128];
    [NonSerialized] public float[] m_fftMagAvg = new float[128];

    // Update is called at 50 FPS
    void FixedUpdate()
    {
        if (!m_levelBuffer.IsEmpty)
        {
            m_lastLevel = m_levelBuffer.Back();
        }

        m_levelBuffer.PushBack(m_level.normalizedLevel);
        m_BPMBuffer.PushBack(BPM);

        var arr = m_levelBuffer.ToArray();
        Array.Copy(arr, m_currentLevels, arr.Length);
        Array.Copy(arr, m_fftreals, arr.Length);
        Array.Copy(arr, m_fftcomplex, arr.Length);

        for (int s = 0; s < MAX_SHIFT; s ++)
        {
            float sad = 0;
            float numSamples = 0;
            for ( int i = 0; i < arr.Length; i ++)
            {
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

        GetHarmonicFromPeaks(ref currentPeaks, ref harmonicPeaks);

        GetMaxBPM();

        float thisFreq = BPM / 60f;

        if ( Input.GetKeyDown(KeyCode.Space))
        {
            phaseOffset = Time.time;
        }
     
        frameNum++;
    }


    private int HowManyOverlap(ref float[] peaks, float freq)
    {
        int n = peaks.Length;
        int k = m_weights.Length / (int)freq;
        int overlap = 0;
        for (int i = 0; i < k; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if(Mathf.Abs(peaks[j] - freq * i) < harmonicThreshold)
                {
                    overlap++;
                }
            }
        }

        return overlap;
    }

    public struct PointF
    {
        public float X;
        public float Y;

        public PointF(float x, float y) : this()
        {
            this.X = x;
            this.Y = y;
        }
    }

    public float FindSlope(List<PointF> points )
    {
        float[] beatWeight = new float[] { 0, 1, 20, 1, 10, 1, 40, 1, 10 };
        float avg = 0;
        float weight = 0;

        foreach (PointF p in points)
        {
            if ( p.X > 0 && p.Y > 0 && p.X < beatWeight.Length)
            {
                weight += p.X * p.X * beatWeight[(int)p.X];
                avg += (p.Y / p.X) * (p.X * p.X * beatWeight[(int)p.X]);
            }

        }

        return avg / weight;
    }

    private void GetHarmonicFromPeaks(ref float[] peaks, ref float[] harmonics)
    {
        int n = peaks.Length;
        float bestfreq = -1;
        float bestScore = -1;

        for (int i = 0; i < n; i ++)
        {
            if (peaks[i] == 0)
                continue;

            for (int fundamental = 5; fundamental > 0; fundamental--)
            { 
                float freq = peaks[i] / fundamental;
                
                if ( freq > 10)
                {
                    int score = HowManyOverlap(ref peaks, freq);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestfreq = freq;
                    }
                }
            }
        }

  
        rawFreq = bestfreq;

        List<PointF> p = new List<PointF>();


        for(int i = 0; i < n; i ++)
        {
            float k = Mathf.Round(peaks[i] / bestfreq);
            if ( Mathf.Abs(k * bestfreq - peaks[i])  < harmonicThreshold)
            {
                p.Add(new PointF(k, peaks[i]));
            }
        }

        float currentFitFreq = FindSlope(p);

        float error = 0;
        float errorNum = 0;
        for (int i = 0; i < p.Count; i++)
        {
            if (p[i].X > 0)
            {
                error += Mathf.Abs(p[i].Y - p[i].X * currentFitFreq);
                errorNum++;
            }

        }
        error = error / errorNum;

        if ( error > 0)
        {
        
            float weight = 1f - 1f / (1f + Mathf.Exp(-error));
            AddInPeak(fitFreq * 2, weight);

            fitFreq = fitFreq * (1f - weight) + currentFitFreq * weight;
        }


        for (int j = 0; j < harmonics.Length; j++)
        {
            harmonics[j] = fitFreq * j;
        }

        fitFreqBPM= GetBPMFromOffset(fitFreq*2);
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

    private void AddInPeak(float peak, float weight)
    {
        float floatOffset = peak;
        if (floatOffset > 10 && floatOffset < m_bpmHistogram.Length - WINDOW_SIZE)
        {
            float bpm = GetBPMFromOffset(floatOffset);
            if (bpm > MIN_BPM && bpm < MAX_BPM)
            {
                for (int d = -WINDOW_SIZE; d <= WINDOW_SIZE; d++)
                {
                    int d_i = (int)bpm + d;
                    float window = 1 - Mathf.Abs((bpm - d_i) / WINDOW_SIZE);
                    m_bpmHistogram[d_i] += window * weight;
                }
            }
        }

        for (int i = 0; i < m_bpmHistogram.Length; i++)
        {
        m_bpmHistogram[i] *= m_bpmAttentionSpan;
        }
    }

    private float Fract(float i)
    {
        return i - Mathf.Floor(i);
    }

    public void  GetPeaks(float[] data, int start)
    {
        var peakIndex = 1;
        currentPeaks[0] = 0;
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
