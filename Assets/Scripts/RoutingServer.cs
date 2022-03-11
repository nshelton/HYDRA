using Lasp;
using UnityEngine;

public static class RoutingServer
{
    public static BeatDetector m_beat;
    public static AudioLevelTracker m_bass;
    public static AudioLevelTracker m_treble;
    public static AudioLevelTracker m_bypass;

    public static float SampleOscillator(OscillatorType type, int beats)
    {
        var normalized = m_beat.GetBeat(beats); 
        switch (type)
        {
            case OscillatorType.Saw: return normalized;
            case OscillatorType.Sine: return Mathf.Sin( normalized * Mathf.PI * 2 ) * 0.5f + 0.5f ;
            default: break;
        }

        return normalized;
    }

    public static float SampleBass() { return m_bass.normalizedLevel; }
    public static float SampleBypass() { return m_bypass.normalizedLevel; }
    public static float SampleTreble() { return m_treble.normalizedLevel; }
}
