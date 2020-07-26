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

    public override void Init()
    {
        RoutingServer.m_bass = m_bassLevel;
        RoutingServer.m_treble = m_trebleLevel;
        RoutingServer.m_bypass = m_level;
        RoutingServer.m_beat = m_beatDetect;

        m_beatTexture = new Texture2D(256, 64, UnityEngine.Experimental.Rendering.DefaultFormat.LDR, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
        m_levelTexture = new Texture2D(256, 64, UnityEngine.Experimental.Rendering.DefaultFormat.LDR, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
      
        m_parameters.Add(new GUIFloat()
        {
            effect = v => {
                m_level.dynamicRange = v;
                m_analyzer.dynamicRange = v; },
            min = 1,
            max = 40,
            value = 25,
            name = "dynamic range"
        });


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

    public override void DrawGUI(Rect area)
    {
        var itemRect = new Rect(
                 area.x + ITEMPADDING,
                 area.y + ITEMPADDING,
                 area.width - ITEMPADDING * 2,
                 ITEMHEIGHT - ITEMPADDING * 2);

        var oldBG = GUI.backgroundColor;
        GUI.backgroundColor = Color.clear;
        if (GUI.Button(itemRect, Name()))
        {
            m_hidden = !m_hidden;
        }
        GUI.backgroundColor = oldBG;
        itemRect.y += ITEMHEIGHT;

        if (m_hidden)
            return;
        
        foreach (var slider in m_parameters)
        {
            slider.DrawGUI(itemRect);
            itemRect.y += ITEMHEIGHT;
        }

        GUI.DrawTexture(itemRect, m_levelTexture);
        itemRect.y += ITEMHEIGHT;

        GUI.DrawTexture(itemRect, m_beatTexture);
        itemRect.y += ITEMHEIGHT;

        GUI.DrawTexture(itemRect, m_spectrum.texture);
        itemRect.y += ITEMHEIGHT;
        itemRect.width /= 4;
        GUI.DrawTexture(itemRect, (m_beatDetect.GetBeat(8) > 0.9) ? GUIUtility.LightGrayTexture : GUIUtility.GrayTexture);
        itemRect.x += itemRect.width;
        GUI.DrawTexture(itemRect, (m_beatDetect.GetBeat(4) > 0.9) ? GUIUtility.LightGrayTexture : GUIUtility.GrayTexture);
        itemRect.x += itemRect.width;
        GUI.DrawTexture(itemRect, (m_beatDetect.GetBeat(2) > 0.9) ? GUIUtility.LightGrayTexture : GUIUtility.GrayTexture);
        itemRect.x += itemRect.width;
        GUI.DrawTexture(itemRect, (m_beatDetect.GetBeat(1) > 0.9) ? GUIUtility.LightGrayTexture : GUIUtility.GrayTexture);
        itemRect.x += itemRect.width;
    }


}
