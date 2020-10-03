using Lasp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioModule : BaseGUIModule
{

    public SpectrumAnalyzer m_analyzer;
    public SpectrumToTexture m_spectrum;
    public AudioLevelTracker m_level;
    public AudioLevelTracker m_bassLevel;
    public AudioLevelTracker m_trebleLevel;
    public BeatDetector m_beatDetect;

    private Texture2D m_levelTexture;
    private Texture2D m_beatTexture;
    int m_textureOffset = 0;

    public override string Name() { return "audio"; }
    public override bool ShowPresets() { return false; }

    public override void InitInternal()
    {
        RoutingServer.m_bass = m_bassLevel;
        RoutingServer.m_treble = m_trebleLevel;
        RoutingServer.m_bypass = m_level;
        RoutingServer.m_beat = m_beatDetect;

        m_beatTexture = new Texture2D(256, 64, UnityEngine.Experimental.Rendering.DefaultFormat.LDR, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
        m_levelTexture = new Texture2D(256, 64, UnityEngine.Experimental.Rendering.DefaultFormat.LDR, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);

        var row = new GUIRow();
        row.Items.Add(new GUIFloat("dynamic range", 1f, 50f, 25f, delegate( float v )
        { 
                m_level.dynamicRange = v;
                m_analyzer.dynamicRange = v; 
        }));
        GUIRows.Add(row);

        row = new GUIRow();

        row.Items.Add(new GUITexture(m_beatTexture));
        GUIRows.Add(row);

        row = new GUIRow();

        row.Items.Add(new GUITexture(m_levelTexture));
        GUIRows.Add(row);
    }

    public override void Update()
    {
        for (int i = 0; i < 64; i++)
        {
            float weight = m_beatDetect.GetBeat(1) * 0.125f + m_beatDetect.GetBeat(2) * 0.25f + m_beatDetect.GetBeat(4) * 0.5f;
            var color = Color.white * 0.5f;
            
            var pct = (float)i / 64f;
            if ( pct > weight)
                color.a = 0.0f;

            m_beatTexture.SetPixel(m_textureOffset, i, color);
        }
        m_beatTexture.Apply();

        for (int i = 0; i < 64; i ++)
        {
            var color = Color.white;
            color.a = 0.5f;
            var pct = (float)i / 64f;
            if (pct > m_bassLevel.normalizedLevel && pct > m_level.normalizedLevel)
                color = Color.clear;
            
            if (pct > m_bassLevel.normalizedLevel)
                color.r = 0;
            if (pct > m_level.normalizedLevel)
                color.g = 0;
            m_levelTexture.SetPixel(m_textureOffset, i, color);
        }
        m_levelTexture.Apply();
        m_textureOffset++;
    }
}
